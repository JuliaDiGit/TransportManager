using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Services;

namespace TelemetryStorageServer
{
    public class Messenger
    {
        private readonly string _receivingMethod;
        private readonly HttpMessageReceiver _receiverHttp;
        private readonly HttpListener _listener;
        private readonly TransactionalQueueMessageReceiver _receiverTransact;
        private readonly TelemetryPacketsService _service;

        public Messenger(string receivingMethod, 
                         HttpMessageReceiver receiverHttp, 
                         HttpListener listener, 
                         TransactionalQueueMessageReceiver receiverTransact,
                         TelemetryPacketsService service)
        {
            _receivingMethod = receivingMethod;
            _receiverHttp = receiverHttp;
            _listener = listener;
            _receiverTransact = receiverTransact;
            _service = service;
        }

        public async Task StartAsync()
        {
            while (true)
            {
                var byteArrays = await GetMessagesAsync();
                
                if (byteArrays != null)
                {
                    MessagesPrinter.PrintColorMessage("\nСообщения успешно приняты\n", ConsoleColor.Green);

                    await AddMessagesToDbAsync(byteArrays);
                }
            }
        }

        private async Task<List<byte[]>> GetMessagesAsync()
        {
            Task<List<byte[]>> byteArraysTask = null;
                
            Console.Write("\nОжидаем поступления сообщений.");
                
            if (_receivingMethod == "Http")
            {
                byteArraysTask = _receiverHttp?.Listen(_listener); //начинаем слушать в ожидании сообщений
            }

            if (_receivingMethod == "TransactionalQueue")
            {
                Task peek = _receiverTransact?.PeekMessageAsync(); //проверяем, есть ли сообщения в очереди
                TaskWaiter.Wait(peek); //ждём, когда сообщения появятся 

                Console.Write("\nПринимаем сообщения.");
                byteArraysTask = _receiverTransact?.ReceiveTransactMessagesAsync(); //принимаем сообщения
            }

            TaskWaiter.Wait(byteArraysTask); //ждём, когда сообщения появятся 

            List<byte[]> byteArrays = null;
                
            if (byteArraysTask != null) byteArrays = await byteArraysTask; //получаем сообщения

            return byteArrays;
        }
        
        private async Task AddMessagesToDbAsync(List<byte[]> byteArrays)
        {
            foreach (var byteArr in byteArrays)
            {
                var unpackTelemetry = _service.Unpack(byteArr); //распаковываем пакеты

                if (unpackTelemetry != null)
                {
                    Console.WriteLine(unpackTelemetry);
                    MessagesPrinter.PrintColorMessage("Информация успешно распакована", ConsoleColor.Green);
                }
                else
                {
                    MessagesPrinter.PrintColorMessage("Распаковка пакета не удалась!", ConsoleColor.Red);

                    return;
                }

                //цикл на случай недоступности БД, чтобы была возможность попытаться добавить данные ещё раз
                while (true)
                {
                    var telemetry = await _service.AddTelemetryPacketAsync(unpackTelemetry); //добавляем в БД

                    if (telemetry != null)
                    {
                        MessagesPrinter.PrintColorMessage("\nИнформация успешно добавлена в базу данных\n", 
                                                          ConsoleColor.Green);

                        break;
                    }

                    MessagesPrinter.PrintColorMessage("\nИнформация не добавлена в базу данных!\n", 
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
    }
}