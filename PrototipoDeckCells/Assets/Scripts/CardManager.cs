using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Carta
{
    public string tipo;
    public Sprite imagen;
}

public class CardManager : MonoBehaviour
{
    public GameObject panelCartas;
    public GameObject bordeCartas;
    public Button[] botonesCartas; 
    public Image[] imagenesCartas; 
    public Carta[] todasLasCartas; 
    public PlayerMovement player;
    public GameObject Fireball;  
    public GameObject Iceball;   

    private bool puedeLanzarFuego = false; 
    private bool puedeLanzarHielo = false; 

    public float cooldownFuego = 3f;
    public float cooldownHielo = 5f;

    private float timerFuego = 0f;
    private float timerHielo = 0f;

    public Image cooldownFuegoImage;
    public Image cooldownHieloImage;


    void Start()
    {
        MostrarCartas();
        cooldownFuegoImage.gameObject.SetActive(false);
        cooldownHieloImage.gameObject.SetActive(false);

    }


    public void MostrarCartas()
    {
        bordeCartas.SetActive(true);
        panelCartas.SetActive(true);
        Time.timeScale = 0f; 

        
        List<Carta> cartasElegidas = new List<Carta>();
        List<int> indicesUsados = new List<int>();

        while (cartasElegidas.Count < 3)
        {
            int index = Random.Range(0, todasLasCartas.Length);
            if (!indicesUsados.Contains(index))
            {
                cartasElegidas.Add(todasLasCartas[index]);
                indicesUsados.Add(index);
            }
        }

        for (int i = 0; i < botonesCartas.Length; i++)
        {
            int index = i; 
            imagenesCartas[i].sprite = cartasElegidas[i].imagen;
            botonesCartas[i].onClick.RemoveAllListeners();
            botonesCartas[i].onClick.AddListener(() => SeleccionarCarta(cartasElegidas[index].tipo));
        }

    }

    public void SeleccionarCarta(string tipo)
    {
        Debug.Log("Carta seleccionada: " + tipo);

        switch (tipo)
        {
            case "velocidad":
                AumentarVelocidad();
                break;
            case "salto":
                AumentarSalto();
                break;
            case "fuego":
                puedeLanzarFuego = true; 
                Debug.Log("Habilidad de fuego activada!");
                cooldownFuegoImage.gameObject.SetActive(true); 
                cooldownFuegoImage.fillAmount = 0f;
                break;
            case "hielo":
                puedeLanzarHielo = true; 
                Debug.Log("Habilidad de hielo activada!");
                cooldownHieloImage.gameObject.SetActive(true); 
                cooldownHieloImage.fillAmount = 0f;
                break;
            case "vida":
                MasVida(); 
                break;
            case "fuerza":
                AumentarFuerza();
                break;
        }

        panelCartas.SetActive(false);
        bordeCartas.SetActive(false);
        Time.timeScale = 1f; 
    }

    private void AumentarVelocidad()
    {
        player.moveSpeed += 1f;
    }

    private void MasVida()
    {
        player.currentHealth += 2;
    }

    private void AumentarSalto()
    {
        player.jumpForce += 2f;
    }

    private void AumentarFuerza()
    {
        player.danoBase += 1f;  
        Debug.Log("Fuerza aumentada! Nuevo dano: " + player.danoBase);
    }

    void Update()
    {
        if (timerFuego > 0)
            timerFuego -= Time.deltaTime;
        if (timerHielo > 0)
            timerHielo -= Time.deltaTime;

        if (puedeLanzarFuego && Input.GetKeyDown(KeyCode.F) && timerFuego <= 0f)
        {
            LanzarBolaDeFuego();
            timerFuego = cooldownFuego;
        }

        if (puedeLanzarHielo && Input.GetKeyDown(KeyCode.F) && timerHielo <= 0f)
        {
            LanzarBolaDeHielo();
            timerHielo = cooldownHielo;
        }

        if (puedeLanzarFuego)
        {
            cooldownFuegoImage.fillAmount = 1 - (timerFuego / cooldownFuego);
        }

        if (puedeLanzarHielo)
        {
            cooldownHieloImage.fillAmount = 1 - (timerHielo / cooldownHielo);
        }


    }


    private void LanzarBolaDeFuego()
    {
        if (Fireball != null)
        {
            GameObject fireball = Instantiate(Fireball, player.transform.position, Quaternion.identity);
            FireballMovement fireballMovement = fireball.GetComponent<FireballMovement>();
            fireballMovement.direction = player.transform.right;
            fireballMovement.speed = 7f;

            fireballMovement.dano = player.danoBase;

            Animator fireballAnimator = fireball.GetComponent<Animator>();
            fireballAnimator.SetTrigger("FireballLaunch");
        }
    }

    private void LanzarBolaDeHielo()
    {
        if (Iceball != null)
        {
            GameObject iceball = Instantiate(Iceball, player.transform.position, Quaternion.identity);
            IceballMovement iceballMovement = iceball.GetComponent<IceballMovement>();
            iceballMovement.direction = player.transform.right;
            iceballMovement.speed = 7f;
            iceballMovement.dano = player.danoBase;

            Collider2D playerCollider = player.GetComponent<Collider2D>();
            Collider2D iceballCollider = iceball.GetComponent<Collider2D>();
            if (playerCollider != null && iceballCollider != null)
            {
                Physics2D.IgnoreCollision(iceballCollider, playerCollider);
            }

            Animator iceballAnimator = iceball.GetComponent<Animator>();
            iceballAnimator.SetTrigger("IceballLaunch");
        }
    }


}
