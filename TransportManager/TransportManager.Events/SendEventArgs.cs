using System;

namespace TransportManager.Events
{
    public class SendEventArgs : EventArgs
    {
        public SendEventArgs(string operation, string status, int objectId)
        {
            Operation = operation;
            Status = status;
            ObjectId = objectId;
        }

        public SendEventArgs(string operation, string status)
        {
            Operation = operation;
            Status = status;
        }

        public string Operation { get; set; }
        public string Status { get; set; }
        public int ObjectId { get; set; }
    }
}