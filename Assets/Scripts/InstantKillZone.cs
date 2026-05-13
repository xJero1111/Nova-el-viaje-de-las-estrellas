using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InstantKillZone : MonoBehaviour
{
    // Este script debe ir en un objeto con Collider2D
    // configurado como Trigger.
    //
    // Cuando Nova entra en contacto con esta zona OJITO DONDE SE PONE
    // se busca PlayerHealth y se fuerza una muerte instantánea osea F en el chat, chat.

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Busca PlayerHealth en el objeto o en sus padres.
        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

        // Si el objeto que entró no es Nova, salimos.
        if (playerHealth == null)
        {
            return;
        }

        // Mata inmediatamente al jugador.
        playerHealth.InstantDie();
    }
}