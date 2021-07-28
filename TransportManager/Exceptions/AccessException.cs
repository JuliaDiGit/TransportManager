using System;

namespace Exceptions
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