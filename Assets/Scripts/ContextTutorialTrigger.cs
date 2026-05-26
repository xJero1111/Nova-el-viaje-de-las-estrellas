using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class ContextTutorialTrigger : MonoBehaviour
{
    // =========================================================
    // CONFIGURACIÓN DEL TUTORIAL
    // =========================================================

    [Header("Identidad del Tutorial")]
    [SerializeField] private string tutorialId = "tutorial_01";

    [TextArea(3, 8)]
    [SerializeField] private string message = "Presiona una tecla para continuar.";

    [SerializeField] private string continueHint = "Toca cualquier tecla para continuar";

    [Header("UI del Tutorial")]
    [SerializeField] private GameObject uiRoot;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text continueHintText;
    [SerializeField] private Image speakerImage;

    [Header("Persistencia")]
    [SerializeField] private bool activateOnlyOncePerRun = true;

    // =========================================================
    // ESTADO INTERNO
    // =========================================================

    private const string PrefsPrefix = "NOVA_TUTORIAL_DONE_";

    private PlayerController cachedPlayerController;
    private Rigidbody2D cachedRigidbody;
    private Collider2D cachedCollider;

    private bool isShowing;
    private bool controllerWasEnabled;
    private RigidbodyConstraints2D originalConstraints;
    private int activationFrame = -1;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        cachedCollider = GetComponent<Collider2D>();

        // Este objeto debe funcionar como zona de trigger.
        if (cachedCollider != null)
        {
            cachedCollider.isTrigger = true;
        }

        // Si este tutorial ya fue visto en esta partida,
        // se desactiva para no volver a interrumpir al jugador.
        if (HasBeenCompleted())
        {
            DisableSelf();
            return;
        }

        // Aseguramos que la UI arranque oculta.
        if (uiRoot != null)
        {
            uiRoot.SetActive(false);
        }

        if (continueHintText != null)
        {
            continueHintText.text = continueHint;
        }

        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    // =========================================================
    // TRIGGER
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isShowing || HasBeenCompleted())
        {
            return;
        }

        // Nova tiene el tag Player y sus scripts en el objeto raíz.
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

        ShowTutorial();
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (!isShowing)
        {
            return;
        }

        // Evita cerrar en el mismo frame en que se abrió,
        // por si la tecla que activó el trigger sigue presionada.
        if (Time.frameCount <= activationFrame)
        {
            return;
        }

        if (AnyDismissInputPressed())
        {
            CloseTutorial();
        }
    }

    // =========================================================
    // MOSTRAR TUTORIAL
    // =========================================================

    private void ShowTutorial()
    {
        isShowing = true;
        activationFrame = Time.frameCount;

        // Guardamos el estado actual del controlador para restaurarlo luego.
        controllerWasEnabled = cachedPlayerController.enabled;
        originalConstraints = cachedRigidbody != null
            ? cachedRigidbody.constraints
            : RigidbodyConstraints2D.None;

        // Congelamos el jugador en seco.
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

        // Actualizamos el contenido de la UI específica de este trigger.
        if (messageText != null)
        {
            messageText.text = message;
        }

        if (continueHintText != null)
        {
            continueHintText.text = continueHint;
        }

        if (speakerImage != null)
        {
            speakerImage.enabled = true;
        }

        if (uiRoot != null)
        {
            uiRoot.SetActive(true);
        }

        // Persistencia por partida: una vez mostrado, queda marcado.
        if (activateOnlyOncePerRun)
        {
            PlayerPrefs.SetInt(GetPrefsKey(), 1);
            PlayerPrefs.Save();
        }
    }

    // =========================================================
    // CERRAR TUTORIAL
    // =========================================================

    private void CloseTutorial()
    {
        isShowing = false;

        if (uiRoot != null)
        {
            uiRoot.SetActive(false);
        }

        // Restauramos el estado del jugador.
        if (cachedRigidbody != null)
        {
            cachedRigidbody.constraints = originalConstraints;
        }

        if (cachedPlayerController != null && controllerWasEnabled)
        {
            cachedPlayerController.enabled = true;
        }

        // Como el controlador vuelve limpio y la velocidad ya fue reseteada,
        // Nova continúa con normalidad.
    }

    // =========================================================
    // INPUT DE CIERRE
    // =========================================================

    private bool AnyDismissInputPressed()
    {
        // Teclado
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            return true;
        }

        // Mouse
        if (Mouse.current != null &&
            (Mouse.current.leftButton.wasPressedThisFrame ||
             Mouse.current.rightButton.wasPressedThisFrame ||
             Mouse.current.middleButton.wasPressedThisFrame))
        {
            return true;
        }


        return false;
    }

    // =========================================================
    // PERSISTENCIA
    // =========================================================

    private bool HasBeenCompleted()
    {
        return PlayerPrefs.GetInt(GetPrefsKey(), 0) == 1;
    }

    private string GetPrefsKey()
    {
        string id = string.IsNullOrWhiteSpace(tutorialId) ? gameObject.name : tutorialId;
        return PrefsPrefix + id;
    }

    // =========================================================
    // UTILIDADES
    // =========================================================

    private void DisableSelf()
    {
        if (uiRoot != null)
        {
            uiRoot.SetActive(false);
        }

        if (cachedCollider == null)
        {
            cachedCollider = GetComponent<Collider2D>();
        }

        if (cachedCollider != null)
        {
            cachedCollider.enabled = false;
        }

        enabled = false;
    }
}