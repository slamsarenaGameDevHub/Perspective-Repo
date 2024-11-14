using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using static WalkingNpcType;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class WalkingNPC : MonoBehaviour,IInteract
{
    #region Movement
    internal enum Direction
    {
        Backward,
        Forward
    }
    [SerializeField] Direction _direction;

    NavMeshAgent agent;
    Animator animator;


    [SerializeField] AnimatorOverrideController animatorOverride;

    [SerializeField] int activePath, currentWaypoint;
    [SerializeField] float switchDistance = 7;
    [SerializeField] Transform[] Path;
    Transform CurrentPath;
    List<Transform> nodes;


    Vector3 lastPos;
    [HideInInspector]
    public bool hasChangedPath = false;

    float Speed, nextTimeToChangePath;
    [SerializeField] float ChangeRate = 50, minSpeed = 0.9f, maxSpeed = 2.5f, agentSpeed = 1;
    [SerializeField] float walkAnimationThreshold;

    AudioSource footstepSource;

    #endregion
    [Header("Player interaction")]
    [SerializeField] Transform eye;
    Transform player;
    public bool canInteract=false;

    float countDown;
    [SerializeField] float decideDelay = 4;
    [SerializeField] float playerRange = 3;
    int npcTypeDeterminer,maxHintValue,hintTracker=0;

    [SerializeField] GameObject HintBg,interactButton;
    [SerializeField] TMP_Text HintText;
    [SerializeField] NpcTypes _npcType;

    HintManager hintManager;

    void OnEnable()
    {
        GetCom();
        ChooseWaypoint();
    }
   
    void GetCom()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = animatorOverride;
        footstepSource = GetComponent<AudioSource>();
        lastPos = transform.position;
        player = FindFirstObjectByType<PlayerMovement>().transform;
        countDown = decideDelay;
        hintManager = FindFirstObjectByType<HintManager>();
        maxHintValue = hintManager.hints.Count;
        HintBg.SetActive(false);
    }
    void Update()
    {
        Speed = Vector3.Distance(transform.position, lastPos) / Time.deltaTime;
        lastPos = transform.position;
        DecideType();
        CheckForPlayer();
        PlayAnimation();
        Move();
        activePath = Random.Range(0, Path.Length);
        if (Time.time >= nextTimeToChangePath)
        {
            nextTimeToChangePath = Time.time + ChangeRate;
            ChooseWaypoint();
        }

    }
    void CheckForPlayer()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, playerRange);
        foreach(Collider c in col)
        {
            PlayerMovement player=c.GetComponent<PlayerMovement>();
            if(player != null)
            {
                HintBg.transform.rotation = Camera.main.transform.rotation;
                canInteract = true;
                if (HintBg.activeInHierarchy || _npcType==NpcTypes.None) return;
                interactButton.SetActive(true);
            }

        }
        if(Vector3.Distance(transform.position,player.position)>playerRange)
        {
            interactButton.SetActive(false);
            canInteract = false;
            HintBg.SetActive(false);
            interactButton.SetActive(false);
        }
        if(_npcType==NpcTypes.None)
        {
            print("Dont stop");
        }
    }
    void ChooseWaypoint()
    {
        for (int i = 0; i < Path.Length; i++)
        {
            if (i == activePath)
            {
                CurrentPath = Path[i];  
            }
        }
        GetNodes();
    }
    void GetNodes()
    {
        Transform[] waypoints = CurrentPath.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != CurrentPath)
            {
                nodes.Add(waypoints[i]);
            }
        }
        currentWaypoint = Random.Range(0, nodes.Count);
    }
    void DecideType()
    {
        countDown -= Time.deltaTime;
        if (countDown > 0)
        {
            npcTypeDeterminer = Random.Range(0, 6);
            hintTracker = Random.Range(0, maxHintValue);
        }
        if (countDown <= 0)
        {
            countDown = 0;
            switch (npcTypeDeterminer)
            {
                case 0:
                    _npcType = NpcTypes.Hint;
                    break;
                case 1:
                    _npcType= NpcTypes.Question;
                    break;
                case 2:
                    _npcType= NpcTypes.None;
                    break;
                case 3:
                    _npcType = NpcTypes.Question;
                    break;
                case 4:
                    _npcType= NpcTypes.None;
                    break;
                    case 5:
                    _npcType = NpcTypes.Hint;
                    break;
            }
        }
        if(_npcType==NpcTypes.None)
        {
            agent.isStopped = false;
        }
       
    }
    public void Interact()
    {
        if (!canInteract) return;
        interactButton.SetActive(false);
        switch (_npcType)
        {
            case WalkingNpcType.NpcTypes.None:
                print("None");
                break;
            case WalkingNpcType.NpcTypes.Question:
                agent.isStopped = true;
                FindFirstObjectByType<FillInTheBlank>().AskQuestion(this);
                break;
            case WalkingNpcType.NpcTypes.Hint:
                print("Hint BG open");
                HintBg.SetActive(true);
                HintText.text = hintManager.GiveHint(hintTracker);
                break;
        }
    }
    void Move()
    {
        if (nodes.Count <= 0) return;

        agentSpeed = minSpeed;

        agent.speed = agentSpeed;

        if (_direction == Direction.Forward)
        {
            if (Vector3.Distance(transform.position, nodes[currentWaypoint].position) < switchDistance)
            {
                if (currentWaypoint == nodes.Count - 1)
                {
                    currentWaypoint = 0;
                }
                else
                {
                    currentWaypoint++;
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, nodes[currentWaypoint].position) < switchDistance)
            {
                if (currentWaypoint == 0)
                {
                    currentWaypoint = nodes.Count - 1;
                }
                else
                {
                    currentWaypoint--;
                }
            }
        }
        agent.SetDestination(nodes[currentWaypoint].position);
    }
    void PlayAnimation()
    {
        if (Speed > .1f && Speed < walkAnimationThreshold)
        {

            animator.SetFloat("Motion", 1);
        }
        else if (Speed >= walkAnimationThreshold)
        {
            animator.SetFloat("Motion", 2);           
        }
        else
        {
            animator.SetFloat("Motion", 0);
        }
    }
    public void PlaySound()
    {
        footstepSource.Play();
    }
    public void ChangeState()
    {
        npcTypeDeterminer=2;
    }
}

[System.Serializable]
public class WalkingNpcType
{
    public enum NpcTypes
    {
        Hint,
        Question,
        None
    }
    public NpcTypes NpcType;
}