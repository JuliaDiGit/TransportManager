using System;

namespace TelemetryStorageServer
{
    public static class MessagesPrinter
    {
        public static void PrintColorMessage(object message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}