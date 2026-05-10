using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Transform currentSpawnPoint;
    private PlayerHealth health;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
    }

    public void Respawn()
    {
        transform.position = currentSpawnPoint.position;
        health.ResetHealth();
        
        // Opcional: Resetear velocidades del Rigidbody
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if(rb != null) rb.linearVelocity = Vector2.zero;
        
        Debug.Log("Nova ha reaparecido en el spawn point.");
    }

    public void SetNewSpawn(Transform newPoint)
    {
        currentSpawnPoint = newPoint;
    }
}