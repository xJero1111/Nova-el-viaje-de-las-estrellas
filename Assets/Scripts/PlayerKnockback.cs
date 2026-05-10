using UnityEngine;
using System.Collections;

public class PlayerKnockback : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.2f;
    
    private Rigidbody2D rb;
    private PlayerController playerController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
    }

    public void ApplyKnockback(Transform damageSource)
    {
        StartCoroutine(KnockbackRoutine(damageSource));
    }

    private IEnumerator KnockbackRoutine(Transform damageSource)
    {
        // Bloqueamos el input del jugador un momento
        playerController.enabled = false;

        // Calculamos dirección (opuesta a la fuente del daño)
        Vector2 direction = (transform.position - damageSource.position).normalized;
        
        // Aplicamos fuerza (hacia atrás y un poco hacia arriba)
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(direction.x, 0.5f) * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        playerController.enabled = true;
    }
}