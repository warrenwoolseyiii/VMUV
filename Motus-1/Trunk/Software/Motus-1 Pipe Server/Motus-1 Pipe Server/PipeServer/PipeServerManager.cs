using System;
using System.Threading;
using Motus_1_Pipe_Server.Logging;

namespace Motus_1_Pipe_Server.PipeServer
{
    static class PipeServerManager
    {
        public static bool requestExit = false;
        public static TraceLogger pipeServerLogger = new TraceLogger(128);
        private static string moduleName = "PipeServerManager.cs"; 

        public static void ServerManagerThread(object data)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            int maxServers = PipeServer.GetMaximumAllowedConnections();
            Thread[] servers = new Thread[maxServers];
            bool exit = false;
            string methodName = "ServerManagerThread";

            for (int i = 0; i < maxServers; i++)
            {
                servers[i] = new Thread(PipeServer.ServerThread);
                servers[i].Start();
            }

            pipeServerLogger.QueueMessage(pipeServerLogger.BuildMessage(moduleName, methodName, "Launched " + maxServers.ToString() + " server threads"));

            while (!exit)
            {
                for (int j = 0; j < maxServers; j++)
                {
                    if ((servers[j] == null) && (requestExit == false))
                    {
                        try
                        {
                            servers[j] = new Thread(PipeServer.ServerThread);
                            servers[j].Start();
                        }
                        catch(ArgumentNullException e0)
                        {
                            servers[j] = null;
                            string message = "An exception of type " + e0.GetType().ToString() + " occurred." +
                                " Exception occurred at : " + Environment.StackTrace + ". Exception message is : " + e0.Message;
                            pipeServerLogger.QueueMessage(pipeServerLogger.BuildMessage(moduleName, methodName, message));
                        }
                        catch(ThreadStateException e1)
                        {
                            servers[j] = null;
                            string message = "An exception of type " + e1.GetType().ToString() + " occurred." +
                                " Exception occurred at : " + Environment.StackTrace + ". Exception message is : " + e1.Message;
                            pipeServerLogger.QueueMessage(pipeServerLogger.BuildMessage(moduleName, methodName, message));
                        }
                        catch (OutOfMemoryException e2)
                        {
                            servers[j] = null;
                            string message = "An exception of type " + e2.GetType().ToString() + " occurred." +
                                " Exception occurred at : " + Environment.StackTrace + ". Exception message is : " + e2.Message;
                            pipeServerLogger.QueueMessage(pipeServerLogger.BuildMessage(moduleName, methodName, message));
                        }
                    }
                    else if (!servers[j].IsAlive)
                    {
                        servers[j] = null;
                    }
                    else if (requestExit)
                    {
                        foreach (Thread element in servers)
                        {
                            if (element != null)
                                break;
                        }

                        exit = true;
                    }
                }

                if (PipeServer.HasTraceMessages())
                    QueueTraceLoggerMessages();

                Thread.Sleep(5);
            }
        }

        public static TraceLoggerMessage[] GetTraceMessages()
        {
            return pipeServerLogger.GetAllMessages();
        }

        public static bool HasTraceMessages()
        {
            return pipeServerLogger.HasMessages();
        }

        private static void QueueTraceLoggerMessages()
        {
            TraceLoggerMessage[] msgs = PipeServer.GetTraceMessages();

            for (int i = 0; i < msgs.Length; i++)
                pipeServerLogger.QueueMessage(msgs[i]);
        }
    }
}
