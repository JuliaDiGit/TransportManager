using System;

namespace TelemetryStorageServer
{
    /// <summary>
    ///     класс MessagesPrinter содержит методы для форматированного вывода на консоль
    /// </summary>
    public static class MessagesPrinter
    {
        /// <summary>
        ///     метод PrintColorMessage выводит информацию конкретным цветом
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void PrintColorMessage(object message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}