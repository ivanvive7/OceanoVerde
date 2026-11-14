using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerControllerV2 : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float jumpSpeed = 12f;
    [SerializeField] float jumpBufferTime = 0.1f;

    [Header("Swimming")]
    [SerializeField] float waterLevel = 0f;                 
    [SerializeField] float swimGravityMultiplier = 0.5f;
    [SerializeField] float swimJumpMultiplier = 0.5f;

    Rigidbody2D rb;
    Animator animator;
    InputAction moveAction, jumpAction;

    float moveInput;
    float jumpBufferCounter;
    float defaultGravity;
    bool isGrounded;
    bool isSwimming;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        defaultGravity = rb.gravityScale;

        // Zero friction = no wall sticking
        GetComponent<Collider2D>().sharedMaterial = new PhysicsMaterial2D { friction = 0f, bounciness = 0f };
    }

    void Start()
    {
        moveAction = InputSystem.actions["Move"];
        jumpAction = InputSystem.actions["Jump"];
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>().x;

        if (jumpAction.WasPressedThisFrame())
            jumpBufferCounter = jumpBufferTime;

        jumpBufferCounter -= Time.deltaTime;

        if (animator != null)
        {
            animator.SetBool("OnAir", !isGrounded && !isSwimming);
            animator.SetBool("Swimming", isSwimming);
        }
    }

    void FixedUpdate()
    {
        UpdateSwimState();

        float vy = rb.linearVelocity.y;

        if (jumpBufferCounter > 0f && (isGrounded || isSwimming))
        {
            if (isSwimming)
            {
                vy = jumpSpeed * swimJumpMultiplier;
            }
            else
            {
                vy = jumpSpeed;
                if (animator != null) animator.SetBool("Jumped", true);
            }

            jumpBufferCounter = 0f;
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, vy);

        // Reset; OnCollisionStay2D sets it true again if still on the ground
        isGrounded = false;
    }

    void UpdateSwimState()
    {
        bool shouldSwim = transform.position.y < waterLevel;
        if (shouldSwim == isSwimming) return;

        isSwimming = shouldSwim;
        rb.gravityScale = isSwimming ? defaultGravity * swimGravityMultiplier : defaultGravity;

        if (isSwimming && animator != null)
            animator.SetBool("Jumped", false);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                if (animator != null && rb.linearVelocity.y <= 0.1f)
                    animator.SetBool("Jumped", false);
                return;
            }
        }
    }


}