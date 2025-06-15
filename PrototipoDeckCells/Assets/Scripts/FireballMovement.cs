using UnityEngine;

public class FireballMovement : MonoBehaviour
{
    public float speed = 10f;  // Velocidad de la bola de fuego
    public Vector2 direction;  // Direcci�n en la que la bola se mueve
    public float dano;         // Da�o de la bola de fuego (ser� asignado por el jugador)

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Mover la bola de fuego en la direcci�n que desees (por ejemplo, hacia adelante)
        rb.velocity = direction * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Si la bola de fuego colisiona con un enemigo
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Aplicar el da�o al enemigo
            Enemy enemigo = collision.gameObject.GetComponent<Enemy>();
            if (enemigo != null)
            {
                enemigo.RecibirDano(dano);
            }

            // Destruir la bola de fuego
            Destroy(gameObject);
        }
    }
}


