using System;
using System.Diagnostics;
using System.IO;

namespace VMUVUnityPlugin_NET35_v100
{
    class DEV2ClientProcess
    {
        private Process clientProcess;
        private bool clientProcessLaunched;
        private string dev2ClientLocation;

        public DEV2ClientProcess()
        {
            clientProcessLaunched = false;
            dev2ClientLocation = Path.Combine(Environment.CurrentDirectory, "Assets\\Plugins\\DEV2\\DEV_1ClientConsole");
        }

        public bool DEV2ClientHasLaunched()
        {
            return clientProcessLaunched;
        }

        public void StartDEV2Client()
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

        public void KillDEV2Client()
        {
            clientProcess.Kill();
            clientProcess.Dispose();
        }
    }
}
