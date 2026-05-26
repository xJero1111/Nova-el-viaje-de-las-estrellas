using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class GameEndingTrigger : MonoBehaviour
{
    // =========================================================
    // CONFIGURACIÓN
    // =========================================================

    [Header("UI Final")]
    [SerializeField] private GameObject uiRoot;
    [SerializeField] private Button returnToMenuButton;

    [Header("Scenes")]
    [SerializeField] private string menuSceneName = "UI";

    // =========================================================
    // ESTADO INTERNO
    // =========================================================

    private PlayerController cachedPlayerController;
    private Rigidbody2D cachedRigidbody;
    private Collider2D cachedCollider;

    private bool isEnding;
    private bool controllerWasEnabled;
    private RigidbodyConstraints2D originalConstraints;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        cachedCollider = GetComponent<Collider2D>();

        if (cachedCollider != null)
        {
            cachedCollider.isTrigger = true;
        }

        if (uiRoot != null)
        {
            uiRoot.SetActive(false);
        }

        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.RemoveListener(ReturnToMenu);
            returnToMenuButton.onClick.AddListener(ReturnToMenu);
        }
    }

    // =========================================================
    // TRIGGER
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnding)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        cachedPlayerController = other.GetComponent<PlayerController>();
        cachedRigidbody = other.GetComponent<Rigidbody2D>();

        if (cachedPlayerController == null)
        {
            return;
        }

        BeginEndingSequence();
    }

    // =========================================================
    // INICIO DE LA ESCENA FINAL
    // =========================================================

    private void BeginEndingSequence()
    {
        isEnding = true;

        controllerWasEnabled = cachedPlayerController.enabled;
        originalConstraints = cachedRigidbody != null
            ? cachedRigidbody.constraints
            : RigidbodyConstraints2D.None;

        // Congelamos a Nova por completo.
        if (cachedPlayerController != null)
        {
            cachedPlayerController.enabled = false;
        }

        if (cachedRigidbody != null)
        {
            cachedRigidbody.linearVelocity = Vector2.zero;
            cachedRigidbody.angularVelocity = 0f;
            cachedRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // Mostramos la pantalla de victoria / despedida.
        if (uiRoot != null)
        {
            uiRoot.SetActive(true);
        }
    }

    // =========================================================
    // BOTÓN: VOLVER AL MENÚ
    // =========================================================

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    // =========================================================
    // LIMPIEZA
    // =========================================================

    private void OnDestroy()
    {
        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.RemoveListener(ReturnToMenu);
        }
    }
}