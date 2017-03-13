using UnityEngine;
using UnityEngine.VR;
using System;
using VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific;

namespace VMUVUnityPlugin_NET35_v100
{
    public static class DEV2
    {

        private static DEV2VRMotionFusion motionFusion = new DEV2VRMotionFusion();

        public static void OnStart()
        {
            ServerProcessManager.LaunchProcess();
            InterprocessComms.Init();
        }

        public static void OnUpdate()
        {
            InterprocessComms.Service();
            CheckCriticalModules();
            //motionFusion.CalculateTranslationAndStaffe(dataProcessor.GetPads(), 
                //InputTracking.GetLocalPosition(VRNode.LeftHand), InputTracking.GetLocalPosition(VRNode.RightHand));
        }

        public static void OnAppQuit()
        {
            InterprocessComms.Disconnect();

            while (!InterprocessComms.disconnectComplete)
            {
                InterprocessComms.Service();
            }

            ServerProcessManager.KillProcess();
        }

        public static float GetTranslationFromDEV2()
        {
            return motionFusion.GetTranslation();
        }

        public static float GetStraffeFromDEV2()
        {
            return motionFusion.GetStraffe();
        }

        public static void Calibrate()
        {
            DEV2Calibrator.RunCalibration();
        }

        private static void CheckCriticalModules()
        {
            if (!ServerProcessManager.ProcessIsActive())
            {
                ServerProcessManager.LaunchProcess();
                return;
            }

            if (!PipeInterface.IsServerConnected())
            {
                InterprocessComms.Init();
                return;
            }
        }
    }
}
