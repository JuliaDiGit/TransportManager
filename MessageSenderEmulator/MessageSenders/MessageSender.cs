using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enums;

namespace MessageSenders
{
    public class MessageSender
    {
        public async Task<string> SendAsync(SendingType sendingType, List<byte[]> byteArrays)
        {
            if (byteArrays == null) throw new ArgumentNullException(nameof(byteArrays));
            if (byteArrays.Count == 0) return null;

            if (sendingType == SendingType.TransactionalQueue)
            {
                var sender = new TransactionalQueueSender();
                return sender.Send(byteArrays);
            }
            
            if (sendingType == SendingType.Http)
            {
                var sender = new HttpSender();
                return await sender.SendAsync(byteArrays);
            }

            return null;
        }
    }
}