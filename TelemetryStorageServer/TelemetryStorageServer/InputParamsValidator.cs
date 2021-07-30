using System;
using System.Configuration;

namespace TelemetryStorageServer
{
    public static class InputParamsValidator
    {
        /// <summary>
        ///     метод Validate проверяет верно ли заданы параметры в App.config
        /// </summary>
        /// <param name="receivingMethod">способ получения сообщений</param>
        public static void Validate(out string receivingMethod)
        {
            string path = null;
            int messagesCount = default;
            int messageReceiveTimeout = default;
            receivingMethod = ConfigurationManager.AppSettings.Get("ReceivingMethod");

            if (receivingMethod != "TransactionalQueue" && receivingMethod != "Http")
            { 
                MessagesPrinter.PrintColorMessage("Неверно задан параметр \"ReceivingMethod\"!", ConsoleColor.Red);

                return;
            }
            
            if (receivingMethod == "TransactionalQueue")
            {
                path = ConfigurationManager.AppSettings.Get("TransactionalQueuePath");

                //количество сообщений в транзакции
                var messagesCountStr = ConfigurationManager.AppSettings.Get("MessagesCount");
                if (!int.TryParse(messagesCountStr, out messagesCount) || messagesCount <= 0)
                {
                    MessagesPrinter.PrintColorMessage("Неверно задан параметр \"MessagesCount\"!", ConsoleColor.Red);

                    return;
                }

                //максимальное время ожидания нового сообщения в секундах
                var messageReceiveTimeoutStr = ConfigurationManager.AppSettings.Get("MessageReceiveTimeout");
                if (!int.TryParse(messageReceiveTimeoutStr, out messageReceiveTimeout) || messageReceiveTimeout <= 0)
                {
                    MessagesPrinter.PrintColorMessage("Неверно задан параметр \"MessageReceiveTimeout\"!", ConsoleColor.Red);

                    return;
                }
            }

            MessagesPrinter.PrintColorMessage("\nВходные параметры", ConsoleColor.DarkYellow);
            Console.WriteLine("ReceivingMethod: " + receivingMethod);
            if (receivingMethod == "TransactionalQueue")
            {
                Console.WriteLine("TransactionalQueuePath: " + path);
                Console.WriteLine("MessagesCount: " + messagesCount);
                Console.WriteLine("MessageReceiveTimeout: " + messageReceiveTimeout);
            }
        }
    }
}