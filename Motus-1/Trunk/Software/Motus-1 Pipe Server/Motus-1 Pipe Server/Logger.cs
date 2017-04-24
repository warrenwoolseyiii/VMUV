using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motus_1_Pipe_Server
{
    static class Logger
    {
        private static string currentDir = System.IO.Directory.GetCurrentDirectory();
        private static string fileName = "log.txt";
        private static string path = System.IO.Path.Combine(currentDir, fileName);
        private static bool logFileCreated = false;

        public static void CreateLogFile()
        {
            logFileCreated = false;

            try
            {
                string[] str = {"*** New log file created ***"};
                System.IO.File.WriteAllLines(path, str);
                logFileCreated = true;
            }
            catch (System.IO.IOException e0)
            {
                // Don't print this exception because you don't have a log file to print it to..
            }
            catch (Exception e1)
            {
                // Don't print this exception because you don't have a log file to print it to..
            }
        }

        public static void LogMessage(string msg)
        {
            if (!logFileCreated)
                return;

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                try
                {
                    file.WriteLine(msg);
                }
                catch (Exception e0)
                {
                    // Don't print this exception because you don't have a log file to print it to..
                }
            }
        }
    }
}
