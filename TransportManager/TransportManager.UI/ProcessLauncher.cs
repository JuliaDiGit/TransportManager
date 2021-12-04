using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TransportManager.UI.Helpers;

namespace TransportManager.UI
{
    /// <summary>
    ///     класс ProcessLauncher используется для запуска процесса
    /// </summary>
    public static class ProcessLauncher
    {
        /// <summary>
        ///     метод Start инициирует запуск процесса
        /// </summary>
        /// <param name="processName">имя процесса</param>
        /// <param name="processPath">путь до exe файла</param>
        /// <param name="arguments">аргументы, передаваемые процессу</param>
        public static void Start(string processName, string processPath, string arguments = null)
        {
            try
            {
                Task launchTask = LaunchAsync(processPath, arguments);
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

        /// <summary>
        ///     метод LaunchAsync ассинхронно запускает новый процесс
        /// </summary>
        /// <param name="processPath">путь до exe файла</param>
        /// <param name="arguments">аргументы, передаваемые процессу</param>
        /// <returns></returns>
        private static async Task LaunchAsync(string processPath, string arguments = null)
        {
            await Task.Run(() => Process.Start(processPath, arguments));
        }
    }
}