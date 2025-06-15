using UnityEngine;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

public class ServerAutoStart : MonoBehaviour
{
    private Process serverProcess = null;

    // Cambia estas rutas y argumentos según tu entorno
    private string uvicornPath = @"C:\Users\diego\AppData\Local\Packages\PythonSoftwareFoundation.Python.3.10_qbz5n2kfra8p0\LocalCache\local-packages\Python310\Scripts\uvicorn.exe";
    private string workingDirectory = @".\API_TFG";
    private string arguments = "main:app --reload";

    void Start()
    {
        if (IsServerRunning("127.0.0.1", 8000))
        {
            UnityEngine.Debug.Log("Servidor ya está corriendo en el puerto 8000.");
            serverProcess = null; // No controlamos proceso externo
        }
        else
        {
            StartServerProcess();
        }
    }

    void StartServerProcess()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = uvicornPath,
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = workingDirectory,
        };

        try
        {
            serverProcess = Process.Start(startInfo);

            serverProcess.OutputDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) UnityEngine.Debug.Log(e.Data); };
            serverProcess.ErrorDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) UnityEngine.Debug.LogError(e.Data); };

            serverProcess.BeginOutputReadLine();
            serverProcess.BeginErrorReadLine();

            UnityEngine.Debug.Log("Servidor uvicorn iniciado.");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Error al iniciar el servidor uvicorn: " + e.Message);
        }
    }

    bool IsServerRunning(string host, int port)
    {
        try
        {
            using (TcpClient client = new TcpClient())
            {
                var task = client.ConnectAsync(host, port);
                bool connected = task.Wait(500); // espera max 500ms
                return connected && client.Connected;
            }
        }
        catch
        {
            return false;
        }
    }

    void OnApplicationQuit()
    {
        if (serverProcess != null && !serverProcess.HasExited)
        {
            try
            {
                serverProcess.Kill();
                UnityEngine.Debug.Log("Servidor uvicorn cerrado al salir del juego.");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("No se pudo cerrar el servidor uvicorn: " + e.Message);
            }
        }
    }
}


