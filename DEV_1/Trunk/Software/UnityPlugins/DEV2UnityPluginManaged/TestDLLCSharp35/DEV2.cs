using UnityEngine;

namespace VMUVUnityPlugin_NET35_v100
{
    public class DEV2
    {

        private DEV2VRMotionFusion motionFusion;
        private DEV2DataConnection dataConnection;
        private DEV2ClientProcess clientProcess;
        private DEV2DataProcessor dataProcessor;

        public DEV2()
        {
            dataConnection = new DEV2DataConnection();
            clientProcess = new DEV2ClientProcess();
            motionFusion = new DEV2VRMotionFusion();
            dataProcessor = new DEV2DataProcessor();
        }

        public void OnStart()
        {
            clientProcess.StartDEV2Client();

            if (clientProcess.DEV2ClientHasLaunched())
                dataConnection.StartDEV2ClientStream();
        }

        public void OnUpdate(Vector3 lHand, Vector3 rHand)
        {
            if (!clientProcess.DEV2ClientHasLaunched())
                clientProcess.StartDEV2Client();

            if (!dataConnection.ClientStreamIsConnected())
                dataConnection.StartDEV2ClientStream();

            dataProcessor.SetRawData(dataConnection.GetDataInCnts());
            dataProcessor.ProcessData();

            motionFusion.CalculateTranslationAndStaffe(dataProcessor.GetPads(), lHand, rHand);
        }

        public void OnAppQuit()
        {
            if (clientProcess.DEV2ClientHasLaunched())
                clientProcess.KillDEV2Client();
        }

        public string GetDataInString()
        {
            return dataConnection.GetDataInString();
        }

        public float GetTranslationFromDEV2()
        {
            return motionFusion.GetTranslation();
        }

        public float GetStraffeFromDEV2()
        {
            return motionFusion.GetStraffe();
        }
    }
}
