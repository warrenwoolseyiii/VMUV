using System.Threading;
using System;
using VMUVUnityPlugin_NET35_v100;

namespace TestScript
{
    class Program
    {
        static void Main(string[] args)
        {
            DEV2.OnStart();

            while(true)
            {
                Thread.Sleep(5);
            }
        }
    }
}
