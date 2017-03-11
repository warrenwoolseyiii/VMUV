﻿using System;
using System.Diagnostics;
using System.IO;

namespace VMUVUnityPlugin_NET35_v100
{
    static class ServerProcessManager
    {
        private static Process serverProcess;
        private static string processLocation = Path.Combine(Environment.CurrentDirectory, "Assets\\Plugins\\DEV2\\DEV_1ClientConsole");
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
                }
                catch (Exception e0)
                {
                    DEV2ExceptionHandler.TakeActionOnException(e0);
                }
            }
        }

        public static void KillProcess()
        {
            if (processIsActive)
            {
                killRequest = true;
                serverProcess.Kill();
                serverProcess.Dispose();
                processIsActive = false;
                killRequest = false;
            }
        }
    }
}