using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Services;

namespace TelemetryStorageServer
{
    /// <summary>
    ///     класс Messenger используется для работы с сообщениями
    /// </summary>
    public class Messenger
    {
        private readonly string _receivingMethod;
        private readonly HttpMessageReceiver _receiverHttp;
        private readonly TransactionalQueueMessageReceiver _receiverTransact;
        private readonly TelemetryPacketsService _service;

        public Messenger(MessageReceiver messageReceiver, TelemetryPacketsService service)
        {
            if (messageReceiver == null) throw new ArgumentNullException(nameof(messageReceiver));
            
            _service = service ?? throw new ArgumentNullException(nameof(service));

            _receivingMethod = messageReceiver.ReceivingMethod 
                               ?? throw new NullReferenceException("ReceivingMethod");
            
            _receiverHttp = messageReceiver.ReceiverHttp;
            _receiverTransact = messageReceiver.ReceiverTransact;
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

        /// <summary>
        ///     метод GetMessagesAsync получает сообщения тем способом,
        ///     который был указан при создании экземпляра класса Messenger
        /// </summary>
        private async Task<List<byte[]>> GetMessagesAsync()
        {
            Console.Write("\nОжидаем поступления сообщений.");

            try
            {
                Task<List<byte[]>> byteArraysTask;
                
                if (_receivingMethod == "Http")
                {
                    byteArraysTask = _receiverHttp.Listen(); //начинаем слушать в ожидании сообщений
                }

                else if (_receivingMethod == "TransactionalQueue")
                {
                    Task peek = _receiverTransact?.PeekMessageAsync(); //проверяем, есть ли сообщения в очереди
                    TaskWaiter.WaitAndPrintDot(peek); //ждём, когда сообщения появятся 

                    Console.Write("\nПринимаем сообщения.");
                    byteArraysTask = _receiverTransact?.ReceiveTransactMessagesAsync(); //принимаем сообщения
                }

                else throw new Exception("Неверно задан метод получения сообщений!");

                TaskWaiter.WaitAndPrintDot(byteArraysTask); //ждём, когда сообщения появятся 

                List<byte[]> byteArrays = null;
                
                if (byteArraysTask != null) byteArrays = await byteArraysTask; //получаем сообщения

                return byteArrays;
            }
            catch (Exception e)
            {
                throw new Exception($"Messenger: {e.Message}");
            }
        }
        
        /// <summary>
        ///     метод AddMessagesToDbAsync распаковывает полученные пакеты
        ///     и добавляет получившиеся объекты (сообщения) в БД
        /// </summary>
        /// <param name="byteArrays">сообщения в виде списка массивов байт</param>
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