using System;

namespace TestDLLCSharp35
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

        public void PrintExceptionToConsole()
        {
            Console.WriteLine(localException.ToString());
        }

        public void TakeActionOnException()
        {
            PrintExceptionToConsole();
        }
    }
}
