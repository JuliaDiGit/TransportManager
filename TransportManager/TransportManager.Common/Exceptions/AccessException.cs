using System;

namespace TransportManager.Common.Exceptions
{
    public class AccessException : Exception
    {
        public override string Message { get; }

        public AccessException() : base()
        {
            Message = "Ошибка доступа";
        }
    }
}