using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject cartaPanel;
    public PlayerMovement player;

    private int puntuacionTotal = 0;
    private float tiempoInicio;
    private bool juegoFinalizado = false;

    public GameObject victoryPanel;
    public TMPro.TextMeshProUGUI puntuacionText;
    public TMPro.TextMeshProUGUI tiempoText;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PausarJuego();
        cartaPanel.SetActive(true);
        tiempoInicio = Time.time;
    }

    public void SeleccionarCarta()
    {
        Debug.Log("Carta seleccionada");
        player.moveSpeed += 0.5f;

        cartaPanel.SetActive(false);
        ReanudarJuego();
    }

    void PausarJuego()
    {
        Time.timeScale = 0f;
    }

    void ReanudarJuego()
    {
        Time.timeScale = 1f;
    }

    public void AgregarPuntos(int puntos)
    {
        puntuacionTotal += puntos;
        Debug.Log("Puntos: " + puntuacionTotal);
    }

    public void TerminarJuego()
{
    if (juegoFinalizado) return;

    juegoFinalizado = true;

    float tiempoTotal = Time.time - tiempoInicio;

    Debug.Log($"Juego terminado. Puntos: {puntuacionTotal}, Tiempo: {tiempoTotal}");

    if (GameAPIManager.instance != null)
    {
        GameAPIManager.instance.EnviarResultados(puntuacionTotal, tiempoTotal);
    }

    if (victoryPanel != null)
    {
        victoryPanel.SetActive(true);
        puntuacionText.text = "Puntuación: " + puntuacionTotal;
        tiempoText.text = "Tiempo: " + tiempoTotal.ToString("F2") + "s";
    }

    Time.timeScale = 0f;
}

}

