using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Salud")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Invulnerabilidad")]
    [SerializeField] private float iFramesDuration = 1.5f;
    [SerializeField] private int flashCount = 6;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool isInvulnerable;

    [Header("Eventos")]
    public UnityEvent<int> onHealthChanged; // Conéctalo a la UI
    public UnityEvent onDeath;             // Conéctalo al Respawn

    private void Start()
    {
        currentHealth = maxHealth;
        onHealthChanged.Invoke(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        // Si tiene escudo o es invulnerable, ignoramos
        PlayerController pc = GetComponent<PlayerController>();
        if (isInvulnerable || (pc != null && pc.IsInvulnerable)) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        onHealthChanged.Invoke(currentHealth);

        if (currentHealth > 0)
        {
            StartCoroutine(InvulnerabilityRoutine());
        }
        else
        {
            Die();
        }
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.2f);
            yield return new WaitForSeconds(iFramesDuration / (flashCount * 2));
            spriteRenderer.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(iFramesDuration / (flashCount * 2));
        }
        isInvulnerable = false;
    }

    private void Die()
    {
        onDeath.Invoke();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        onHealthChanged.Invoke(currentHealth);
    }

    public void InstantDie()
    {
    // Ignora invulnerabilidad, escudo y salud actual.
    currentHealth = 0;

    // Actualiza la UI de vida.
    onHealthChanged.Invoke(currentHealth);

    // Ejecuta el sistema normal de muerte.
    Die();
    }
    
}