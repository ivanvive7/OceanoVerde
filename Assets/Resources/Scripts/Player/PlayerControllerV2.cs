using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerV2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float moveSpeed = 8f;

    Vector2 input;

    private Rigidbody2D rb;

    private Animator animator;

    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                animator.SetBool("OnAir", false);
                animator.SetBool("Jumped", false);
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
        animator.SetBool("OnAir", true);

    }
    // Update is called once per frame
    void Update()
    {
        input = InputSystem.actions["Move"].ReadValue<Vector2>();
        Debug.Log("player is grounded:" + isGrounded);

        if (InputSystem.actions["Jump"].WasPressedThisFrame() && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.SetBool("Jumped", true);

        }
    }
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(input.x * moveSpeed, rb.linearVelocity.y);
    }
}
