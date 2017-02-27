using System;

namespace VMUVUnityPlugin_NET35_v100
{
    class DEV2ExceptionHandler
    {
        protected Exception localException;

        public DEV2ExceptionHandler()
        {
            localException = new Exception();
        }

        public DEV2ExceptionHandler(Exception e)
        {
            localException = e;
        }

        public void TakeActionOnException()
        {
            // TODO:
        }
    }
}
