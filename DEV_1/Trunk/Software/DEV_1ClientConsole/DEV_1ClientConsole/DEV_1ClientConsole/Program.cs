using System;
using System.Threading;
using System.Threading.Tasks;

namespace DEV_1ClientConsole
{
    class Program
    {
        private static InterprocessComms comms = new InterprocessComms();
        private static DeviceManager deviceMngr = new DeviceManager(comms);

        static void Main(string[] args)
        {
            EnumerateDevice();
            comms.InitializePipe();

            while (!comms.IsPipeBroken())
            {
                Thread.Sleep(50);
            }

            Console.WriteLine("Pipe is broken!\n");
        }

        private static async Task EnumerateDevice()
        {
            await deviceMngr.EnumerateDEV_1();
        }
    }
}
