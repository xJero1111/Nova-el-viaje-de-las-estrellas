using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpawnPoint : MonoBehaviour
{
    // Este script va en un objeto con Collider2D en modo Trigger!!
    // Al entrar Nova, se registra este punto como el nuevo spawn.

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerRespawn playerRespawn = other.GetComponentInParent<PlayerRespawn>();

        if (playerRespawn == null)
        {
            return;
        }

        playerRespawn.SetNewSpawn(transform);
    }
}