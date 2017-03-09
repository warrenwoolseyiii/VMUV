using System;

namespace DEV_1ClientConsole
{
    static class ExceptionHandler
    { 
        public static void TakeActionOnException(Exception localException)
        {
            Logger.LogMessage(localException.StackTrace);
            Logger.LogMessage(localException.Message);
        }
    }
}
