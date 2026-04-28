using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f; // velocidad horizontal de Nova
    [SerializeField] private float jumpForce = 12f; // fuerza del salto
    [SerializeField] private float groundCheckRadius = 0.2f; // radio para detectar el suelo

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck; // punto ubicado en los pies de Nova
    [SerializeField] private LayerMask groundLayer; // capa(s) que cuentan como suelo

    [Header("References")]
    [SerializeField] private Rigidbody2D rb; // referencia al Rigidbody2D

    private float moveInput;
    private bool jumpRequested;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        // Leer input horizontal: A/D, flechas o joystick
        moveInput = Input.GetAxisRaw("Horizontal");

        // Guardar el salto para procesarlo en FixedUpdate
        if (Input.GetButtonDown("Jump"))
        {
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        // Movimiento horizontal suave y estable
        Vector2 velocity = rb.linearVelocity;
        velocity.x = moveInput * speed;
        rb.linearVelocity = velocity;

        // Ejecutar salto solo si hay solicitud y Nova está en el suelo
        if (jumpRequested && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        jumpRequested = false;
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}