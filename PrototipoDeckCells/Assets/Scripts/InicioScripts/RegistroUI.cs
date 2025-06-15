using UnityEngine;
using TMPro;

public class RegistroUI : MonoBehaviour
{
    public TMP_InputField inputRegistroNombre;
    public TMP_InputField inputRegistroPassword;
    public UIManager uiManager;

    public void BotonRegistrar()
    {
        string nombre = inputRegistroNombre.text.Trim();
        string password = inputRegistroPassword.text;
        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(password)) return;

        StartCoroutine(GameAPIManager.instance.RegistrarUsuario(nombre, password, (exito) =>
        {
            if (exito)
            {
                PlayerPrefs.SetString("nombreJugador", nombre);
                uiManager.EstablecerSesionIniciada();
                uiManager.MostrarSolo(uiManager.panelMenu);
            }
            else
            {
                Debug.LogWarning("Error registrando usuario.");
            }
        }));
    }
}


