using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private const string ContinueRequestKey = "NOVA_CONTINUE_REQUEST";
    private const string SavedSpawnXKey = "NOVA_SAVED_SPAWN_X";
    private const string SavedSpawnYKey = "NOVA_SAVED_SPAWN_Y";
    private const string SavedSpawnZKey = "NOVA_SAVED_SPAWN_Z";
    private const string HasSavedSpawnKey = "NOVA_HAS_SAVED_SPAWN";

    [SerializeField] private Transform currentSpawnPoint;
    private PlayerHealth health;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        // Si el jugador eligió "Continuar", intentamos cargar el último checkpoint guardado.
        bool continueRequested = PlayerPrefs.GetInt(ContinueRequestKey, 0) == 1;
        bool hasSavedSpawn = PlayerPrefs.GetInt(HasSavedSpawnKey, 0) == 1;

        if (continueRequested && hasSavedSpawn)
        {
            float x = PlayerPrefs.GetFloat(SavedSpawnXKey, transform.position.x);
            float y = PlayerPrefs.GetFloat(SavedSpawnYKey, transform.position.y);
            float z = PlayerPrefs.GetFloat(SavedSpawnZKey, transform.position.z);

            Vector3 savedPosition = new Vector3(x, y, z);
            transform.position = savedPosition;

            // Mantenemos actualizado el spawn actual para que Respawn() use este punto.
            if (currentSpawnPoint != null)
            {
                currentSpawnPoint.position = savedPosition;
            }
        }

        // Consumimos la solicitud de continuar para que sea solo de entrada.
        PlayerPrefs.SetInt(ContinueRequestKey, 0);
        PlayerPrefs.Save();
    }

    public void Respawn()
    {
        if (currentSpawnPoint != null)
        {
            transform.position = currentSpawnPoint.position;
        }

        health.ResetHealth();

        // Opcional: Resetear velocidades del Rigidbody
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Debug.Log("Nova ha reaparecido en el spawn point.");
    }

    public void SetNewSpawn(Transform newPoint)
    {
        if (newPoint == null)
        {
            return;
        }

        currentSpawnPoint = newPoint;

        // Guardamos la posición del nuevo checkpoint.
        Vector3 pos = newPoint.position;
        PlayerPrefs.SetFloat(SavedSpawnXKey, pos.x);
        PlayerPrefs.SetFloat(SavedSpawnYKey, pos.y);
        PlayerPrefs.SetFloat(SavedSpawnZKey, pos.z);
        PlayerPrefs.SetInt(HasSavedSpawnKey, 1);
        PlayerPrefs.Save();
    }
}