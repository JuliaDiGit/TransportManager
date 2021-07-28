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
                Wait(taskStatisticsDb);

                if (taskStatisticsDb.Exception != null)
                {
                    Console.WriteLine();
                    foreach (var e in taskStatisticsDb.Exception.InnerExceptions)
                    {
                        throw e;
                    }
                }
                
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nБаза данных телеметрии: Связь установлена.");
                Console.ResetColor();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nОшибка инициализации БД: {e.Message}");
                Console.ResetColor();
                throw;
            }
        }

        private static void Wait(Task task)
        {
            while (!task.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(400);
            }
        }
    }
}