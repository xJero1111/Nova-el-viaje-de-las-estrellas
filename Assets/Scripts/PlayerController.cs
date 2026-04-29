using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float doubleJumpForce = 11f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float glideGravityScale = 0.5f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.6f;

    [Header("Attack")]
    [SerializeField] private CircleCollider2D spinAttackTrigger;
    [SerializeField] private float attackDuration = 0.18f;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Shield")]
    [SerializeField] private float shieldDuration = 0.35f;
    [SerializeField] private float shieldCooldown = 1.0f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private AbilityModule abilityModule;

    [Header("Input System")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private InputActionReference glideAction;
    [SerializeField] private InputActionReference shieldAction;

    private Vector2 moveInput;
    private bool jumpPressedThisFrame;
    private bool dashPressedThisFrame;
    private bool attackPressedThisFrame;
    private bool shieldPressedThisFrame;
    private bool jumpHeld;

    private bool isGrounded;
    private bool hasDoubleJumped;
    private bool isDashing;
    private bool isAttacking;
    private bool isShielding;
    private bool isInvulnerable;

    private float defaultGravityScale;
    private float facingDirection = 1f;

    private float dashEndTime;
    private float nextDashTime;

    private float attackEndTime;
    private float nextAttackTime;

    private float shieldEndTime;
    private float nextShieldTime;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (abilityModule == null)
        {
            abilityModule = FindObjectOfType<AbilityModule>();
        }

        defaultGravityScale = rb.gravityScale;

        if (spinAttackTrigger != null)
        {
            spinAttackTrigger.enabled = false;
        }
    }

    private void OnEnable()
    {
        EnableAction(moveAction);
        EnableAction(jumpAction);
        EnableAction(dashAction);
        EnableAction(attackAction);
        EnableAction(glideAction);
        EnableAction(shieldAction);
    }

    private void OnDisable()
    {
        DisableAction(moveAction);
        DisableAction(jumpAction);
        DisableAction(dashAction);
        DisableAction(attackAction);
        DisableAction(glideAction);
        DisableAction(shieldAction);
    }

    private void Update()
    {
        ReadInput();

        if (jumpAction != null && jumpAction.action.WasPressedThisFrame())
        {
            jumpPressedThisFrame = true;
        }

        if (dashAction != null && dashAction.action.WasPressedThisFrame())
        {
            dashPressedThisFrame = true;
        }

        if (attackAction != null && attackAction.action.WasPressedThisFrame())
        {
            attackPressedThisFrame = true;
        }

        if (shieldAction != null && shieldAction.action.WasPressedThisFrame())
        {
            shieldPressedThisFrame = true;
        }

        HandleTimers();
    }

    private void FixedUpdate()
    {
        isGrounded = IsGrounded();

        if (isGrounded)
        {
            hasDoubleJumped = false;
        }

        if (dashPressedThisFrame)
        {
            TryStartDash();
        }

        if (attackPressedThisFrame)
        {
            TryStartAttack();
        }

        if (shieldPressedThisFrame)
        {
            TryStartShield();
        }

        if (isDashing)
        {
            rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);
            jumpPressedThisFrame = false;
            dashPressedThisFrame = false;
            attackPressedThisFrame = false;
            shieldPressedThisFrame = false;
            return;
        }

        // Movimiento horizontal base
        if (!isShielding)
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.x = moveInput.x * speed;
            rb.linearVelocity = velocity;
        }

        // Recordar hacia dónde mira Nova
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            facingDirection = Mathf.Sign(moveInput.x);
        }

        // Salto normal o doble salto
        if (jumpPressedThisFrame)
        {
            TryJump();
        }

        // Planeo: si está en el aire y mantiene salto o Glide, la gravedad baja
        bool glideHeld = (jumpHeld || (glideAction != null && glideAction.action.IsPressed()));
        bool canGlide = !isGrounded && !isDashing && !isShielding && abilityModule != null && abilityModule.CanGlide();

        rb.gravityScale = (canGlide && glideHeld) ? glideGravityScale : defaultGravityScale;

        if (canGlide && glideHeld)
        {
            abilityModule.SetState(AbilityModule.CompanionState.Glide);
        }
        else if (abilityModule != null && abilityModule.CurrentState == AbilityModule.CompanionState.Glide)
        {
            abilityModule.ResetState();
        }

        jumpPressedThisFrame = false;
        dashPressedThisFrame = false;
        attackPressedThisFrame = false;
        shieldPressedThisFrame = false;
    }

    private void ReadInput()
    {
        moveInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        jumpHeld = jumpAction != null && jumpAction.action.IsPressed();
    }

    private void HandleTimers()
    {
        if (isDashing && Time.time >= dashEndTime)
        {
            EndDash();
        }

        if (isAttacking && Time.time >= attackEndTime)
        {
            EndAttack();
        }

        if (isShielding && Time.time >= shieldEndTime)
        {
            EndShield();
        }
    }

    private void TryJump()
    {
        if (isDashing || isShielding)
        {
            return;
        }

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            return;
        }

        if (abilityModule != null && abilityModule.CanDoubleJump() && !hasDoubleJumped)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
            hasDoubleJumped = true;
        }
    }

    private void TryStartDash()
    {
        if (abilityModule == null || !abilityModule.CanDash())
        {
            return;
        }

        if (Time.time < nextDashTime || isDashing || isShielding)
        {
            return;
        }

        isDashing = true;
        dashEndTime = Time.time + dashDuration;
        nextDashTime = Time.time + dashCooldown;

        if (abilityModule != null)
        {
            abilityModule.SetState(AbilityModule.CompanionState.Dash);
        }
    }

    private void EndDash()
    {
        isDashing = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (abilityModule != null)
        {
            abilityModule.ResetState();
        }
    }

    private void TryStartAttack()
    {
        if (abilityModule == null || !abilityModule.CanAttack())
        {
            return;
        }

        if (Time.time < nextAttackTime || isAttacking)
        {
            return;
        }

        isAttacking = true;
        attackEndTime = Time.time + attackDuration;
        nextAttackTime = Time.time + attackCooldown;

        if (spinAttackTrigger != null)
        {
            spinAttackTrigger.enabled = true;
        }

        if (abilityModule != null)
        {
            abilityModule.SetState(AbilityModule.CompanionState.Attack);
        }
    }

    private void EndAttack()
    {
        isAttacking = false;

        if (spinAttackTrigger != null)
        {
            spinAttackTrigger.enabled = false;
        }

        if (abilityModule != null)
        {
            abilityModule.ResetState();
        }
    }

    private void TryStartShield()
    {
        if (abilityModule == null || !abilityModule.CanShield())
        {
            return;
        }

        if (Time.time < nextShieldTime || isShielding)
        {
            return;
        }

        isShielding = true;
        isInvulnerable = true;
        shieldEndTime = Time.time + shieldDuration;
        nextShieldTime = Time.time + shieldCooldown;

        if (abilityModule != null)
        {
            abilityModule.SetState(AbilityModule.CompanionState.Shield);
        }
    }

    private void EndShield()
    {
        isShielding = false;
        isInvulnerable = false;

        if (abilityModule != null)
        {
            abilityModule.ResetState();
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) != null;
    }

    private void EnableAction(InputActionReference actionReference)
    {
        if (actionReference != null && actionReference.action != null)
        {
            actionReference.action.Enable();
        }
    }

    private void DisableAction(InputActionReference actionReference)
    {
        if (actionReference != null && actionReference.action != null)
        {
            actionReference.action.Disable();
        }
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

    public bool IsInvulnerable => isInvulnerable;
}