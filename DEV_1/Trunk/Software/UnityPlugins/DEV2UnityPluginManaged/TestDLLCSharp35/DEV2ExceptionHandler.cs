using System;

namespace VMUVUnityPlugin_NET35_v100
{
    static class DEV2ExceptionHandler
    {
        public static void TakeActionOnException(Exception e)
        {
            Logger.LogMessage("Exception occurred!");
            Logger.LogMessage(e.StackTrace);
            Logger.LogMessage(e.Message);
        }
    }
}
