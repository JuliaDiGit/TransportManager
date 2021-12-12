using System;
using System.Threading.Tasks;
using MessageSenderEmulator.API;
using MessageSenderEmulator.Enums;
using MessageSenderEmulator.Models;
using MessageSenderEmulator.Services;

namespace MessageSenderEmulator.UI
{
    public static class UserInterface
    {
        public static async Task StartAsync()
        {
            while (true)
            {
                int command;

                while (true)
                {
                    PrintColorMessage("\nВыберите способ отправки сообщений:", ConsoleColor.Cyan);
                    Console.WriteLine("1. Отправить сообщения через очередь сообщений");
                    Console.WriteLine("2. Отправить сообщения по протоколу HTTP");
                    Console.WriteLine("\n0. Выход из программы");
                    Console.WriteLine();

                    Console.Write("Введите команду: ");
                    string strCommand = Console.ReadLine();
                    if (int.TryParse(strCommand, out command)) break;
                    PrintColorMessage("Некорректные данные! Введите номер команды!", ConsoleColor.Red);
                }

                if (command == 0) return;

                var sendingConditions = (SendingType)command;

                int count;

                if (command == 1)
                {
                    while (true)
                    {
                        Console.Write("\nВведите количество сообщений в одной транзакции: ");
                        string str = Console.ReadLine();
                        if (int.TryParse(str, out count) && count > 0) break;
                        PrintColorMessage("Некорректные данные! Введите целое число больше 0!", ConsoleColor.Red);
                    }
                }
                else count = 1;

                var controller = new EmulatorController(new TelemetryPacketsService());

                while (true)
                {
                    Console.WriteLine();
                    PrintWithSpaceBetween($"Для отправки сообщений ({count})", "нажмите любую кнопку");
                    PrintWithSpaceBetween("Для возврата в предыдущее меню", "нажммите Esc");
                    Console.WriteLine();
                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine();
                        break;
                    }

                    try
                    {
                        TelemetryPacketModel telemetryPacket = new TelemetryPacketModel();
                        var telemetryForSending = controller.GetTelemetryPacketsListForSending();

                        while (telemetryForSending.Count < count)
                        {
                            telemetryPacket = controller.GetOneTelemetryPacket();

                            if (telemetryPacket == null)
                            {
                                Console.WriteLine("Нет доступных сообщений!");
                                break;
                            }

                            controller.AddTelemetryToSendingList(telemetryPacket);

                            telemetryForSending = controller.GetTelemetryPacketsListForSending();
                        }

                        if (telemetryForSending.Count == count || telemetryPacket == null)
                        {
                            //если список на отправку не пустой
                            if (telemetryForSending.Count > 0)
                            {
                                var result = await controller.SendAsync(sendingConditions);
                                if (result != null)
                                {
                                    telemetryForSending.ForEach(Console.WriteLine);

                                    Console.WriteLine();
                                    PrintColorMessage(result, ConsoleColor.Green);
                                }
                                else
                                {
                                    PrintColorMessage("\nСообщения не отправлены!", ConsoleColor.Red);
                                }
                            }

                            if (telemetryPacket == null)
                            {
                                Console.WriteLine("\nCообщений нет!");
                                PrintColorMessage("Программа завершает свою работу.", ConsoleColor.Green);

                                return;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine();
                        Console.WriteLine(e.Message); 
                        PrintColorMessage("\nОшибка работы программы!", ConsoleColor.Red);

                        return;
                    }
                }
            }
        }

        private static void PrintColorMessage(object message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void PrintWithSpaceBetween(string leftValue, object rightValue)
        {
            Console.Write(leftValue.PadRight(40));
            Console.WriteLine(rightValue);
        }
    }
}