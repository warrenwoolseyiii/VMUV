
using System;
using System.Collections.Generic;

namespace Trace_Logger_CSharp
{
    public class TraceLogger
    {
        private List<TraceLoggerMessage> messageList;
        private int _numMessagesQueued;
        private int _maxListSize = 32;

        public TraceLogger()
        {
            messageList = new List<TraceLoggerMessage>(_maxListSize);
            _numMessagesQueued = 0;
        }

        public TraceLogger(int maximumListSize)
        {
            _numMessagesQueued = 0;
            _maxListSize = maximumListSize;

            try
            {
                messageList = new List<TraceLoggerMessage>(_maxListSize);
            }
            catch (ArgumentOutOfRangeException e0)
            {
                messageList = new List<TraceLoggerMessage>(32);
            }
        }

        public int GetNumMessagesQueued()
        {
            return _numMessagesQueued;
        }

        public int GetBuffSize()
        {
            return messageList.Capacity;
        }

        public bool HasMessages()
        {
            return (_numMessagesQueued > 0);
        }

        public bool IsRoomInBuff()
        {
            return _numMessagesQueued < messageList.Capacity;
        }

        public bool QueueMessage(TraceLoggerMessage msg)
        {
            if (!IsRoomInBuff())
                return false;

            messageList.Add(msg);
            _numMessagesQueued++;
            return true;
        }

        public TraceLoggerMessage DeQueueMessage()
        {
            TraceLoggerMessage rtn;
            if (_numMessagesQueued > 0)
            {
                rtn = messageList[--_numMessagesQueued];
                messageList.RemoveAt(_numMessagesQueued);
                return rtn;
            }
            throw new IndexOutOfRangeException();
        }

        public TraceLoggerMessage BuildMessage(string modName, string method, string msg)
        {
            TraceLoggerMessage rtn = new TraceLoggerMessage
            {
                moduleName = modName,
                methodName = method,
                message = msg
            };

            return rtn;
        }

        public TraceLoggerMessage[] GetAllMessages()
        {
            TraceLoggerMessage[] rtn = messageList.ToArray();
            try
            {
                if (_numMessagesQueued > 0)
                {
                    for (int i = _numMessagesQueued - 1; i >= 0; i--)
                        messageList.RemoveAt(i);
                }
            }
            catch (ArgumentOutOfRangeException e0)
            {
                // force the list to remove everything
                messageList.TrimExcess();
                for (int i = 0; i < messageList.Capacity; i++)
                    messageList.RemoveAt(i);

                messageList.Capacity = _maxListSize;
            }

            _numMessagesQueued = 0;
            return rtn;
        }

        public static string TraceLoggerMessageToString(TraceLoggerMessage msg)
        {
            return (msg.moduleName + ": " + msg.methodName + ": " + msg.message);
        }
    }

    public struct TraceLoggerMessage
    {
        public string moduleName;
        public string methodName;
        public string message;
    }
}
