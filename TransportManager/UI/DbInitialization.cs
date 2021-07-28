using System;
using System.Threading;
using System.Threading.Tasks;
using DataEF;

namespace UI
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
                Wait(taskMainDb);
                Wait(taskStatisticsDb);

                if (taskMainDb.Exception != null)
                {
                    foreach (var e in taskMainDb.Exception.InnerExceptions)
                    {
                        throw e;
                    }
                }
                
                Console.WriteLine();
                PrintColorMessage("\nОсновная база данных: Связь установлена.", ConsoleColor.Green);

                if (taskStatisticsDb.Exception != null)
                {
                    Console.WriteLine();
                    foreach (var e in taskStatisticsDb.Exception.InnerExceptions)
                    {
                        throw e;
                    }
                }
                
                PrintColorMessage("База данных телеметрии: Связь установлена.", ConsoleColor.Green);
            }
            catch (Exception e)
            {
                PrintColorMessage($"Ошибка инициализации БД: {e.Message}", ConsoleColor.Red);

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

        private static void PrintColorMessage(object message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}