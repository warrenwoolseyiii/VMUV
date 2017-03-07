using System.Threading;

namespace DEV_1ClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            HardwareInterface.EnumerateDevice();
            InterprocessComms.Init();

            while (true)
            {
                InterprocessComms.ProcessNextRequest();
                Thread.Sleep(10);
            }
        }
    }
}
