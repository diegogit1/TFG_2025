using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public int maxHealth = 3;
    public int currentHealth;
    public float danoBase = 1f; 

    public float invulnerabilityDuration = 1.5f;
    public float knockbackForce = 300f;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isInvulnerable = false;
    private bool isDead = false;

    public TextMeshProUGUI textoVida;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        rb.freezeRotation = true;

    }

    void Update()
    {
        ActualizarVidaUI();

        if (!isDead)
        {
            Move();
            Jump();
        }

        Animate();
    }

    void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        if (moveInput > 0)
            transform.localScale = new Vector3(4, 4, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-4, 4, 1);
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool("IsJumping", true);
            isGrounded = false;
        }
    }


    void Animate()
    {
        if (isGrounded)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
        }
        else
        {
            animator.SetBool("IsJumping", rb.velocity.y > 0.1f);
            animator.SetBool("IsFalling", rb.velocity.y < -0.1f);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("IsJumping", false);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1, collision.transform.position);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void TakeDamage(int amount, Vector3 sourcePosition)
    {
        if (isInvulnerable || isDead) return;

        currentHealth -= amount;
        Debug.Log("Vida restante: " + currentHealth);
        ActualizarVidaUI();


        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hit");
            StartCoroutine(InvulnerabilityCoroutine());

            Vector2 knockbackDir = (transform.position - sourcePosition).normalized;
            rb.AddForce(knockbackDir * knockbackForce);

            StartCoroutine(WaitForHitAnimation());
        }
    }

    void ActualizarVidaUI()
    {
        if (textoVida != null)
            textoVida.text = "HP: " + currentHealth;
    }


    IEnumerator WaitForHitAnimation()
    {
        yield return new WaitForSeconds(0.5f);

        if (isGrounded)
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        else
            animator.SetFloat("Speed", 0);
    }

    IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        spriteRenderer.enabled = true;

        float elapsed = 0f;
        while (elapsed < invulnerabilityDuration)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit"))
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }

            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        spriteRenderer.enabled = true;
        isInvulnerable = false;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("El jugador ha muerto");

        animator.SetTrigger("Death");

        this.enabled = false;
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        StartCoroutine(HandleDeath());
    }

    IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Vector3 ataqueOffset = transform.right * 1.5f * (transform.localScale.x > 0 ? 1 : -1);
        Vector3 centroAtaque = transform.position + ataqueOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centroAtaque, 1f);
    }
}
