using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TransportManager.UI.Helpers
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
