using System;
using System.Diagnostics;
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

                // формируем путь для запуска TelemetryStorageServer
                var projectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\"));
                var serverPath = Path.Combine(projectPath,
                                              @"TelemetryStorageServer\TelemetryStorageServer\bin\Debug\TelemetryStorageServer.exe");

                // запускаем TelemetryStorageServer по сформированному пути
                ProcessLauncher.Start(serverPath, "TelemetryStorageServer");
            }
            catch (Exception)
            {
                Console.ReadKey();
                return;
            }

            // формируем путь и создаем папку для хранения логов
            const string folderLogs = "Logs";
            string pathFolderLogs = Path.Combine(Directory.GetCurrentDirectory(), folderLogs);
            Directory.CreateDirectory(pathFolderLogs);

            // создаём Логгер по указанному пути
            ILogger logger = new Logger(pathFolderLogs);
            
            // запускаем авторизацию
            var authorization = new Authorization(logger);
            authorization.Start();

            // по завершению программы оставливаем работу TelemetryStorageServer
            var proc = Process.GetProcessesByName("TelemetryStorageServer");
            foreach (var process in proc)
            {
                try
                {
                    process.Kill();
                    
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(Resources.Success_KillStorageServer);
                    Console.ResetColor();
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(Resources.Fail_KillStorageServer);
                    Console.WriteLine(Resources.Error + e.Message);
                    Console.ResetColor();
                }
            }
            
            Console.ReadKey();
        }
    }
}
