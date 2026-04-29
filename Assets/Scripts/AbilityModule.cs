using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AbilityModule : MonoBehaviour
{
    public enum AbilityType
    {
        Dash,
        DoubleJump,
        Attack,
        Shield,
        Glide
    }

    public enum CompanionState
    {
        Normal,
        Dash,
        DoubleJump,
        Attack,
        Shield,
        Glide
    }

    [Header("State Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color dashColor = new Color(0.35f, 0.75f, 1f);
    [SerializeField] private Color doubleJumpColor = new Color(0.65f, 0.45f, 1f);
    [SerializeField] private Color attackColor = new Color(1f, 0.55f, 0.25f);
    [SerializeField] private Color shieldColor = new Color(0.35f, 1f, 0.8f);
    [SerializeField] private Color glideColor = new Color(1f, 1f, 0.6f);

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Runtime State")]
    [SerializeField] private CompanionState currentState = CompanionState.Normal;

    // Persistencia simple por sesión
    private static bool dashUnlocked;
    private static bool doubleJumpUnlocked;
    private static bool attackUnlocked;
    private static bool shieldUnlocked;
    private static bool glideUnlocked;

    public CompanionState CurrentState => currentState;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        ApplyStateColor();
    }

    public void UnlockAbility(AbilityType type)
    {
        switch (type)
        {
            case AbilityType.Dash:
                dashUnlocked = true;
                break;

            case AbilityType.DoubleJump:
                doubleJumpUnlocked = true;
                break;

            case AbilityType.Attack:
                attackUnlocked = true;
                break;

            case AbilityType.Shield:
                shieldUnlocked = true;
                break;

            case AbilityType.Glide:
                glideUnlocked = true;
                break;
        }
    }

    public bool IsAbilityUnlocked(AbilityType type)
    {
        switch (type)
        {
            case AbilityType.Dash:
                return dashUnlocked;

            case AbilityType.DoubleJump:
                return doubleJumpUnlocked;

            case AbilityType.Attack:
                return attackUnlocked;

            case AbilityType.Shield:
                return shieldUnlocked;

            case AbilityType.Glide:
                return glideUnlocked;

            default:
                return false;
        }
    }

    // "¿Puedo hacer esto ahora?"
    // Normal = estado neutral. Si el brillo entra en un estado activo,
    // sólo ese estado específico quedará permitido.
    public bool CanUseAbility(AbilityType type)
    {
        if (!IsAbilityUnlocked(type))
        {
            return false;
        }

        if (currentState == CompanionState.Normal)
        {
            return true;
        }

        return currentState == GetStateFromAbility(type);
    }

    public bool CanDash()
    {
        return CanUseAbility(AbilityType.Dash);
    }

    public bool CanDoubleJump()
    {
        return CanUseAbility(AbilityType.DoubleJump);
    }

    public bool CanAttack()
    {
        return CanUseAbility(AbilityType.Attack);
    }

    public bool CanShield()
    {
        return CanUseAbility(AbilityType.Shield);
    }

    public bool CanGlide()
    {
        return CanUseAbility(AbilityType.Glide);
    }

    public void SetState(CompanionState newState)
    {
        currentState = newState;
        ApplyStateColor();
    }

    public void ResetState()
    {
        SetState(CompanionState.Normal);
    }

    private CompanionState GetStateFromAbility(AbilityType type)
    {
        switch (type)
        {
            case AbilityType.Dash:
                return CompanionState.Dash;

            case AbilityType.DoubleJump:
                return CompanionState.DoubleJump;

            case AbilityType.Attack:
                return CompanionState.Attack;

            case AbilityType.Shield:
                return CompanionState.Shield;

            case AbilityType.Glide:
                return CompanionState.Glide;

            default:
                return CompanionState.Normal;
        }
    }

    private void ApplyStateColor()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        switch (currentState)
        {
            case CompanionState.Dash:
                spriteRenderer.color = dashColor;
                break;

            case CompanionState.DoubleJump:
                spriteRenderer.color = doubleJumpColor;
                break;

            case CompanionState.Attack:
                spriteRenderer.color = attackColor;
                break;

            case CompanionState.Shield:
                spriteRenderer.color = shieldColor;
                break;

            case CompanionState.Glide:
                spriteRenderer.color = glideColor;
                break;

            default:
                spriteRenderer.color = normalColor;
                break;
        }
    }
}