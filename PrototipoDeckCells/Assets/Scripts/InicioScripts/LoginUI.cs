using UnityEngine;
using TMPro;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField inputLoginNombre;
    public TMP_InputField inputLoginPassword;
    public UIManager uiManager;

    public void BotonLogin()
    {
        string nombre = inputLoginNombre.text.Trim();
        string password = inputLoginPassword.text;
        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(password)) return;

        StartCoroutine(GameAPIManager.instance.LoginUsuario(nombre, password, (token) =>
        {
            if (!string.IsNullOrEmpty(token))
            {
                PlayerPrefs.SetString("nombreJugador", nombre);
                PlayerPrefs.SetString("token", token);
                uiManager.EstablecerSesionIniciada();
                uiManager.MostrarSolo(uiManager.panelMenu);
            }
            else
            {
                Debug.LogWarning("Credenciales incorrectas.");
            }
        }));
    }
}

