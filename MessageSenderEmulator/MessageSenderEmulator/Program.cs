using System.Threading.Tasks;
using UI;

namespace MessageSenderEmulator
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            await UserInterface.StartAsync();
        }
    }
}