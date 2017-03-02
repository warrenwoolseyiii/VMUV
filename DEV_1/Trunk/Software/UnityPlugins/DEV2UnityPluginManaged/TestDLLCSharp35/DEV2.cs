using UnityEngine;
using UnityEngine.VR;

namespace VMUVUnityPlugin_NET35_v100
{
    public static class DEV2
    {

        private static DEV2VRMotionFusion motionFusion = new DEV2VRMotionFusion();
        private static DEV2DataProcessor dataProcessor = new DEV2DataProcessor();

        public static void OnStart()
        {
            if (!DEV2ClientProcess.DEV2ClientHasLaunched())
                DEV2ClientProcess.StartDEV2Client();

            if (!DEV2DataConnection.ClientStreamIsConnected())
                DEV2DataConnection.StartDEV2ClientStream();
        }

        public static void OnUpdate()
        {
            RunRawDataProcessor();
            motionFusion.CalculateTranslationAndStaffe(dataProcessor.GetPads(), 
                InputTracking.GetLocalPosition(VRNode.LeftHand), InputTracking.GetLocalPosition(VRNode.RightHand));
        }

        public static void OnAppQuit()
        {
            if (DEV2ClientProcess.DEV2ClientHasLaunched())
                DEV2ClientProcess.KillDEV2Client();
        }

        public static string GetDataInString()
        {
            return DEV2DataConnection.GetDataInString();
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
            if (!DEV2ClientProcess.DEV2ClientHasLaunched())
            {
                DEV2ClientProcess.StartDEV2Client();
                return;
            }

            if (!DEV2DataConnection.ClientStreamIsConnected())
            {
                DEV2DataConnection.StartDEV2ClientStream();
                return;
            }

            dataProcessor.SetRawData(DEV2DataConnection.GetDataInCnts());
            dataProcessor.ProcessData();
        }
    }
}
