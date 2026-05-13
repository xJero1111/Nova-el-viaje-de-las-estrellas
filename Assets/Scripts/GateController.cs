using UnityEngine;

public class GateController : MonoBehaviour
{
    [SerializeField] private int requiredKeyId = 0;
    [SerializeField] private bool consumeKeyOnOpen = true;

    [Header("Save ID")]
    [SerializeField] private string gateSaveId = "Gate_01";

    [SerializeField] private Collider2D blockingCollider;
    [SerializeField] private GameObject closedVisual;
    [SerializeField] private GameObject openedVisual;
    [SerializeField] private Animator gateAnimator;

    private const string GateOpenPrefix = "NOVA_GATE_OPEN_";
    private bool isOpen = false;

    private void Awake()
    {
        LoadGateState();
    }

    // La puerta detecta al jugador por su collider, pero busca
    // PlayerInventory en el padre para que funcione con la estructura
    // actual de Nova.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen)
        {
            return;
        }

        PlayerInventory playerInventory = other.GetComponentInParent<PlayerInventory>();

        if (playerInventory == null)
        {
            return;
        }

        if (playerInventory.HasKey(requiredKeyId))
        {
            if (consumeKeyOnOpen)
            {
                playerInventory.UseKey(requiredKeyId);
            }

            OpenGate();
        }
    }

    private void OpenGate()
    {
        isOpen = true;

        if (blockingCollider != null)
        {
            blockingCollider.enabled = false;
        }

        if (closedVisual != null)
        {
            closedVisual.SetActive(false);
        }

        if (openedVisual != null)
        {
            openedVisual.SetActive(true);
        }

        if (gateAnimator != null)
        {
            gateAnimator.SetTrigger("Open");
        }

        SaveGateState();
    }

    private void LoadGateState()
    {
        if (PlayerPrefs.GetInt(GetSaveKey(), 0) == 1)
        {
            isOpen = true;

            if (blockingCollider != null)
            {
                blockingCollider.enabled = false;
            }

            if (closedVisual != null)
            {
                closedVisual.SetActive(false);
            }

            if (openedVisual != null)
            {
                openedVisual.SetActive(true);
            }
        }
        else
        {
            isOpen = false;

            if (blockingCollider != null)
            {
                blockingCollider.enabled = true;
            }

            if (closedVisual != null)
            {
                closedVisual.SetActive(true);
            }

            if (openedVisual != null)
            {
                openedVisual.SetActive(false);
            }
        }
    }

    private void SaveGateState()
    {
        PlayerPrefs.SetInt(GetSaveKey(), 1);
        PlayerPrefs.Save();
    }

    private string GetSaveKey()
    {
        return GateOpenPrefix + gateSaveId;
    }
}