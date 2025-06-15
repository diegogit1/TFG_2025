using UnityEngine;

public class RegistroControlador : MonoBehaviour
{
    public GameObject panelRegistro;
    public GameObject panelMenu;

    void Start()
    {
        if (PlayerPrefs.HasKey("nombreJugador"))
        {
            panelRegistro.SetActive(false);
            panelMenu.SetActive(true);
        }
        else
        {
            panelRegistro.SetActive(true);
            panelMenu.SetActive(false);
        }
    }

    public void OnUsuarioRegistrado()
    {
        panelRegistro.SetActive(false);
        panelMenu.SetActive(true);
    }

    public void CerrarSesion()
    {
        PlayerPrefs.DeleteKey("nombreJugador");
        panelRegistro.SetActive(true);
        panelMenu.SetActive(false);
    }
}

