using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    public float vida = 3f;
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    public float daño = 1f;

    private Transform player;
    private float cooldownTimer = 0f;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isDead = false;

    public float activationRange = 10f;
    private bool hasSeenPlayer = false;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Mathf.Abs(transform.position.x - player.position.x);

        if (!hasSeenPlayer && distance <= activationRange)
        {
            hasSeenPlayer = true;
        }

        if (!hasSeenPlayer)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            return;
        }


        if (distance > attackRange)
        {
            // Mover hacia el jugador
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

            if (direction.x > 0)
                transform.localScale = new Vector3(4, 4, 1);
            else
                transform.localScale = new Vector3(-4, 4, 1);
        }
        else
        {
            // Parar y atacar
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0f);

            if (cooldownTimer <= 0f)
            {
                animator.SetTrigger("Attack");
                Atacar();
                cooldownTimer = attackCooldown;
            }
        }

        cooldownTimer -= Time.deltaTime;
    }


    void Atacar()
    {
        float distance = Mathf.Abs(transform.position.x - player.position.x);
        if (distance <= attackRange)
        {
            player.GetComponent<PlayerMovement>()?.TakeDamage((int)daño, transform.position);
        }
    }

    public void RecibirDaño(float daño)
    {
        if (isDead) return;

        vida -= daño;

        if (vida <= 0)
        {
            Morir();
        }
    }
    private void Morir()
    {
        isDead = true;
        rb.velocity = Vector2.zero;

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        // Destruye después de que la animación termine (ej. 1s)
        Destroy(gameObject, 0.5f); // ajusta el tiempo a lo que dure la animación
    }

}



