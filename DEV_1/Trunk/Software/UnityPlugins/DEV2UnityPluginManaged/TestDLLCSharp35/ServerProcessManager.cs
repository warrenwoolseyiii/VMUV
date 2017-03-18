using System;
using System.Diagnostics;

namespace VMUVUnityPlugin_NET35_v100
{
    static class ServerProcessManager
    {
        private static Process serverProcess;
        private static string processLocation = "C:\\Program Files (x86)\\VMUV\\DEV_1ClientConsole";
            //Path.Combine(Environment.CurrentDirectory, "Assets\\Plugins\\DEV2\\DEV_1ClientConsole");
            //"C://Users//Warren Woolsey//Repositories//VRDemos//VRDemos//Trunk//Unity//Unity Plugins//DEV2//DEV_1ClientConsole";
        private static bool processIsActive = false;
        private static bool killRequest = false;

        public static bool ProcessIsActive()
        {
            return processIsActive;
        }

        public static void LaunchProcess()
        {
            if (!processIsActive && !killRequest)
            {
                try
                {
                    serverProcess = new Process();
                    serverProcess.StartInfo.FileName = processLocation;
                    serverProcess.Start();
                    processIsActive = true;
                    Logger.LogMessage("Process launch successful!");
                }
                catch (Exception e0)
                {
                    DEV2ExceptionHandler.TakeActionOnException(e0);
                }
            }
            else
            {
                Logger.LogMessage("Process launch was requested but not attempted");
            }
        }

        public static void KillProcess()
        {
            if (processIsActive)
            {
                try
                {
                    killRequest = true;
                    serverProcess.Kill();
                    serverProcess.Dispose();
                    processIsActive = false;
                    killRequest = false;
                    Logger.LogMessage("Process ended successfully");
                }
                catch (Exception e)
                {
                    DEV2ExceptionHandler.TakeActionOnException(e);
                }
            }
            else
            {
                Logger.LogMessage("Process kill was requested but not attempted");
            }
        }
    }
}
