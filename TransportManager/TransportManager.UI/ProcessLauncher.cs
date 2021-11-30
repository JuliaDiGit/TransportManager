using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TransportManager.UI.Helpers;

namespace TransportManager.UI
{
    public static class ProcessLauncher
    {
        public static void Start(string processPath, string processName)
        {
            try
            {
                Task launchTask = Launch(processPath);
                Console.WriteLine($"\nИдёт запуск процесса {processName}.");

                Console.Write("\nПожалуйста, подождите.");
                TaskWaiter.WaitAndPrintDot(launchTask);

                if (launchTask.Exception != null)
                {
                    foreach (var e in launchTask.Exception.InnerExceptions)
                    {
                        throw e;
                    }
                }

                Console.WriteLine();
                MessagePrinter.PrintConsoleColorMessage($"\n{processName} успешно запущен.", 
                                                        ConsoleColor.Green);
            }
            catch (Exception e)
            {
                Console.WriteLine();
                MessagePrinter.PrintConsoleColorMessage($"\nОшибка запуска процесса: {e.Message}", 
                                                        ConsoleColor.Red);

                throw;
            }

        }
        private static async Task Launch(string processPath)
        {
            await Task.Run(() => Process.Start(processPath));
        }
    }
}