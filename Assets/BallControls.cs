using UnityEngine;
using UnityEngine.InputSystem;

public class BallMover : MonoBehaviour
{
    public float speed = 50f;
    public float maxSpeed = 25f;
    public Transform cameraTransform;

    private Rigidbody rb;
    private PlayerControls controls;
    private Vector2 moveInput;
    private bool isCrouching = false;
    private int groundContacts = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();

        controls.Gameplay.Move.performed += OnMove;
        controls.Gameplay.Move.canceled += OnMove;

        controls.Gameplay.Crouch.performed += OnCrouch;
        controls.Gameplay.Crouch.canceled += OnCrouch;
    }

    void OnDisable()
    {
        controls.Gameplay.Move.performed -= OnMove;
        controls.Gameplay.Move.canceled -= OnMove;

        controls.Gameplay.Crouch.performed -= OnCrouch;
        controls.Gameplay.Crouch.canceled -= OnCrouch;

        controls.Gameplay.Disable();
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    void OnCrouch(InputAction.CallbackContext ctx)
    {

        isCrouching = ctx.ReadValueAsButton();
    }
    
    void OnCollisionEnter(Collision collision)
    {
        groundContacts++;
    }

    void OnCollisionExit(Collision collision)
    {
        groundContacts--;
    }
    
    bool IsTouchingSomething()
    {
        return groundContacts > 0;
    }
    
    void FixedUpdate()
    {
        bool crouchActive = isCrouching && IsTouchingSomething();
        
        if (crouchActive) 
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }
    
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        
        Vector3 movement = (camForward * moveInput.y + camRight * moveInput.x);
        rb.AddForce(movement * speed, ForceMode.Acceleration);

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            Vector3 clamped = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(clamped.x, rb.linearVelocity.y, clamped.z);
        }
    }
}