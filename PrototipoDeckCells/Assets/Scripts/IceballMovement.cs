using UnityEngine;

public class IceballMovement : MonoBehaviour
{
    public Vector2 direction;  // Direcci�n de movimiento
    public float speed = 10f;  // Velocidad de la bola de hielo
    public float dano;         // Da�o de la bola de hielo (ser� asignado por el jugador)

    private Rigidbody2D rb;    // Referencia al Rigidbody2D

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Mover la bola de hielo en la direcci�n que desees
        rb.velocity = direction * speed;  // Aplica velocidad en la direcci�n
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Si la bola de hielo colisiona con un enemigo
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Aplicar el da�o al enemigo
            Enemy enemigo = collision.gameObject.GetComponent<Enemy>();
            if (enemigo != null)
            {
                enemigo.RecibirDano(dano);
            }

            // Destruir la bola de hielo
            Destroy(gameObject);
        }
    }
}




