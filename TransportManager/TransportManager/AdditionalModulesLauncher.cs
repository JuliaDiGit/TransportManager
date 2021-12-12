using System.Configuration;
using System.IO;
using TransportManager.UI;

namespace TransportManager
{
    /// <summary>
    ///     класс AdditionalModulesLauncher используется для запуска дополнительных модулей
    /// </summary>
    public static class AdditionalModulesLauncher
    {
        /// <summary>
        ///     метод Launch запускает дополнительые модули
        /// </summary>
        public static void Launch()
        {
            // считываем инфу из app.config
            var telemetryStorageServer = ConfigurationManager.AppSettings.Get("TelemetryStorageServer");
            var messageSenderEmulator = ConfigurationManager.AppSettings.Get("MessageSenderEmulator");

            var projectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\"));

            if (telemetryStorageServer.ToUpper() == "YES")
            {
                // формируем путь для запуска TelemetryStorageServer
                var serverPath = Path.Combine(projectPath,
                                              @"TelemetryStorageServer\TelemetryStorageServer\bin\Debug\TelemetryStorageServer.exe");

                // получаем значение ReceivingMethod из app.config
                var receivingMethod = ConfigurationManager.AppSettings.Get("ReceivingMethod");
                if (receivingMethod != "Http" && receivingMethod != "TransactionalQueue") receivingMethod = null;

                // запускаем TelemetryStorageServer по сформированному пути с аргументом ReceivingMethod
                ProcessLauncher.Start("TelemetryStorageServer", serverPath, receivingMethod);
            }

            if (messageSenderEmulator.ToUpper() == "YES")
            {
                var senderPath = Path.Combine(projectPath,
                                              @"MessageSenderEmulator\MessageSenderEmulator\bin\Debug\MessageSenderEmulator.exe");

                ProcessLauncher.Start("MessageSenderEmulator", senderPath);
            }
        }
    }
}
