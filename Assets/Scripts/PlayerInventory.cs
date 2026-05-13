using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<int> keys = new List<int>();

    // Este script puede vivir en el mismo GameObject que PlayerController.
    // No depende de la lógica de movimiento, salto, dash o combate.
    public void AddKey(int keyId)
    {
        if (!keys.Contains(keyId))
        {
            keys.Add(keyId);
        }
    }

    public bool HasKey(int keyId)
    {
        return keys.Contains(keyId);
    }

    public bool UseKey(int keyId)
    {
        if (!HasKey(keyId))
        {
            return false;
        }

        keys.Remove(keyId);
        return true;
    }
}