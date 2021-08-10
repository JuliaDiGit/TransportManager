using System;
using System.Threading;
using System.Threading.Tasks;

namespace TelemetryStorageServer
{
    /// <summary>
    ///     класс TaskWaiter содержит методы для бесконечного вывода в консоль информации,
    ///     пока поступившая задача не будет завершена
    /// </summary>
    public static class TaskWaiter
    {
        /// <summary>
        ///     метод WaitAndPrintDot выводит на консоль символ точки,
        ///     пока ждёт завершения задачи
        /// </summary>
        /// <param name="task"></param>
        public static void WaitAndPrintDot(Task task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            
            while (!task.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(400);
            }
        }
    }
}