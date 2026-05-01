using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AbilityUnlocker : MonoBehaviour
{
    [Header("Unlock Settings")]
    [SerializeField] private AbilityModule.AbilityType abilityToUnlock = AbilityModule.AbilityType.Dash;

    [Header("Feedback")]
    [SerializeField] private GameObject collectVfxPrefab;
    [SerializeField] private bool destroyOnCollect = true;
    [SerializeField] private float destroyDelay = 0.05f;

    [Header("References")]
    [SerializeField] private AbilityModule abilityModule;

    private bool collected;

    private void Awake()
    {
        Collider2D trigger = GetComponent<Collider2D>();
        trigger.isTrigger = true;

        if (abilityModule == null)
        {
            abilityModule = FindObjectOfType<AbilityModule>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (abilityModule == null)
        {
            abilityModule = FindObjectOfType<AbilityModule>();
        }

        if (abilityModule == null)
        {
            return;
        }

        collected = true;
        abilityModule.UnlockAbility(abilityToUnlock);

        if (collectVfxPrefab != null)
        {
            Instantiate(collectVfxPrefab, transform.position, Quaternion.identity);
        }

        if (destroyOnCollect)
        {
            Destroy(gameObject, destroyDelay);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}