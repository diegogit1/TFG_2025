using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float vida = 3f;
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    public float dano = 1f;

    private Transform player;
    private float cooldownTimer = 0f;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isDead = false;

    public float activationRange = 10f;
    private bool hasSeenPlayer = false;

    public int puntos = 100;

    public bool esJefeFinal = false;


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
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            player.GetComponent<PlayerMovement>()?.TakeDamage((int)dano, transform.position);
        }
    }

    public void RecibirDano(float dano)
    {
        if (isDead) return;

        vida -= dano;

        if (vida <= 0)
        {
            Morir();
        }
    }
    private void Morir()
    {
        isDead = true;
        rb.velocity = Vector2.zero;

        if (animator != null)
            animator.SetTrigger("Death");

        GameManager.Instance?.AgregarPuntos(puntos);

        if (esJefeFinal)
            GameManager.Instance?.TerminarJuego();

    Destroy(gameObject, 0.3f);
    }

}



