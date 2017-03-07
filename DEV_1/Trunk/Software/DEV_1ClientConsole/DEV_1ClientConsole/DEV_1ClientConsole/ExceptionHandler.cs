using System;

namespace DEV_1ClientConsole
{
    static class ExceptionHandler
    { 
        public static void PrintExceptionToConsole(Exception localException)
        {
            Console.WriteLine(localException.ToString());
        }

        public static void TakeActionOnException(Exception localException)
        {
            PrintExceptionToConsole(localException);
        }
    }
}
