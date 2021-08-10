using System;
using System.Configuration;

namespace TelemetryStorageServer
{
    public static class InputParamsValidator
    {
        /// <summary>
        ///     метод Validate проверяет верно ли заданы параметры в App.config
        /// </summary>
        public static void Validate()
        {
            string path = null;
            int messagesCount = default;
            int messageReceiveTimeout = default;

            try
            {
                var receivingMethod = ConfigurationManager.AppSettings.Get("ReceivingMethod");

                if (receivingMethod != "TransactionalQueue" && receivingMethod != "Http")
                {
                    throw new Exception("Неверно задан параметр \"ReceivingMethod\"!");
                }

                if (receivingMethod == "TransactionalQueue")
                {
                    path = ConfigurationManager.AppSettings.Get("TransactionalQueuePath");

                    //количество сообщений в транзакции
                    var messagesCountStr = ConfigurationManager.AppSettings.Get("MessagesCount");
                    if (!int.TryParse(messagesCountStr, out messagesCount) || messagesCount <= 0)
                    {
                        throw new Exception("Неверно задан параметр \"MessagesCount\"!");
                    }

                    //максимальное время ожидания нового сообщения в секундах
                    var messageReceiveTimeoutStr = ConfigurationManager.AppSettings.Get("MessageReceiveTimeout");
                    if (!int.TryParse(messageReceiveTimeoutStr, out messageReceiveTimeout) ||
                        messageReceiveTimeout <= 0)
                    {
                        throw new Exception("Неверно задан параметр \"MessageReceiveTimeout\"!");
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
            catch (Exception e)
            {
                throw new Exception($"InputParamsValidator: {e.Message}");
            }
        }
    }
}