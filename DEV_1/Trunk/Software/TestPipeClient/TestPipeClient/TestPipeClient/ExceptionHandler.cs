using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV_1ClientConsole
{
    class ExceptionHandler
    {
        protected Exception localException;

        public ExceptionHandler()
        {
            localException = new Exception();
        }

        public ExceptionHandler(Exception e)
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
