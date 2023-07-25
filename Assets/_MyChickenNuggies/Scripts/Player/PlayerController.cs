using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour {
    public static PlayerController Instance;
    public PlayerInputActions Input;
    
    [SerializeField] private float interactRadius;
    [SerializeField] private float grabCooldown = 0.1f;
    [SerializeField] private LayerMask interactMask;
    [SerializeField] private PlayerArm arm;
    [SerializeField] private GameObject interactPrompt;

    public bool IsBeingGrabbed { get; private set; }
    private float _currentGrabCooldown;
    
    private GameObject _currentInteractable;
    private bool _enabled;


    private void Awake() {
        if (Instance == null) Instance = this;

        Input = new PlayerInputActions();
        Input.Walking.Enable();
    }

    private void Start() {
        Input.Walking.Interact.performed += OnPressInteract;
        Input.Walking.Punch.performed += arm.Punch;
    }

    private void Update() {
        if (_currentGrabCooldown > 0) _currentGrabCooldown -= Time.deltaTime;
    }

    private void FixedUpdate() {
        if (!_enabled) return;
        
        GetInteractable();
    }


    public void Enable() {
        Input.Walking.Enable();
        
        arm.enabled = true;
        GetComponent<PlayerMovement>().enabled = true;
        
        _enabled = true;
    }
     
    public void Disable() {
        _enabled = false;
        IsBeingGrabbed = false;
        _currentInteractable = null;
        interactPrompt.SetActive(false);
        
        arm.enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        
        Input.Walking.Disable();
    }

    public bool Grab() {
        if (_currentGrabCooldown > 0) return false;
        IsBeingGrabbed = true;
        return true;
    }

    public void Release() {
        IsBeingGrabbed = false;
        _currentGrabCooldown = grabCooldown;
    }

    private void OnPressInteract(InputAction.CallbackContext ctx) {
        if (_currentInteractable == null) return;
        _currentInteractable.GetComponent<IInteractable>().Interact(gameObject);
    }

    private void GetInteractable() {
        Collider2D col = Physics2D.OverlapCircle(transform.position, interactRadius, interactMask);
        if (col == null) {
            if (_currentInteractable == null) return;
            
            _currentInteractable = null;
            OnLeaveInteractable();

            return;
        }
        
        if (col.gameObject.Equals(_currentInteractable)) return;
        _currentInteractable = col.gameObject;
        OnEnterInteractable();
    }

    private void OnEnterInteractable() {
        interactPrompt.SetActive(true);
    }

    private void OnLeaveInteractable() {
        interactPrompt.SetActive(false);
    }


    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
