using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TransportManager.Common.Helpers;

namespace TransportManager.UI
{
    /// <summary>
    ///     класс ProcessKiller используется для завершения работы процесса
    /// </summary>
    public static class ProcessKiller
    {
        /// <summary>
        ///     метод Shutdown инициирует завершение работы процесса
        /// </summary>
        /// <param name="processName">имя процесса</param>
        public static void Shutdown(string processName)
        {
            var telemetryStorageServerProcess = Process.GetProcessesByName(processName);
            foreach (var process in telemetryStorageServerProcess)
            {
                try
                {
                    Task shutdownTask = CloseAsync(process);
                    Console.WriteLine($"\nИдёт завершение процесса {processName}.");

                    Console.Write("\nПожалуйста, подождите.");
                    TaskWaiter.WaitAndPrintDot(shutdownTask);

                    Console.WriteLine();
                    MessagePrinter.PrintConsoleColorMessage($"{processName}: работа процесса успешно завершена.",
                                                            ConsoleColor.Green);
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    MessagePrinter.PrintConsoleColorMessage($"{processName}: не удалось завершить работу процесса.",
                                                            ConsoleColor.Red);
                    MessagePrinter.PrintConsoleColorMessage($"Ошибка: {e.Message}", ConsoleColor.Red);
                }
            }
        }

        /// <summary>
        ///     метод CloseAsync ассинхронно завершает процесс
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private static async Task CloseAsync(Process process)
        {
            await Task.Run(process.CloseMainWindow);
            await Task.Run(process.Close);
        }
    }
}
