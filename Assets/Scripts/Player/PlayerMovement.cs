using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController),(typeof(AudioSource)))]
public class PlayerMovement : MonoBehaviour
{
    #region Input
    PlayerInput playerInput;
    InputSystem_Actions inputHandler;
    #endregion 

    [Header("Components")]
    CharacterController characterController;
    Animator animator;

    [Header("Movement")]
    float speed;
    float relativeVelocity;
    [SerializeField] float maxSpeed = 10;
    [Tooltip("The Turn Time Shouldn't pass 0.4f")]
    [SerializeField] float smoothTurnTime = 0.1f;
    [SerializeField] float walkThreshold = 2.5f;
    Vector3 moveDir;

    //Gravity
    [SerializeField] float jumpHeight = 3;
    const float gravity = -9.81f;
    Vector3 velocity;

    //Slide
    Vector3 hitNormalPoint;
    private bool isSliding
    {
        get
        {
            if (isGrounded() && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 50))
            {
                hitNormalPoint = slopeHit.normal;
                return Vector3.Angle(hitNormalPoint, Vector3.up) > characterController.slopeLimit;
            }
            else
            {
                return false;
            }
        }
    }


    Vector3 lastPos;


    [Header("Ground Check")]
    [SerializeField] Transform GroundD;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundRadius = 0.3f;


    [Header("Footstep Sounds")]
    string _TerrainName;
    AudioSource source;
    AudioClip footstepClip;
    [SerializeField] TerrainType[] TerrainTypes;
    void Start()
    {
        GetCom();
    }
    void GetCom()
    {
        playerInput = GetComponent<PlayerInput>();
        inputHandler = new InputSystem_Actions();
        inputHandler.Player.Enable();
        characterController = GetComponent<CharacterController>();
        inputHandler.Player.Jump.performed += ctx => Jump();
        animator = GetComponent<Animator>();
        lastPos = transform.position;
        source = GetComponent<AudioSource>();
    }
    void Update()
    {
        CalculateSpeed();
        Move();
        ApplyGravity();
    }
    void ApplyGravity()
    {
        if (isGrounded() && velocity.y < 0)
        {
            velocity.y = -2;
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    void Move()
    {
        Vector2 move = inputHandler.Player.Move.ReadValue<Vector2>();
        Vector3 movement = new Vector3(move.x, 0, move.y).normalized;
        if(movement.magnitude>=0.1f)
        {
            float targetAngle=Mathf.Atan2(movement.x,movement.z)*Mathf.Rad2Deg+Camera.main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref relativeVelocity, smoothTurnTime);
            transform.rotation=Quaternion.Euler(0,angle,0);
            
            Vector3 moveDir=Quaternion.Euler(0,targetAngle,0)*Vector3.forward;
            characterController.Move(moveDir*maxSpeed*Time.deltaTime);
        }
        if(isSliding)
        {
            characterController.Move(hitNormalPoint*maxSpeed*Time.deltaTime);
        }
        PlayAnimation();
    }
    void Jump()
    {
        if (!isGrounded()) return;
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        characterController.Move(velocity * Time.deltaTime);
    }
    bool isGrounded() => Physics.CheckSphere(GroundD.position, groundRadius,groundLayer);
    void CalculateSpeed()
    {
        speed = Vector3.Distance(transform.position, lastPos)/Time.deltaTime;
        lastPos = transform.position;
    }
    void PlayAnimation()
    {
        if (animator == null) return;
        if (speed >= walkThreshold && isGrounded())
        {
            animator.SetFloat("Motion", 2);
        }
        else if(speed>0.1f && isGrounded())
        {
            animator.SetFloat("Motion", 1);
        }
        else if(!isGrounded())
        {
            animator.SetFloat("Motion", 3);
        }
        else
        {
            animator.SetFloat("Motion", 0);
        }


    }
    public void PlaySound()
    {
        source.clip = footstepClip;
        RaycastHit hit;
        if(Physics.Raycast(transform.position,Vector3.down,out hit,10))
        {
            _TerrainName = hit.transform.tag;
        }
        for (int i = 0; i < TerrainTypes.Length; i++)
        {
            if (_TerrainName == TerrainTypes[i].TerrainName)
            {
                footstepClip = TerrainTypes[i].terrainClip;
            }
            else
            {
                footstepClip = TerrainTypes[0].terrainClip;
            }
        }
        source.Play();
    }

}
[System.Serializable]
public class TerrainType
{
    public string TerrainName;
    public AudioClip terrainClip;
}
