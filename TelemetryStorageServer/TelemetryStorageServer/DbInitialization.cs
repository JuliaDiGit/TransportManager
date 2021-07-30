using System;
using System.Threading;
using System.Threading.Tasks;
using Data;

namespace TelemetryStorageServer
{
    public static class DbInitialization
    {
        public static void Start()
        {
            try
            {
                Task taskStatisticsDb = PrimaryDbInitializer.InitializeTelemetryDbAsync();
                Console.WriteLine("Запущена инициализация базы данных телеметрии.");
                
                Console.Write("\nПожалуйста, подождите.");
                TaskWaiter.Wait(taskStatisticsDb);

                if (taskStatisticsDb.Exception != null)
                {
                    Console.WriteLine();
                    foreach (var e in taskStatisticsDb.Exception.InnerExceptions)
                    {
                        throw e;
                    }
                }
                
                Console.WriteLine();
                MessagesPrinter.PrintColorMessage("\nБаза данных телеметрии: Связь установлена.", ConsoleColor.Green);
            }
            catch (Exception e)
            {
                Console.WriteLine();
                MessagesPrinter.PrintColorMessage($"\nОшибка инициализации БД: {e.Message}", ConsoleColor.Red);
                throw;
            }
        }
    }
}