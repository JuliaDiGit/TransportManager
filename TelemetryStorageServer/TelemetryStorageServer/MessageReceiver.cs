using System;
using System.Configuration;
using System.Net;
using Services;

namespace TelemetryStorageServer
{
    public class MessageReceiver
    {
        public string ReceivingMethod { get; private set; }
        public HttpMessageReceiver ReceiverHttp { get; private set; }
        public TransactionalQueueMessageReceiver ReceiverTransact { get; private set; }

        public MessageReceiver()
        {
            CreateReceiver();
        }
        
        /// <summary>
        ///     метод CreateReceiver определяет каким методом необходимо принимать сообщения
        ///     и создаёт соответсвующий приёмник
        /// </summary>
        private void CreateReceiver()
        {
            try
            {
                ReceivingMethod = ConfigurationManager.AppSettings.Get("ReceivingMethod");
                
                if (ReceivingMethod == "Http")
                {
                    var listener = new HttpListener();

                    listener.Prefixes.Add("http://localhost:6666/");
                    listener.Start();
                    
                    ReceiverHttp = new HttpMessageReceiver(listener);
                }
            
                else if (ReceivingMethod == "TransactionalQueue") ReceiverTransact = new TransactionalQueueMessageReceiver();
            
                else throw new Exception("Неверно задан метод получения сообщений!");
            }
            catch (Exception e)
            {
                throw new Exception($"MessageReceiver: \"{e.Message}\"");
            }
        }
    }
}