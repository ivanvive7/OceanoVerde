using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movimiento 2D del personaje con dos modos automáticos:
///  - ANDAR: cuando toca el suelo (también en las plataformas dentro del agua).
///           Solo puede saltar si está tocando el suelo.
///  - NADAR: cuando está dentro del agua y NO toca el suelo.
///           Se mueve libremente en las 4 direcciones.
/// Funciona con teclado (editor) y con botones táctiles (Android).
/// Parámetros del Animator: "Speed" (float), "IsJumping" (bool), "IsSwimming" (bool).
/// El agua se detecta con un Collider2D Trigger que tenga el tag o la layer "Water".
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Andar")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    [Header("Nadar")]
    public float swimSpeed = 3.5f;       // velocidad horizontal nadando
    public float swimUpSpeed = 3f;       // velocidad al subir / bajar nadando
    public float sinkSpeed = 1f;         // se hunde despacio si no pulsas nada (0 = se queda quieto)
    public float waterExitBoost = 6f;    // impulso al salir del agua hacia arriba
    public float swimStrokeDuration = 0.35f;

    [Header("Detección de suelo")]
    public Transform groundCheck;
    public float groundRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;

    private float keyboardInput;
    private float mobileInput;
    private float mobileVertical;
    private bool jumpRequested;
    private bool jumpHeld;

    private bool isGrounded;
    private bool inWater;
    private bool isSwimming;
    private float swimStrokeTimer;
    private float defaultGravity;
    private readonly HashSet<Collider2D> waterColliders = new HashSet<Collider2D>();

    // Compatibilidad: Unity 6 usa linearVelocity, versiones anteriores usan velocity
    private Vector2 Velocity
    {
#if UNITY_6000_0_OR_NEWER
        get { return rb.linearVelocity; }
        set { rb.linearVelocity = value; }
#else
        get { return rb.velocity; }
        set { rb.velocity = value; }
#endif
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        defaultGravity = rb.gravityScale;
    }

    private void Update()
    {
        // 1) Entrada de teclado (pruebas en PC)
        keyboardInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")) jumpRequested = true;
        jumpHeld = Input.GetButton("Jump");

        // 2) Suelo
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        }
        else
        {
            Debug.LogWarning("PlayerController: falta asignar el objeto GroundCheck.");
        }

        isSwimming = inWater;

        // 4) Girar sprite
        float direction = GetHorizontalInput();
        if (direction > 0.01f) Flip(1);
        else if (direction < -0.01f) Flip(-1);

        // 5) Animator
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        // Dentro del agua siempre se nada, incluso si se toca el fondo.
        rb.gravityScale = inWater ? 0f : defaultGravity;

        if (inWater)
        {
            isSwimming = true;
            if (jumpRequested) swimStrokeTimer = swimStrokeDuration;
            Swim();
        }
        else
        {
            isSwimming = false;
            Walk();
        }

        jumpRequested = false;
    }

    // ---------- Modos de movimiento ----------

    private void Walk()
    {
        Velocity = new Vector2(GetHorizontalInput() * moveSpeed, Velocity.y);

        // Fuera del agua solo se puede saltar tocando el suelo.
        if (jumpRequested && isGrounded)
        {
            Velocity = new Vector2(Velocity.x, jumpForce);
        }
    }

    private void Swim()
    {
        float vertical = GetVerticalInput();
        float vy;

        if (jumpHeld || swimStrokeTimer > 0f || vertical > 0.1f)
        {
            vy = swimUpSpeed;
            swimStrokeTimer = Mathf.Max(0f, swimStrokeTimer - Time.fixedDeltaTime);
        }
        else if (vertical < -0.1f) vy = -swimUpSpeed;    // bajar
        else                       vy = -sinkSpeed;      // hundirse despacio

        Velocity = new Vector2(GetHorizontalInput() * swimSpeed, vy);
    }

    // ---------- Detección de agua ----------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsWaterCollider(other) && waterColliders.Add(other))
        {
            inWater = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!waterColliders.Remove(other) || waterColliders.Count > 0) return;

        // Pequeño impulso si sale del agua nadando hacia arriba (para subirse a tierra)
        if (isSwimming && Velocity.y > 0f)
        {
            Velocity = new Vector2(Velocity.x, waterExitBoost);
        }

        inWater = false;
    }

    private bool IsWaterCollider(Collider2D other)
    {
        int waterLayer = LayerMask.NameToLayer("Water");
        Transform waterRoot = other.transform.root;

        return other.CompareTag("Water")
            || other.gameObject.layer == waterLayer
            || waterRoot.CompareTag("Water")
            || waterRoot.gameObject.layer == waterLayer;
    }

    // ---------- Entrada ----------

    private float GetHorizontalInput()
    {
        return Mathf.Abs(mobileInput) > 0.01f ? mobileInput : keyboardInput;
    }

    private float GetVerticalInput()
    {
        return mobileVertical;
    }

    /// <summary>Botones táctiles izquierda/derecha (-1, 0, 1).</summary>
    public void SetMobileInput(float value) { mobileInput = value; }

    /// <summary>Botones táctiles arriba/abajo para nadar (1, -1, 0).</summary>
    public void SetMobileVertical(float value) { mobileVertical = value; }

    /// <summary>Salta en tierra o da una brazada ascendente dentro del agua.</summary>
    public void Jump() { jumpRequested = true; }

    // ---------- Visual y animación ----------

    private void Flip(int dir)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        transform.localScale = scale;
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetFloat("Speed", Mathf.Abs(Velocity.x));
        animator.SetBool("IsSwimming", isSwimming);
        animator.SetBool("IsJumping", !isGrounded && !isSwimming);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}