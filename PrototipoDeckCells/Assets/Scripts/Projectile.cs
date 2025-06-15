using UnityEngine;
using System.Collections;

public class ProjectileShooter : MonoBehaviour
{
    public PlayerMovement player;
    public GameObject projectilePrefab;

    public float cooldown = 1f;
    private float cooldownTimer = 0f;

    private Coroutine currentCoroutine;

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && cooldownTimer <= 0f)
        {
            ShootProjectile();
            cooldownTimer = cooldown;
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab == null || player == null)
        {
            Debug.LogWarning("Falta asignar el prefab o el jugador.");
            return;
        }

        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        // Inicia y guarda la referencia a la coroutine
        currentCoroutine = StartCoroutine(DelayedShoot(1f));
    }

    IEnumerator DelayedShoot(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (this == null || gameObject == null || player == null)
            yield break;

        GameObject proyectil = Instantiate(projectilePrefab, player.transform.position, Quaternion.identity);

        Collider2D playerCol = player.GetComponent<Collider2D>();
        Collider2D proyectilCol = proyectil.GetComponent<Collider2D>();
        if (playerCol != null && proyectilCol != null)
        {
            Physics2D.IgnoreCollision(proyectilCol, playerCol);
        }

        ProjectileMovement movimiento = proyectil.GetComponent<ProjectileMovement>();
        if (movimiento != null)
        {
            Vector2 direccion = player.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            movimiento.direction = direccion;
            movimiento.speed = 7f;
            movimiento.damage = player.danoBase;
        }
    }

    void OnDestroy()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
    }
}

