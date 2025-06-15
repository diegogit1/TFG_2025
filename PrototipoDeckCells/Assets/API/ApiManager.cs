using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ApiManager : MonoBehaviour
{
    private string baseUrl = "http://127.0.0.1:8000";  // Cambia al IP donde esté tu API si no es local

    public IEnumerator RegistrarUsuario(string nombre)
    {
        string url = baseUrl + "/registrar/";
        // Construir JSON
        string json = JsonUtility.ToJson(new Usuario { nombre = nombre, puntuacion = 0, tiempo = 0f });

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Registro exitoso: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error en registro: " + www.error);
        }
    }

    [System.Serializable]
    public class Usuario
    {
        public string nombre;
        public int puntuacion;
        public float tiempo;
    }
}

