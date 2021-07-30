using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Services;

namespace TelemetryStorageServer
{
    public class MessagesGetter
    {
        public static async Task<List<byte[]>> GetMessagesAsync(string receivingMethod, 
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
                TaskWaiter.Wait(peek); //ждём, когда сообщения появятся 

                Console.Write("\nПринимаем сообщения.");
                byteArraysTask = receiverTransact?.ReceiveTransactMessagesAsync(); //принимаем сообщения
            }

            TaskWaiter.Wait(byteArraysTask); //ждём, когда сообщения появятся 

            List<byte[]> byteArrays = null;
                
            if (byteArraysTask != null) byteArrays = await byteArraysTask; //получаем сообщения

            return byteArrays;
        }
    }
}