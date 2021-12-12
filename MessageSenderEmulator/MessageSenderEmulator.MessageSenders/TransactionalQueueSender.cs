using System;
using System.Collections.Generic;
using System.Configuration;
using System.Messaging;
using MessageSenderEmulator.Domain;

namespace MessageSenderEmulator.MessageSenders
{
    public class TransactionalQueueSender
    {
        public string Send(List<byte[]> byteArrays)
        {
            if (byteArrays == null) throw new ArgumentNullException(nameof(byteArrays));
            if (byteArrays.Count == 0) return null;

            string transactionalQueuePath = ConfigurationManager.AppSettings.Get("TransactionalQueuePath");
            
            using (var queue = new MessageQueue(transactionalQueuePath))
            {
                if (!MessageQueue.Exists(queue.Path)) MessageQueue.Create(queue.Path, true);

                queue.Formatter = new XmlMessageFormatter(new[]{typeof(TelemetryPacket)});

                using (var transaction = new MessageQueueTransaction())
                {
                    try
                    { 
                        transaction.Begin();

                        byteArrays.ForEach(arr => queue.Send(arr, transaction));

                        transaction.Commit();
                        
                        return Resources.SendingMessages_Success;
                    }
                    catch (Exception e)
                    {
                        transaction.Abort();

                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e.Message);
                        Console.ResetColor();
                        
                        return null;
                    }
                }
            }
        }
    }
}