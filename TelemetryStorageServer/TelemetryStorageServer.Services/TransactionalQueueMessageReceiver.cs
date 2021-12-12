using System;
using System.Collections.Generic;
using System.Configuration;
using System.Messaging;
using System.Threading.Tasks;

namespace TelemetryStorageServer.Services
{
    public class TransactionalQueueMessageReceiver
    {
        private readonly string _path = ConfigurationManager.AppSettings.Get("TransactionalQueuePath");
        private readonly int _messagesCount;
        private readonly int _messageReceiveTimeout;

        public TransactionalQueueMessageReceiver()
        {
            int.TryParse(ConfigurationManager.AppSettings.Get("MessagesCount"), out _messagesCount);
            int.TryParse(ConfigurationManager.AppSettings.Get("MessageReceiveTimeout"), out _messageReceiveTimeout);
        }
        
        public async Task<byte[]> PeekMessageAsync()
        {
            using (var queue = new MessageQueue(_path))
            {
                if (!MessageQueue.Exists(queue.Path)) MessageQueue.Create(queue.Path, true);
                
                queue.Formatter = new XmlMessageFormatter(new[] {typeof(byte[])});

                Message message = await Task.Factory.FromAsync(queue.BeginPeek(), queue.EndPeek);

                return message.Body as byte[];
            }
        }

        public async Task<List<byte[]>> ReceiveTransactMessagesAsync()
        {
            return await Task.Run(ReceiveTransactMessages);
        }

        public List<byte[]> ReceiveTransactMessages()
        {
            var byteArrays = new List<byte[]>();

            using (var queue = new MessageQueue(_path))
            {
                queue.Formatter = new XmlMessageFormatter(new[] {typeof(byte[])});

                using (var transaction = new MessageQueueTransaction())
                {
                    try
                    {
                        transaction.Begin();

                        while (byteArrays.Count != _messagesCount)
                        {
                            var message = queue.Receive(TimeSpan.FromSeconds(_messageReceiveTimeout), transaction);

                            byteArrays.Add(message?.Body as byte[]);
                        }

                        transaction.Commit();

                        return byteArrays;
                    }
                    catch (MessageQueueException e)
                    {
                        //если превышен таймаут ожидания нового сообщения,
                        //то забираем все сообщения, что осталось в очереди
                        if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                        {
                            transaction.Commit();

                            return byteArrays;
                        }
                        
                        transaction.Abort();
                        
                        Console.WriteLine(e.Message);
                    }
                    catch (Exception e)
                    {
                        transaction.Abort();
                        
                        Console.WriteLine(e.Message);
                    }
                }
            }

            return null;
        }
    }
}