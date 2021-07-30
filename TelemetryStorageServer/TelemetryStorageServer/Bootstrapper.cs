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
    public class Bootstrapper
    {

        private HttpMessageReceiver _receiverHttp;
        private HttpListener _listener;
        private TransactionalQueueMessageReceiver _receiverTransact;
        private TelemetryPacketsService _service;
            
            
        public async Task Initialize()
        {
            InputParamsValidator.Validate(out var receivingMethod);

            try
            {
                DbInitialization.Start();
            }
            catch (Exception)
            {
                Console.ReadKey();
                return;
            }
            
            ReceiverDeterminer.Determine(receivingMethod, 
                                         out _receiverHttp, 
                                         out _listener, 
                                         out _receiverTransact);

            _service = new TelemetryPacketsService(new TelemetryPacketsRepository());

            while (true)
            {
                var byteArrays = await MessagesGetter.GetMessagesAsync(receivingMethod, 
                                                                _receiverHttp, 
                                                                _listener, 
                                                                _receiverTransact);
                
                if (byteArrays != null)
                {
                    MessagesPrinter.PrintColorMessage("\nСообщения успешно приняты\n", ConsoleColor.Green);

                    await AddMessagesToDbAsync(_service, byteArrays);
                }
            }
        }

        private static async Task AddMessagesToDbAsync(TelemetryPacketsService service, List<byte[]> byteArrays)
        {
            foreach (var byteArr in byteArrays)
            {
                var unpackTelemetry = service.Unpack(byteArr); //распаковываем пакеты

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
                    var telemetry = await service.AddTelemetryPacketAsync(unpackTelemetry); //добавляем в БД

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