using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // =========================================================
    // PLAYER PREFS KEYS
    // =========================================================

    private const string ContinueRequestKey = "NOVA_CONTINUE_REQUEST";
    private const string SavedPosXKey = "NOVA_SAVED_POS_X";
    private const string SavedPosYKey = "NOVA_SAVED_POS_Y";
    private const string SavedPosZKey = "NOVA_SAVED_POS_Z";
    private const string HasSavedPosKey = "NOVA_HAS_SAVED_POS";

    // =========================================================
    // REFERENCIAS VISUALES
    // =========================================================

    [Header("Referencias Visuales")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    // =========================================================
    // MOVEMENT
    // =========================================================

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float doubleJumpForce = 11f;
    [SerializeField] private float doubleJumpVelocityReset = 0.5f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float glideGravityScale = 0.5f;
    [SerializeField] private float doubleJumpFlashDuration = 0.12f;

    // =========================================================
    // DASH
    // =========================================================

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.6f;

    // =========================================================
    // ATTACK
    // =========================================================

    [Header("Attack")]
    [SerializeField] private CircleCollider2D spinAttackTrigger;
    [SerializeField] private float attackDuration = 0.18f;
    [SerializeField] private float attackCooldown = 0.5f;

    // =========================================================
    // SHIELD
    // =========================================================

    [Header("Shield")]
    [SerializeField] private float shieldDuration = 0.35f;
    [SerializeField] private float shieldCooldown = 1f;

    // =========================================================
    // GROUND CHECK
    // =========================================================

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    // =========================================================
    // SPAWN
    // =========================================================

    [Header("Spawn / Load")]
    [SerializeField] private Transform spawnPoint;

    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private AbilityModule abilityModule;

    // =========================================================
    // INPUT SYSTEM
    // =========================================================

    [Header("Input System")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private InputActionReference glideAction;
    [SerializeField] private InputActionReference shieldAction;

    // =========================================================
    // INPUT VARIABLES
    // =========================================================

    private Vector2 moveInput;

    private bool jumpPressedThisFrame;
    private bool dashPressedThisFrame;
    private bool attackPressedThisFrame;
    private bool shieldPressedThisFrame;

    private bool jumpHeld;

    // =========================================================
    // STATES
    // =========================================================

    private bool isGrounded;
    private bool hasDoubleJumped;

    private bool isDashing;
    private bool isAttacking;
    private bool isShielding;
    private bool isInvulnerable;

    // =========================================================
    // PHYSICS
    // =========================================================

    private float defaultGravityScale;
    private float facingDirection = 1f;

    // =========================================================
    // DASH TIMERS
    // =========================================================

    private float dashEndTime;
    private float nextDashTime;

    // =========================================================
    // ATTACK TIMERS
    // =========================================================

    private float attackEndTime;
    private float nextAttackTime;

    // =========================================================
    // SHIELD TIMERS
    // =========================================================

    private float shieldEndTime;
    private float nextShieldTime;

    // =========================================================
    // AWAKE
    // =========================================================

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

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        defaultGravityScale = rb.gravityScale;

        if (spinAttackTrigger != null)
        {
            spinAttackTrigger.enabled = false;
        }
    }

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        TryLoadSavedPositionOnStart();
    }

    // =========================================================
    // ENABLE / DISABLE INPUTS
    // =========================================================

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

    // =========================================================
    // UPDATE
    // =========================================================

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

    // =========================================================
    // FIXED UPDATE
    // =========================================================

    private void FixedUpdate()
    {
        isGrounded = IsGrounded();

        if (isGrounded)
        {
            hasDoubleJumped = false;
        }

        // =====================================================
        // DASH
        // =====================================================

        if (dashPressedThisFrame)
        {
            TryStartDash();
        }

        // =====================================================
        // ATTACK
        // =====================================================

        if (attackPressedThisFrame)
        {
            TryStartAttack();
        }

        // =====================================================
        // SHIELD
        // =====================================================

        if (shieldPressedThisFrame)
        {
            TryStartShield();
        }

        // =====================================================
        // DASH MOVEMENT
        // =====================================================

        if (isDashing)
        {
            rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);

            jumpPressedThisFrame = false;
            dashPressedThisFrame = false;
            attackPressedThisFrame = false;
            shieldPressedThisFrame = false;

            return;
        }

        // =====================================================
        // NORMAL MOVEMENT
        // =====================================================

        if (!isShielding)
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.x = moveInput.x * speed;
            rb.linearVelocity = velocity;
        }

        // =====================================================
        // FACING DIRECTION
        // =====================================================

        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            facingDirection = Mathf.Sign(moveInput.x);

            transform.localScale = new Vector3(facingDirection, 1f, 1f);
        }

        // =====================================================
        // JUMP
        // =====================================================

        if (jumpPressedThisFrame)
        {
            TryJump();
        }

        // =====================================================
        // GLIDE
        // =====================================================

        bool glideHeld = (glideAction != null && glideAction.action.IsPressed());

        bool canGlide =
            abilityModule != null &&
            abilityModule.CanGlide() &&
            !isGrounded &&
            !isDashing &&
            !isAttacking &&
            !isShielding;

        if (canGlide && glideHeld)
        {
            rb.gravityScale = glideGravityScale;

            if (abilityModule != null)
            {
                abilityModule.SetState(AbilityModule.CompanionState.Glide);
            }

            if (animator != null)
            {
                animator.SetBool("isGliding", true);
            }
        }
        else
        {
            rb.gravityScale = defaultGravityScale;

            if (animator != null)
            {
                animator.SetBool("isGliding", false);
            }

            if (
                abilityModule != null &&
                abilityModule.CurrentState == AbilityModule.CompanionState.Glide
            )
            {
                abilityModule.ResetState();
            }
        }

        // =====================================================
        // RESET INPUT FLAGS
        // =====================================================

        jumpPressedThisFrame = false;
        dashPressedThisFrame = false;
        attackPressedThisFrame = false;
        shieldPressedThisFrame = false;

        // =====================================================
        // ANIMATOR
        // =====================================================

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
        }
    }

    // =========================================================
    // INPUT
    // =========================================================

    private void ReadInput()
    {
        moveInput = moveAction != null
            ? moveAction.action.ReadValue<Vector2>()
            : Vector2.zero;

        jumpHeld = jumpAction != null && jumpAction.action.IsPressed();
    }

    // =========================================================
    // TIMERS
    // =========================================================

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

    // =========================================================
    // JUMP
    // =========================================================

    private void TryJump()
    {
        if (isDashing || isShielding)
        {
            return;
        }

        // SALTO NORMAL
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            return;
        }

        // DOBLE SALTO
        if (
            abilityModule != null &&
            abilityModule.CanDoubleJump() &&
            !hasDoubleJumped
        )
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                doubleJumpVelocityReset
            );

            rb.AddForce(
                Vector2.up * doubleJumpForce,
                ForceMode2D.Impulse
            );

            hasDoubleJumped = true;

            abilityModule.FlashState(
                AbilityModule.CompanionState.DoubleJump,
                doubleJumpFlashDuration
            );

            if (animator != null)
            {
                animator.SetTrigger("DoubleJump");
            }
        }
    }

    // =========================================================
    // DASH
    // =========================================================

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

        abilityModule.SetState(AbilityModule.CompanionState.Dash);

        if (animator != null)
        {
            animator.SetBool("isDashing", true);
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

        if (animator != null)
        {
            animator.SetBool("isDashing", false);
        }
    }

    // =========================================================
    // ATTACK
    // =========================================================

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

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        abilityModule.SetState(AbilityModule.CompanionState.Attack);
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

    // =========================================================
    // SHIELD
    // =========================================================

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

        abilityModule.SetState(AbilityModule.CompanionState.Shield);
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

    // =========================================================
    // GROUND CHECK
    // =========================================================

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        ) != null;
    }

    // =========================================================
    // INPUT HELPERS
    // =========================================================

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

    // =========================================================
    // GIZMOS
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }

    // =========================================================
    // INVULNERABILITY
    // =========================================================

    public bool IsInvulnerable => isInvulnerable;

    // =========================================================
    // SAVE POSITION
    // =========================================================

    public void SaveCurrentPosition()
    {
        Vector3 pos = transform.position;

        PlayerPrefs.SetFloat(SavedPosXKey, pos.x);
        PlayerPrefs.SetFloat(SavedPosYKey, pos.y);
        PlayerPrefs.SetFloat(SavedPosZKey, pos.z);

        PlayerPrefs.SetInt(HasSavedPosKey, 1);

        PlayerPrefs.Save();
    }

    // =========================================================
    // LOAD POSITION
    // =========================================================

    public bool LoadSavedPosition()
    {
        if (PlayerPrefs.GetInt(HasSavedPosKey, 0) != 1)
        {
            return false;
        }

        float x = PlayerPrefs.GetFloat(SavedPosXKey, 0f);
        float y = PlayerPrefs.GetFloat(SavedPosYKey, 0f);
        float z = PlayerPrefs.GetFloat(SavedPosZKey, 0f);

        transform.position = new Vector3(x, y, z);

        return true;
    }

    // =========================================================
    // LOAD ON START
    // =========================================================

    private void TryLoadSavedPositionOnStart()
    {
        bool continueRequested =
            PlayerPrefs.GetInt(ContinueRequestKey, 0) == 1;

        if (continueRequested && LoadSavedPosition())
        {
            PlayerPrefs.SetInt(ContinueRequestKey, 0);
            PlayerPrefs.Save();

            return;
        }

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
        }

        PlayerPrefs.SetInt(ContinueRequestKey, 0);
        PlayerPrefs.Save();
    }

    // =========================================================
    // DAMAGE / ENEMY COLLISION
    // =========================================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (
            collision.gameObject.CompareTag("Enemy") &&
            !isInvulnerable
        )
        {
            GetComponent<PlayerKnockback>()
                .ApplyKnockback(collision.transform);

            GetComponent<PlayerHealth>()
                .TakeDamage(1);

            Debug.Log("Nova recibió daño.");
        }
    }
}
