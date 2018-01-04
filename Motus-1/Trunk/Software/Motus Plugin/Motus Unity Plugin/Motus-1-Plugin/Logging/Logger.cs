using System;

namespace Motus_Unity_Plugin.Logging
{
    static class Logger
    {
        private static string currentDir = System.IO.Directory.GetCurrentDirectory();
        private static string logFileName = "vmuv_log.txt";
        private static string logFilePath = System.IO.Path.Combine(currentDir, logFileName);
        private static bool logFileCreated = false;

        public static void CreateLogFile()
        {
            logFileCreated = false;

            try
            {
                string[] str = { "*** New log file created ***" };
                System.IO.File.WriteAllLines(logFilePath, str);
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

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(logFilePath, true))
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

        public static void LogMessage(string[] msgs)
        {
            string msg = "";

            if (!logFileCreated)
                return;

            foreach (string element in msgs)
            {
                msg += element;
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(logFilePath, true))
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
