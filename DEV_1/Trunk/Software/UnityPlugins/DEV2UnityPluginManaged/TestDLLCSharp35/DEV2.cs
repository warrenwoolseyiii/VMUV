using UnityEngine;

namespace VMUVUnityPlugin_NET35_v100
{
    public class DEV2
    {

        private DEV2VRMotionFusion motionFusion;
        private DEV2DataConnection dataConnection;
        private DEV2ClientProcess clientProcess;
        private DEV2DataProcessor dataProcessor;
        private DEV2Calibration calibrator;
        private DEV2Pad[] pads;

        public DEV2()
        {
            pads = new DEV2Pad[9];
            for (int i = 0; i < pads.Length; i++)
                pads[i] = new DEV2Pad();

            dataConnection = new DEV2DataConnection();
            clientProcess = new DEV2ClientProcess();
            motionFusion = new DEV2VRMotionFusion();
            dataProcessor = new DEV2DataProcessor(pads);
            calibrator = new DEV2Calibration(pads);
        }

        public void OnStart()
        {
            clientProcess.StartDEV2Client();

            if (clientProcess.DEV2ClientHasLaunched())
                dataConnection.StartDEV2ClientStream();
        }

        public void OnUpdate(Vector3 lHand, Vector3 rHand)
        {
            RunRawDataProcessor();
            motionFusion.CalculateTranslationAndStaffe(dataProcessor.GetPads(), lHand, rHand);
        }

        public void CalibratePad(Vector3 point)
        {
            RunRawDataProcessor();
            calibrator.CalibratePad(point);
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

        private void RunRawDataProcessor()
        {
            if (!clientProcess.DEV2ClientHasLaunched())
            {
                clientProcess.StartDEV2Client();
                return;
            }

            if (!dataConnection.ClientStreamIsConnected())
            {
                dataConnection.StartDEV2ClientStream();
                return;
            }

            dataProcessor.SetRawData(dataConnection.GetDataInCnts());
            dataProcessor.ProcessData();
        }
    }
}
