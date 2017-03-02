using System;
using System.Diagnostics;
using System.IO;

namespace VMUVUnityPlugin_NET35_v100
{
    public static class DEV2ClientProcess
    {
        private static Process clientProcess;
        private static bool clientProcessLaunched = false;
        private static string dev2ClientLocation = Path.Combine(Environment.CurrentDirectory, "Assets\\Plugins\\DEV2\\DEV_1ClientConsole");

        public static bool DEV2ClientHasLaunched()
        {
            return clientProcessLaunched;
        }

        public static void StartDEV2Client()
        {
            try
            {
                clientProcess = new Process();
                clientProcess.StartInfo.FileName = dev2ClientLocation;
                clientProcess.Start();
                clientProcessLaunched = true;
            }
            catch (Exception e0)
            {
                DEV2ExceptionHandler eHandle = new DEV2ExceptionHandler(e0);
                eHandle.TakeActionOnException();
            }
        }

        public static void KillDEV2Client()
        {
            clientProcess.Kill();
            clientProcess.Dispose();
        }
    }
}
