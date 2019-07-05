using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoobaCharacterController : MonoBehaviour
{
    public Animator Anim;
    private Rigidbody Rb;
    private CapsuleCollider Collider;
    private Vector3 ColliderExtents;
    private Vector3 MovementVector = Vector3.zero;
    private Vector2 ExternalInput = Vector2.zero;
    private Vector2 InputVector = Vector2.zero;
    private Vector2 MoveInput = Vector2.zero;
    private Coroutine ExternalInput_Coroutine;
    private Vector3 LookDirection = Vector3.forward;
    private float RotationSpeed = 20.0f;
    private float MovementSpeed = 8.0f;
    private float AirMovementSpeed = 1.0f;
    private float MaxMovementSpeed = 10.0f;
    private float JumpSpeed = 5.0f;
    private float MaxMovementVelocity = 10.0f;
    private float MaxMovementVelocityWhenJumping = 3.0f;
    private float BeginningJumpingTime;
    private float EndJumpingTime = 1.0f;
    private float JumpingTime = 0.6f;
    private bool Jumping;
    private bool DidJump;
    private bool Grounded;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Collider = GetComponent<CapsuleCollider>();
        ColliderExtents = Collider.bounds.extents;
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleRotation();
        HandleJump();
        HandleGrounded();
        HandleAnimations();
    }

    private void HandleInput()
    {
        // when external input, override all input
        if (ExternalInput != Vector2.zero)
        {
            InputVector = ExternalInput;
        }
        else if (MoveInput != Vector2.zero)
        {
            InputVector = MoveInput;
        }
        else
        {
            InputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }

    private void HandleRotation()
    {
        var targetRotation = Quaternion.LookRotation(LookDirection);
        var newRotation = Quaternion.Slerp(Rb.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        Rb.MoveRotation(newRotation);
    }

    private void HandleMovement()
    {
        MovementVector = new Vector3(InputVector.x, 0, InputVector.y);
        LookDirection = Vector3.RotateTowards(transform.forward, MovementVector, RotationSpeed * Time.deltaTime, 0.0f);
        var moveSpeed = Grounded ? MovementSpeed : AirMovementSpeed;
        var newVelocity = Rb.velocity + (MovementVector * moveSpeed);
        var movementSpeed = Vector3.ClampMagnitude(newVelocity, MaxMovementVelocity).magnitude;
        Rb.velocity += MovementVector * movementSpeed * Time.deltaTime;
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && Grounded)
        {
            Jumping = true;
            BeginningJumpingTime = Time.time;
        }
        if (Jumping)
        {
            if (!DidJump && Rb.velocity.magnitude > MaxMovementVelocityWhenJumping)
            {
                var movementSpeed = Vector3.ClampMagnitude(Rb.velocity, MaxMovementVelocityWhenJumping).magnitude;
                Rb.velocity = MovementVector * movementSpeed * Time.deltaTime;
            }
            if (!DidJump && Time.time - BeginningJumpingTime >= JumpingTime)
            {
                DidJump = true;
                Rb.velocity += new Vector3(0, 1, 0) * JumpSpeed;
            }
            if (DidJump && Time.time - BeginningJumpingTime > EndJumpingTime)
            {
                Jumping = false;
                DidJump = false;
            }
        }
    }

    private void HandleGrounded()
    {
        int layerMask = 1 << 8;
        RaycastHit hitinfo;
        Grounded = Physics.Raycast(transform.position, Vector3.down, out hitinfo, ColliderExtents.y - 0.1f, layerMask);
    }

    private void HandleAnimations()
    {
        Anim.SetBool("Jump", Jumping);
        Anim.SetBool("Grounded", Grounded);
        Anim.SetFloat("Velocity", Rb.velocity.magnitude);
    }

    public void AddExternalInput(Vector2 externalInput, float clearAfterTime)
    {
        ExternalInput = externalInput;
        if (ExternalInput_Coroutine != null)
        {
            StopCoroutine(ExternalInput_Coroutine);
            ExternalInput_Coroutine = null;
        }
        ExternalInput_Coroutine = StartCoroutine(ClearExternalInput(clearAfterTime));
    }

    private IEnumerator ClearExternalInput(float clearAfterTime)
    {
        yield return new WaitForSeconds(clearAfterTime);
        ExternalInput = Vector2.zero;
        ExternalInput_Coroutine = null;
    }

    public void SetMoveInput(Vector2 moveInput)
    {
        MoveInput = moveInput;
    }

    #region Helpers
    public Rigidbody GetRigidBody
        {
        get {
            return Rb;
        }
    }
    #endregion
}
