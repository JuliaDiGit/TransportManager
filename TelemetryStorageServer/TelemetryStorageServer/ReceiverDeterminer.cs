using System;
using System.Net;
using Services;

namespace TelemetryStorageServer
{
    public static class ReceiverDeterminer
    {
        public static void Determine(string receivingMethod, 
                                      out HttpMessageReceiver receiverHttp, 
                                      out HttpListener listener, 
                                      out TransactionalQueueMessageReceiver receiverTransact)
        {
            if (receivingMethod == null) 
                throw new Exception("ReceiverDeterminer: \"Не задан метод получения сообщений!\"");
            
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
                    throw new Exception($"HttpListener: \"{e.Message}\"");
                }
            }
            
            else if (receivingMethod == "TransactionalQueue") receiverTransact = new TransactionalQueueMessageReceiver();
            
            else throw new Exception("ReceiverDeterminer: \"Неверно задан метод получения сообщений!\"");
        }
    }
}