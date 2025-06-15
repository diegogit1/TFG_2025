using UnityEngine;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class ServerAutoStart : MonoBehaviour
{
    private Process serverProcess = null;

    private string workingDirectory;
    private string arguments = "main:app --reload";

    void Start()
    {
        // Detectar carpeta base y construir workingDirectory
        string baseFolder = FindBaseFolder(Application.dataPath, "TFG_DIEGO_GONZALEZ");
        if (baseFolder == null)
        {
            UnityEngine.Debug.LogError("No se encontró la carpeta base 'TFG_DIEGO_GONZALEZ'. No se puede iniciar el servidor.");
            return;
        }

        workingDirectory = Path.Combine(baseFolder, "TFG_2025", "API_TFG");
        UnityEngine.Debug.Log($"Working directory detectado: {workingDirectory}");

        if (IsServerRunning("127.0.0.1", 8000))
        {
            UnityEngine.Debug.Log("Servidor ya está corriendo en el puerto 8000.");
            serverProcess = null;
        }
        else
        {
            StartServerProcess();
        }
    }

    string FindBaseFolder(string startPath, string folderName)
    {
        var directory = new DirectoryInfo(startPath);
        while (directory != null && directory.Name != folderName)
        {
            directory = directory.Parent;
        }
        return directory?.FullName;
    }

    void StartServerProcess()
    {
        string uvicornCommand = DetectUvicornCommand();

        if (uvicornCommand == null)
        {
            UnityEngine.Debug.LogError("No se pudo encontrar uvicorn. Asegúrate de tenerlo instalado (pip install uvicorn) y que esté en el PATH.");
            return;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = uvicornCommand.Split(' ')[0],
            Arguments = string.Join(" ", uvicornCommand.Split(' ').Skip(1)) + " " + arguments,
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

    string DetectUvicornCommand()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = "-m uvicorn --version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                process.WaitForExit(1000);
                if (process.ExitCode == 0)
                    return "python -m uvicorn";
            }
        }
        catch {}

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "uvicorn",
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                process.WaitForExit(1000);
                if (process.ExitCode == 0)
                    return "uvicorn";
            }
        }
        catch {}

        return null;
    }

    bool IsServerRunning(string host, int port)
    {
        try
        {
            using (TcpClient client = new TcpClient())
            {
                var task = client.ConnectAsync(host, port);
                bool connected = task.Wait(500);
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
