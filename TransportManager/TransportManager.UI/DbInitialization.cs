using System;
using System.Threading;
using System.Threading.Tasks;
using TransportManager.DataEF;
using TransportManager.UI.Helpers;

namespace TransportManager.UI
{
    public static class DbInitialization
    {
        public static void Start()
        {
            try
            {
                Task taskMainDb = PrimaryDbInitializer.InitializeMainDbAsync();
                Console.WriteLine("Запущена инициализация основной базы данных.");
                Task taskStatisticsDb = PrimaryDbInitializer.InitializeTelemetryDbAsync();
                Console.WriteLine("Запущена инициализация базы данных телеметрии.");
                
                Console.Write("\nПожалуйста, подождите.");
                TaskWaiter.WaitAndPrintDot(taskMainDb);
                TaskWaiter.WaitAndPrintDot(taskStatisticsDb);

                if (taskMainDb.Exception != null)
                {
                    foreach (var e in taskMainDb.Exception.InnerExceptions)
                    {
                        throw e;
                    }
                }
                
                Console.WriteLine();
                MessagePrinter.PrintConsoleColorMessage("\nОсновная база данных: Связь установлена.", 
                                                        ConsoleColor.Green);

                if (taskStatisticsDb.Exception != null)
                {
                    Console.WriteLine();
                    foreach (var e in taskStatisticsDb.Exception.InnerExceptions)
                    {
                        throw e;
                    }
                }

                MessagePrinter.PrintConsoleColorMessage("База данных телеметрии: Связь установлена.", 
                                                        ConsoleColor.Green);
            }
            catch (Exception e)
            {
                MessagePrinter.PrintConsoleColorMessage($"Ошибка инициализации БД: {e.Message}", 
                                                        ConsoleColor.Red);

                throw;
            }
        }
    }
}