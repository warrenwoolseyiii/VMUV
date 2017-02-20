using System;
using System.Collections;
using System.Diagnostics;

namespace TestPipeClient
{
    class Program
    {
        private static DEV_1 dev1 = new DEV_1();

        static void Main(string[] args)
        {
            try
            {
                Process myProcess = new Process();
                myProcess.StartInfo.FileName = 
                    "C:\\Users\\Warren Woolsey\\Repositories\\VMUV\\VMUV\\DEV_1\\Trunk\\Software\\DEV_1ClientConsole\\DEV_1ClientConsole\\DEV_1ClientConsole\\bin\\Release\\DEV_1ClientConsole";
                myProcess.Start();
            }
            catch (Exception e0)
            {

            }

            dev1.ConnectToClientService();
            Console.WriteLine("Connection success!\n");
            dev1.ReadAsync();

            while (true)
            {
                if (dev1.IsAsyncReadComplete())
                {
                    dev1.ReadAsync();
                    Console.WriteLine(dev1.GetDataInString());
                }
            }
        }
    }
}
