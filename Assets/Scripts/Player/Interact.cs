using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    //Input System
    PlayerInput playerInput;
    InputSystem_Actions inputHandler;
    //Variables
    [SerializeField] float interactRange = 7;

    IInteract _interactable = null;
    void Start()
    {
        GetCom();
    }

    void GetCom()
    {
        inputHandler = new InputSystem_Actions();
        playerInput = GetComponent<PlayerInput>();
        inputHandler.Player.Enable();
        inputHandler.Player.Interact.performed += ctx => InteractWithObject();
    }
    void InteractWithObject()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider c in col)
        {
            if (c == null) return;
            _interactable = c.GetComponent<IInteract>();
            if (_interactable != null)
            {
                _interactable.Interact();
            }
        }
    }
}
public interface IInteract
{
    public void Interact();
}