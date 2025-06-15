using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float dano = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemigo = collision.GetComponent<Enemy>();
            if (enemigo != null)
            {
                enemigo.RecibirDano(dano);
            }
        }
    }
}

