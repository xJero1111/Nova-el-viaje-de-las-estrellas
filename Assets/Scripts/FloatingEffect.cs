using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.5f; //qué tanto sube y baja
    [SerializeField] private float frequency = 1f;   //qué tan rápido lo hace

    private Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //guardamos la posición inicial del objeto
        startPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        //calculamos el desplazamiento usando una onda senoidal
        float offsetY = Mathf.Sin(Time.time * frequency) * amplitude;

        //aplicamos el movimiento relativo a la posición inicial
        transform.position = startPosition + new Vector3(0f, offsetY, 0f);
    }
}