using System.Threading;
using UnityEngine;
using System;

namespace Motus_1_Plugin
{
#if DEBUG
    class Program
    {
        static void Main(string[] args)
        {
            PluginInterface.Initialize();

            while (true)
            {
                PluginInterface.Service();
                Vector3 nib = PluginInterface.GetXZVector();
                Console.WriteLine(nib.x.ToString() + "," + nib.z.ToString());
                Thread.Sleep(1);
            }
        }
    }
#endif
}
