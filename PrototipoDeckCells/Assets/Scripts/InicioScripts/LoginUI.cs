using UnityEngine;
using TMPro;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField inputLoginNombre;
    public TMP_InputField inputLoginPassword;  // Nuevo campo para contraseña
    public UIManager uiManager;

    public void BotonLogin()
    {
        string nombre = inputLoginNombre.text.Trim();
        string password = inputLoginPassword.text;  // Obtenemos la contraseña
        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(password)) return;

        StartCoroutine(GameAPIManager.instance.LoginUsuario(nombre, password, (token) =>
        {
            if (!string.IsNullOrEmpty(token))
            {
                PlayerPrefs.SetString("nombreJugador", nombre);
                PlayerPrefs.SetString("token", token);  // Guarda token para futuras llamadas
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

