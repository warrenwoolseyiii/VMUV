using System;
using System.Diagnostics;
using System.IO.Pipes;
using UnityEngine;
using System.IO;

namespace VMUVUnityPlugin_NET35_v100
{
    public class DEV2
    {

        private DEV2VRMotionFusion motionFusion;
        private DEV2DataConnection dataConnection;
        private DEV2ClientProcess clientProcess;

        public DEV2()
        {
            dataConnection = new DEV2DataConnection();
            clientProcess = new DEV2ClientProcess();
            motionFusion = new DEV2VRMotionFusion();
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

            motionFusion.CalculateTranslationAndStaffe(dataConnection.GetDataInCnts(), lHand, rHand);
        }

        public void OnAppQuit()
        {
            if (dataConnection.ClientStreamIsConnected())
                dataConnection.CLoseDEV2ClientStream();

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
