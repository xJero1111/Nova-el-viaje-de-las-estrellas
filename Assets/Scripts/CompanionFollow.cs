using UnityEngine;

public class CompanionFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // Nova

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0.8f, 0.6f, 0f); // distancia flotante respecto a Nova
    [SerializeField] private float smoothTime = 0.25f; // suavizado del movimiento

    private Vector3 currentVelocity; // necesario para SmoothDamp

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // Posición objetivo con un pequeño desplazamiento para que parezca que flota
        Vector3 targetPosition = target.position + offset;
        targetPosition.z = transform.position.z; // mantener la profundidad visual actual

        // Movimiento orgánico y suave
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            smoothTime
        );
    }
}