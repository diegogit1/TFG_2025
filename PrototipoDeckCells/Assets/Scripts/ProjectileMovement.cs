using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public float speed = 10f;
    public Vector2 direction;
    public float damage = 1f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemigo = collision.gameObject.GetComponent<Enemy>();
            if (enemigo != null)
            {
                enemigo.RecibirDano(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            // Opcional: destruir proyectil al tocar suelo o pared
            Destroy(gameObject);
        }
    }
}
