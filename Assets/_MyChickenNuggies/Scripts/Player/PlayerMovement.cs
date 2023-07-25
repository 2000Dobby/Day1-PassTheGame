using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour {
    private Rigidbody2D _rb;
    private PlayerInputActions _input;

    [SerializeField] private float speed = 3.0f;
    
    [HideInInspector] public Vector2 inputVector;
    
    
    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        _input = PlayerController.Instance.Input;
    }

    private void Update() {
        if (PlayerController.Instance.IsBeingGrabbed) {
            inputVector = Vector2.zero;
            return;
        }
        
        inputVector = _input.Walking.Movement.ReadValue<Vector2>();
    }

    private void FixedUpdate() {
        _rb.velocity = ((Vector2) transform.forward + inputVector) * speed;
    }


    public void OnDisable() {
        _rb.velocity = Vector2.zero;
    }
}
