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
                    MessagesPrinter.PrintColorMessage(e.Message, ConsoleColor.Red);
                    throw;
                }
            }

            if (receivingMethod == "TransactionalQueue") receiverTransact = new TransactionalQueueMessageReceiver();
        }
    }
}