using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class GameAPIManager : MonoBehaviour
{
    public static GameAPIManager instance;
    private string baseUrl = "http://127.0.0.1:8000";

    public string token = "";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator RegistrarUsuario(string nombre, string password, Action<bool> callback)
    {
        string json = $"{{\"nombre\": \"{nombre}\", \"password\": \"{password}\"}}";

        using (UnityWebRequest www = new UnityWebRequest($"{baseUrl}/registrar/", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
                callback?.Invoke(true);
            else
                callback?.Invoke(false);
        }
    }

    public IEnumerator LoginUsuario(string nombre, string password, Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", nombre);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post($"{baseUrl}/login", form))
        {
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;

                try
                {
                    var tokenObj = JsonUtility.FromJson<TokenResponse>(jsonResponse);
                    token = tokenObj.access_token;  
                    callback?.Invoke(token);
                }
                catch
                {
                    Debug.LogWarning("No se pudo parsear el token.");
                    callback?.Invoke("");
                }
            }
            else
            {
                Debug.LogWarning("Error al hacer login: " + www.error);
                callback?.Invoke("");
            }
        }
    }

    [Serializable]
    private class TokenResponse
    {
        public string access_token;
        public string token_type;
    }

    [Serializable]
    public class ResultadoPartida
    {
        public int puntuacion;
        public float tiempo;
    }

    public void EnviarResultados(int puntuacion, float tiempo)
    {
        ResultadoPartida data = new ResultadoPartida
        {
            puntuacion = puntuacion,
            tiempo = tiempo
        };

        string json = JsonUtility.ToJson(data);
        StartCoroutine(EnviarPOST("/actualizar", json));
    }

    private IEnumerator EnviarPOST(string endpoint, string json)
    {
        UnityWebRequest request = new UnityWebRequest(baseUrl + endpoint, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        if (!string.IsNullOrEmpty(token))
        {
            request.SetRequestHeader("Authorization", "Bearer " + token);
        }
        else
        {
            Debug.LogWarning(" No hay token disponible. ¿Iniciaste sesión?");
        }

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al enviar resultados: " + request.error);
        }
        else
        {
            Debug.Log("Resultados enviados correctamente: " + request.downloadHandler.text);
        }
    }
}
