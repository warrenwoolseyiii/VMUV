﻿using System.Threading;
using VMUVUnityPlugin_NET35_v100;

namespace TestScript
{
    class Program
    {
        static void Main(string[] args)
        {
            int cnt = 0;

            DEV2.OnStart();

            while(true)
            {
                cnt++;
                Thread.Sleep(15);
                DEV2.OnUpdate();
                DEV2.Calibrate();
            }
        }
    }
}
