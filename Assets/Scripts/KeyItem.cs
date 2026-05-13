using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KeyItem : MonoBehaviour
{
    [SerializeField] private int keyId = 0;

    // Esta llave debe tener un Collider2D en modo Trigger.
    // Cuando Nova la toque, se busca su PlayerInventory en el padre
    // y se agrega la llave al inventario antes de destruir el objeto.
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInventory playerInventory = other.GetComponentInParent<PlayerInventory>();

        if (playerInventory == null)
        {
            return;
        }

        playerInventory.AddKey(keyId);
        Destroy(gameObject);
    }

    public int GetKeyId()
    {
        return keyId;
    }
}