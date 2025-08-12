using System;
using System.IO;

namespace PokeAPI.Logs
{
    public class Logger<T>
    {
        private readonly string logFilePath;

        public Logger()
        {
            // Carpeta Logs en el directorio base del proyecto
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            logFilePath = Path.Combine(logDirectory, $"log_{typeof(T).Name}.txt");
        }

        public void RegistrarERROR(Exception ex, string mensajePersonalizado = "")
        {
            string logMessage = $"{DateTime.Now}: ERROR en {typeof(T).Name} - {mensajePersonalizado} \n{ex}\n";
            EscribirLog(logMessage);
        }

        public void RegistrarINFO(string mensaje)
        {
            string logMessage = $"{DateTime.Now}: INFO en {typeof(T).Name} - {mensaje}\n";
            EscribirLog(logMessage);
        }

        private void EscribirLog(string mensaje)
        {
            try
            {
                File.AppendAllText(logFilePath, mensaje);
            }
            catch
            {
                // En caso de error al escribir log, no interrumpir la aplicación
            }
        }
    }
}
