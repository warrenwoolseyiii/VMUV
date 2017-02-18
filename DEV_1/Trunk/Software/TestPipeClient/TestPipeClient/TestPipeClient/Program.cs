using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPipeClient
{
    class Program
    {
        private static DEV_1 dev1 = new DEV_1();

        static void Main(string[] args)
        {
            dev1.ConnectToClientService();
            Console.WriteLine("Connection success!\n");

            while (true)
            {
                dev1.ReadPacket();
                if (dev1.IsPacketValid())
                    Console.WriteLine(dev1.GetDataInString());
            }
        }
    }
}
