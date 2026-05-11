using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    [Header("Configuración de Sprites")]
    [SerializeField] private Sprite corazonLleno;
    [SerializeField] private Sprite corazonVacio;

    [Header("Contenedor")]
    [SerializeField] private List<Image> imagenesCorazones;

    /// <summary>
    /// Actualiza la visualización de los corazones.
    /// Se llamará desde el evento OnHealthChanged de PlayerHealth.
    /// </summary>
    public void ActualizarCorazones(int saludActual)
    {
        for (int i = 0; i < imagenesCorazones.Count; i++)
        {
            if (i < saludActual)
            {
                imagenesCorazones[i].sprite = corazonLleno;
                imagenesCorazones[i].enabled = true; // Aseguramos que se vea
            }
            else
            {
                // Opción A: Cambiar el sprite a uno vacío (estilo Zelda)
                imagenesCorazones[i].sprite = corazonVacio;
                
                // Opción B: Si prefieres que desaparezcan, descomenta la línea de abajo:
                // imagenesCorazones[i].enabled = false;
            }
        }
    }
}