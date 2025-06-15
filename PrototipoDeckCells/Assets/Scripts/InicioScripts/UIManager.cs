using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static bool sesionIniciada = false;

    public GameObject panelInicio;
    public GameObject panelLogin;
    public GameObject panelRegistro;
    public GameObject panelMenu;

    void Start()
    {
    if (sesionIniciada)
    {
        MostrarSolo(panelMenu); // Ya se inició sesión durante esta sesión de juego
    }
    else
    {
        // Si hay datos guardados de otra sesión anterior, los borramos solo si el juego ha sido reiniciado completamente
        if (!PlayerPrefs.HasKey("token"))
        {
            PlayerPrefs.DeleteKey("nombreJugador");
        }

        MostrarSolo(panelInicio);
    }
}




    public void MostrarSolo(GameObject panel)
    {
        panelInicio.SetActive(false);
        panelLogin.SetActive(false);
        panelRegistro.SetActive(false);
        panelMenu.SetActive(false);

        panel.SetActive(true);
    }

    public void EstablecerSesionIniciada()
    {
        sesionIniciada = true;
    }

    public void CerrarSesion()
    {
        sesionIniciada = false;
        PlayerPrefs.DeleteKey("nombreJugador");
        MostrarSolo(panelInicio);
    }
}

