using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            while (true)
            {
                if ((deviceMngr.IsDeviceEnumerated()) && (!comms.ClientIsConnected()))
                {
                    comms.WaitForClientConnect();
                    Console.WriteLine("Client Connected\n");
                }
            }
        }

        private static async Task EnumerateDevice()
        {
            await deviceMngr.EnumerateDEV_1();
        }
    }
}
