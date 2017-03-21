using VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific;
using VMUVUnityPlugin_NET35_v100.Motion;

namespace VMUVUnityPlugin_NET35_v100
{
    public static class DEV2
    {

        private static DEV2VRMotionFusion motionFusion = new DEV2VRMotionFusion();

        public static void OnStart()
        {
            Logger.LogMessage("Attempting to start..");
            ServerProcessManager.LaunchProcess();
            InterprocessComms.Init();
            CurrentValueTable.SetCalibrationTermsOnStart(DEV2Calibrator.ReadCalibrationFile());
        }

        public static void OnUpdate()
        {
            InterprocessComms.Service();
            CheckCriticalModules();
            StandardWalkRun.CalculateTranslationAndStraffe();
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

        public static void Calibrate()
        {
            if (!DEV2Calibrator.initialized)
                DEV2Calibrator.Init();

            DEV2Calibrator.RunCalibration();
        }

        public static float GetTranslation()
        {
            return StandardWalkRun.GetTranslation();
        }

        public static float GetStraffe()
        {
            return StandardWalkRun.GetStraffe();
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
