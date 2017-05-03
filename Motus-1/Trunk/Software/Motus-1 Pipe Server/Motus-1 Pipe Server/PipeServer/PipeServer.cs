using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using Motus_1_Pipe_Server.Logging;
using Motus_1_Pipe_Server.DataStorage;

namespace Motus_1_Pipe_Server.PipeServer
{
    class PipeServer
    {
        private static int maximumAllowedConnections = 5;
        private static string key = "For other rappers, the jealous ass crackers, and the police!";
        private static TraceLogger traceLogger = new TraceLogger(64);
        private static string moduleName = "PipeServer.cs";

        public static int GetMaximumAllowedConnections()
        {
            return maximumAllowedConnections;
        }

        public static bool HasTraceMessages()
        {
            return traceLogger.HasMessages();
        }
        
        public static TraceLoggerMessage[] GetTraceMessages()
        {
            return traceLogger.GetAllMessages();
        }

        public static void ServerThread(object data)
        {
            NamedPipeServerStream pipeServer = new NamedPipeServerStream("Motus-1", PipeDirection.InOut, maximumAllowedConnections);
            int threadId = Thread.CurrentThread.ManagedThreadId;
            string methodName = "ServerThread";

            pipeServer.WaitForConnection();

            try
            {
                StreamString ss = new StreamString(pipeServer);
                ss.WriteString(key);

                if (ss.ReadString() == "Ack")
                {
                    ss.WriteBytes(DataStorageTable.GetCurrentData());
                }
            }
            catch (IOException e0)
            {
                string message = "An exception of type " + e0.GetType().ToString() + " occurred." +
                    " Exception occurred at : " + Environment.StackTrace + ". Exception message is : " + e0.Message;
                traceLogger.QueueMessage(traceLogger.BuildMessage(moduleName, methodName, message));
            }
            catch (Exception e1)
            {
                string message = "An exception of type " + e1.GetType().ToString() + " occurred." +
                    " Exception occurred at : " + Environment.StackTrace + ". Exception message is : " + e1.Message;
                traceLogger.QueueMessage(traceLogger.BuildMessage(moduleName, methodName, message));
            }

            pipeServer.Close();
        }
    }
}
