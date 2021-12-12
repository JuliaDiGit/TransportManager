using System;

namespace TransportManager.Common.Helpers
{
    public static class MessagePrinter
    {
        public static void PrintConsoleColorMessage(object message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
