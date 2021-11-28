using System;
using System.Diagnostics;
using System.IO;
using TransportManager.Logger.Abstract;
using TransportManager.UI;

namespace TransportManager
{
    public class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                DbInitialization.Start();
            }
            catch (Exception)
            {
                Console.ReadKey();
                return;
            }
            
            try
            {
                var projectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\"));
                var serverPath = Path.Combine(projectPath, 
                                              @"TelemetryStorageServer\TelemetryStorageServer\bin\Debug\TelemetryStorageServer.exe");

                Process.Start(serverPath);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Resources.Success_StartStorageServer);
                Console.ResetColor();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Resources.Fail_StartStorageServer);
                Console.WriteLine(Resources.Error + e.Message);
                Console.ResetColor();

                Console.ReadKey();
                return;
            }
            
            const string folderLogs = "Logs";
            string pathFolderLogs = Path.Combine(Directory.GetCurrentDirectory(), folderLogs);
            Directory.CreateDirectory(pathFolderLogs);
            ILogger logger = new Logger.Logger(pathFolderLogs);
            
            var authorization = new Authorization(logger);
            authorization.Start();
            
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
