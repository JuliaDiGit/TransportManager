using System;
using System.Threading;
using System.Threading.Tasks;

namespace TelemetryStorageServer
{
    public static class TaskWaiter
    {
        public static void Wait(Task task)
        {
            while (!task.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(400);
            }
        }
    }
}