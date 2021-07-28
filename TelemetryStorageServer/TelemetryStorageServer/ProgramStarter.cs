using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Services;

namespace TelemetryStorageServer
{
    public static class ProgramStarter
    { 
        public static async Task StartAsync()
        {
            ValidateInputParams(out var receivingMethod);
            
            try
            {
                DbInitialization.Start();
            }
            catch (Exception)
            {
                Console.ReadKey();
                return;
            }
            
            DetermineReceiver(receivingMethod, 
                              out var receiverHttp, 
                              out var listener, 
                              out var receiverTransact);

            var service = new TelemetryPacketsService(new TelemetryPacketsRepository());

            while (true)
            {
                var byteArrays = await GetMessagesAsync(receivingMethod, 
                                                        receiverHttp, 
                                                        listener, 
                                                        receiverTransact);
                
                if (byteArrays != null)
                {
                    PrintColorMessage("\nСообщения успешно приняты\n", ConsoleColor.Green);

                    await AddMessagesToDbAsync(service, byteArrays);
                }
            }
        }

        /// <summary>
        ///     метод ValidateInputParams проверяет верно ли заданы параметры в App.config
        /// </summary>
        /// <param name="receivingMethod">способ получения сообщений</param>
        private static void ValidateInputParams(out string receivingMethod)
        {
            string path = null;
            int messagesCount = default;
            int messageReceiveTimeout = default;
            receivingMethod = ConfigurationManager.AppSettings.Get("ReceivingMethod");

            if (receivingMethod != "TransactionalQueue" && receivingMethod != "Http")
            { 
                PrintColorMessage("Неверно задан параметр \"ReceivingMethod\"!", ConsoleColor.Red);

                return;
            }
            
            if (receivingMethod == "TransactionalQueue")
            {
                path = ConfigurationManager.AppSettings.Get("TransactionalQueuePath");

                //количество сообщений в транзакции
                var messagesCountStr = ConfigurationManager.AppSettings.Get("MessagesCount");
                if (!int.TryParse(messagesCountStr, out messagesCount) || messagesCount <= 0)
                {
                    PrintColorMessage("Неверно задан параметр \"MessagesCount\"!", ConsoleColor.Red);

                    return;
                }

                //максимальное время ожидания нового сообщения в секундах
                var messageReceiveTimeoutStr = ConfigurationManager.AppSettings.Get("MessageReceiveTimeout");
                if (!int.TryParse(messageReceiveTimeoutStr, out messageReceiveTimeout) || messageReceiveTimeout <= 0)
                {
                    PrintColorMessage("Неверно задан параметр \"MessageReceiveTimeout\"!", ConsoleColor.Red);

                    return;
                }
            }

            PrintColorMessage("\nВходные параметры", ConsoleColor.DarkYellow);
            Console.WriteLine("ReceivingMethod: " + receivingMethod);
            if (receivingMethod == "TransactionalQueue")
            {
                Console.WriteLine("TransactionalQueuePath: " + path);
                Console.WriteLine("MessagesCount: " + messagesCount);
                Console.WriteLine("MessageReceiveTimeout: " + messageReceiveTimeout);
            }
        }

        private static void DetermineReceiver(string receivingMethod, 
                                              out HttpMessageReceiver receiverHttp, 
                                              out HttpListener listener, 
                                              out TransactionalQueueMessageReceiver receiverTransact)
        {
            receiverHttp = null;
            listener = null;
            receiverTransact = null;
            
            if (receivingMethod == "Http")
            {
                receiverHttp = new HttpMessageReceiver();
                listener = new HttpListener();

                try
                {
                    listener.Prefixes.Add("http://localhost:6666/");
                    listener.Start();
                }
                catch (Exception e)
                {
                    PrintColorMessage(e.Message, ConsoleColor.Red);
                    throw;
                }
            }

            if (receivingMethod == "TransactionalQueue") receiverTransact = new TransactionalQueueMessageReceiver();
        }

        private static async Task<List<byte[]>> GetMessagesAsync(string receivingMethod, 
                                                                 HttpMessageReceiver receiverHttp, 
                                                                 HttpListener listener, 
                                                                 TransactionalQueueMessageReceiver receiverTransact)
        {
            Task<List<byte[]>> byteArraysTask = null;
                
            Console.Write("\nОжидаем поступления сообщений.");
                
            if (receivingMethod == "Http")
            {
                byteArraysTask = receiverHttp?.Listen(listener); //начинаем слушать в ожидании сообщений
            }

            if (receivingMethod == "TransactionalQueue")
            {
                Task peek = receiverTransact?.PeekMessageAsync(); //проверяем, есть ли сообщения в очереди
                Wait(peek); //ждём, когда сообщения появятся 

                Console.Write("\nПринимаем сообщения.");
                byteArraysTask = receiverTransact?.ReceiveTransactMessagesAsync(); //принимаем сообщения
            }

            Wait(byteArraysTask); //ждём, когда сообщения появятся 

            List<byte[]> byteArrays = null;
                
            if (byteArraysTask != null) byteArrays = await byteArraysTask; //получаем сообщения

            return byteArrays;
        }

        private static async Task AddMessagesToDbAsync(TelemetryPacketsService service, List<byte[]> byteArrays)
        {
            foreach (var byteArr in byteArrays)
            {
                var unpackTelemetry = service.Unpack(byteArr); //распаковываем пакеты

                if (unpackTelemetry != null)
                {
                    Console.WriteLine(unpackTelemetry);
                    PrintColorMessage("Информация успешно распакована", ConsoleColor.Green);
                }
                else
                {
                    PrintColorMessage("Распаковка пакета не удалась!", ConsoleColor.Red);

                    return;
                }

                //цикл на случай недоступности БД, чтобы была возможность попытаться добавить данные ещё раз
                while (true)
                {
                    var telemetry = await service.AddTelemetryPacketAsync(unpackTelemetry); //добавляем в БД

                    if (telemetry != null)
                    {
                        PrintColorMessage("\nИнформация успешно добавлена в базу данных\n", 
                                          ConsoleColor.Green);

                        break;
                    }

                    PrintColorMessage("\nИнформация не добавлена в базу данных!\n", 
                                      ConsoleColor.Red);

                    Console.Write("Повторная попытка через       ");
                    for (int i = 5; i > 0; i--)
                    {
                        Console.SetCursorPosition(Console.CursorLeft - 6, Console.CursorTop);
                        Console.Write(i + " сек.");
                        Thread.Sleep(1000);
                    }

                    Console.WriteLine();
                }
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