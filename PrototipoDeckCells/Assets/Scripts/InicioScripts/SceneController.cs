using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public void LoadMenuScene()
    {
        SceneManager.LoadScene("Inicio");
    }

    public void Jugar()
    {
        SceneManager.LoadScene("FirstLevel");  // Cambia el nombre por tu escena
    }

    public void Ajustes()
    {
        // M�s adelante puedes activar un panel de opciones
        Debug.Log("Abrir ajustes (pr�ximamente)");
    }

    public void Salir()
    {
        Debug.Log("Salir del juego");
        Application.Quit();
    }

}
