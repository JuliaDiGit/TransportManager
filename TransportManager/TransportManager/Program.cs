using System;
using System.IO;
using TransportManager.Loggers;
using TransportManager.Loggers.Abstract;
using TransportManager.UI;

namespace TransportManager
{
    public class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                // запускаем инициализацию баз данных
                DbInitialization.Start();

                // запускаем необходимые дополнительные модули 
                AdditionalModulesLauncher.Launch();

                // формируем путь и создаем папку для хранения логов
                const string folderLogs = "Logs";
                string pathFolderLogs = Path.Combine(Directory.GetCurrentDirectory(), folderLogs);
                Directory.CreateDirectory(pathFolderLogs);

                // создаём Логгер по указанному пути
                ILogger logger = new Logger(pathFolderLogs);

                // запускаем авторизацию
                var authorization = new Authorization(logger);
                authorization.Start();

                // после выхода пользователя из программы - оставливаем работу TelemetryStorageServer и MessageSenderEmulator
                ProcessKiller.Shutdown("TelemetryStorageServer");
                ProcessKiller.Shutdown("MessageSenderEmulator");
            }
            catch (Exception)
            {
                Console.ReadKey();
                return;
            }

            Console.ReadKey();
        }
    }
}
