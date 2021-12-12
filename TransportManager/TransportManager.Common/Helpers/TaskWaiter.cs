using System;
using System.Threading;
using System.Threading.Tasks;

namespace TransportManager.Common.Helpers
{
    public static class TaskWaiter
    {
        public static void WaitAndPrintDot(Task task)
        {
            while (!task.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(400);
            }
        }
    }
}
