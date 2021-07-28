using System;

namespace Exceptions
{
    public class SourceNotAvailableException : Exception
    {
        public override string Message { get; }

        public SourceNotAvailableException() : base()
        {
            Message = "Ошибка работы базы данных";
        }
    }
}
