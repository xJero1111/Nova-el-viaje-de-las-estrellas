using System.Collections;
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

    private const string DashKey = "NOVA_ABILITY_DASH";
    private const string DoubleJumpKey = "NOVA_ABILITY_DOUBLE_JUMP";
    private const string AttackKey = "NOVA_ABILITY_ATTACK";
    private const string ShieldKey = "NOVA_ABILITY_SHIELD";
    private const string GlideKey = "NOVA_ABILITY_GLIDE";

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

    private static bool dashUnlocked;
    private static bool doubleJumpUnlocked;
    private static bool attackUnlocked;
    private static bool shieldUnlocked;
    private static bool glideUnlocked;

    private Coroutine flashRoutine;

    public CompanionState CurrentState => currentState;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        LoadAbilitiesFromPrefs();
        ApplyStateColor();
    }

    public void UnlockAbility(AbilityType type)
    {
        switch (type)
        {
            case AbilityType.Dash:
                dashUnlocked = true;
                PlayerPrefs.SetInt(DashKey, 1);
                break;

            case AbilityType.DoubleJump:
                doubleJumpUnlocked = true;
                PlayerPrefs.SetInt(DoubleJumpKey, 1);
                break;

            case AbilityType.Attack:
                attackUnlocked = true;
                PlayerPrefs.SetInt(AttackKey, 1);
                break;

            case AbilityType.Shield:
                shieldUnlocked = true;
                PlayerPrefs.SetInt(ShieldKey, 1);
                break;

            case AbilityType.Glide:
                glideUnlocked = true;
                PlayerPrefs.SetInt(GlideKey, 1);
                break;
        }

        PlayerPrefs.Save();
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

    public bool CanDash()
    {
        return IsAbilityUnlocked(AbilityType.Dash);
    }

    public bool CanDoubleJump()
    {
        return IsAbilityUnlocked(AbilityType.DoubleJump);
    }

    public bool CanAttack()
    {
        return IsAbilityUnlocked(AbilityType.Attack);
    }

    public bool CanShield()
    {
        return IsAbilityUnlocked(AbilityType.Shield);
    }

    public bool CanGlide()
    {
        return IsAbilityUnlocked(AbilityType.Glide);
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

    public void FlashState(CompanionState state, float duration)
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashStateRoutine(state, duration));
    }

    private IEnumerator FlashStateRoutine(CompanionState state, float duration)
    {
        SetState(state);
        yield return new WaitForSeconds(duration);

        if (currentState == state)
        {
            ResetState();
        }

        flashRoutine = null;
    }

    private void LoadAbilitiesFromPrefs()
    {
        dashUnlocked = PlayerPrefs.GetInt(DashKey, 0) == 1;
        doubleJumpUnlocked = PlayerPrefs.GetInt(DoubleJumpKey, 0) == 1;
        attackUnlocked = PlayerPrefs.GetInt(AttackKey, 0) == 1;
        shieldUnlocked = PlayerPrefs.GetInt(ShieldKey, 0) == 1;
        glideUnlocked = PlayerPrefs.GetInt(GlideKey, 0) == 1;
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