using System;
using System.Net;
using Services;

namespace TelemetryStorageServer
{
    public static class MessageReceiverInstaller
    {
        /// <summary>
        ///     метод InstallReceiver подгатавливает к работе приёмник сообщений 
        /// </summary>
        /// <param name="receivingMethod">способ получения сообщений</param>
        /// <param name="receiverHttp"></param>
        /// <param name="listener"></param>
        /// <param name="receiverTransact"></param>
        public static void InstallReceiver(string receivingMethod, 
                                           out HttpMessageReceiver receiverHttp, 
                                           out HttpListener listener, 
                                           out TransactionalQueueMessageReceiver receiverTransact)
        {
            if (receivingMethod == null) 
                throw new Exception("MessageReceiverInstaller: \"Не задан метод получения сообщений!\"");
            
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
                    throw new Exception($"MessageReceiverInstaller/HttpListener: \"{e.Message}\"");
                }
            }
            
            else if (receivingMethod == "TransactionalQueue") receiverTransact = new TransactionalQueueMessageReceiver();
            
            else throw new Exception("MessageReceiverInstaller: \"Неверно задан метод получения сообщений!\"");
        }
    }
}