using System.Diagnostics;
using System;
using Trace_Logger_CSharp;

namespace Motus_Unity_Plugin
{
    class ServerApp
    {
        public const string fname = "C:/Program Files (x86)/VMUV LLC/Motus/Server App CSharp.exe";
        public const string appName = "Server App CSharp";
        private TraceLogger _traceLogger = new TraceLogger();
        private string _moduleName = "ServerApp";

        public void LaunchProcess(string name)
        {
            string methodName = "LaunchProcess";
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = name;
                process.Start();
            }
            catch (ArgumentNullException e0)
            {
                _traceLogger.QueueMessage(_traceLogger.BuildMessage(_moduleName, methodName, e0.Message));
            }
            catch (InvalidOperationException e1)
            {
                _traceLogger.QueueMessage(_traceLogger.BuildMessage(_moduleName, methodName, e1.Message));
            }
            catch (System.ComponentModel.Win32Exception e2)
            {
                _traceLogger.QueueMessage(_traceLogger.BuildMessage(_moduleName, methodName, e2.Message));
            }
        }

        public TraceLoggerMessage[] GetTraceLoggerMessages()
        {
            return _traceLogger.GetAllMessages();
        }

        public bool HasTraceLoggerMessages()
        {
            return _traceLogger.HasMessages();
        }
    }
}
