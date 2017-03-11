using UnityEngine;
using UnityEngine.VR;
using System;

namespace VMUVUnityPlugin_NET35_v100
{
    public static class DEV2
    {

        private static DEV2VRMotionFusion motionFusion = new DEV2VRMotionFusion();
        private static DEV2DataProcessor dataProcessor = new DEV2DataProcessor();

        public static void OnStart()
        {
            ServerProcessManager.LaunchProcess();
            InterprocessComms.Init();
        }

        public static void OnUpdate()
        {
            InterprocessComms.Service();
            RunRawDataProcessor();
            //motionFusion.CalculateTranslationAndStaffe(dataProcessor.GetPads(), 
                //InputTracking.GetLocalPosition(VRNode.LeftHand), InputTracking.GetLocalPosition(VRNode.RightHand));
        }

        public static void OnAppQuit()
        {
            InterprocessComms.RequestDisconnect();

            while (!InterprocessComms.IsDisconnectComplete())
            { }

            ServerProcessManager.KillProcess();
        }

        public static DEV2Pad[] GetPads()
        {
            return dataProcessor.GetPads();
        }

        public static float GetTranslationFromDEV2()
        {
            return motionFusion.GetTranslation();
        }

        public static float GetStraffeFromDEV2()
        {
            return motionFusion.GetStraffe();
        }

        private static void RunRawDataProcessor()
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

            //dataProcessor.SetRawData(DEV2DataConnection.GetDataInCnts());
            dataProcessor.ProcessData();
        }
    }
}
