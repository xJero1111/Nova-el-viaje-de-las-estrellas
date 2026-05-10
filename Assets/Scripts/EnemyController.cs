using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 2f;
    public Transform sensorSuelo; // El objeto vacío "Ground Check"
    public LayerMask capaSuelo;   // Selecciona "Ground"
    public float distanciaDeteccion = 0.2f;

    [Header("Configuración de Vida")]
    public int vidaMaxima = 3;
    private int vidaActual;

    private Rigidbody2D rb;
    private bool mirandoDerecha = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        vidaActual = vidaMaxima;
    }

    void Update()
{
    Patrullar();
}

void Patrullar()
{
    // 1. Raycasts: Detectamos suelo y pared
    RaycastHit2D haySuelo = Physics2D.Raycast(sensorSuelo.position, Vector2.down, distanciaDeteccion, capaSuelo);
    RaycastHit2D hayPared = Physics2D.Raycast(sensorSuelo.position, transform.right, 0.2f, capaSuelo);

    // 2. ¿Debemos girar?
    if (haySuelo.collider == null || hayPared.collider != null)
    {
        Girar();
    }
    else
    {
        // 3. Solo aplicamos velocidad si NO estamos girando
        rb.linearVelocity = new Vector2(transform.right.x * velocidad, rb.linearVelocity.y);
    }
}

void Girar()
{
    // Cambiamos el estado lógico
    mirandoDerecha = !mirandoDerecha;

    // Aplicamos la rotación física
    if (mirandoDerecha)
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
    }
    else
    {
        transform.eulerAngles = new Vector3(0, -180, 0);
    }

    // --- EL TRUCO MAESTRO ---
    // Le damos un pequeño empujón instantáneo en la nueva dirección 
    // para que el sensor "salga" de la zona de peligro inmediatamente.
    rb.linearVelocity = new Vector2(transform.right.x * velocidad, rb.linearVelocity.y);
}

    
    // 2. RECIBIR DAÑO (Nova ataca con Spin Attack)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detectamos el Trigger del ataque de Nova
        if (other.CompareTag("PlayerAttack"))
        {
            RecibirDanio(1);
        }
    }

    public void RecibirDanio(int cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log("Enemigo herido. Vida: " + vidaActual);

        if (vidaActual <= 0)
        {
            Debug.Log("Enemigo derrotado");
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (sensorSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(sensorSuelo.position, sensorSuelo.position + Vector3.down * distanciaDeteccion);
            Gizmos.DrawLine(sensorSuelo.position, sensorSuelo.position + transform.right * 0.2f);
        }
    }
}