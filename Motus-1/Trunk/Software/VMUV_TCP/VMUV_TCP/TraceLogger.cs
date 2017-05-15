
namespace VMUV_TCP
{
    class TraceLogger
    {
        private uint buffSize, buffHead, buffTail;
        private TraceLoggerMessage[] msgBuff;

        public TraceLogger()
        {
            buffHead = buffTail = 0;
            buffSize = 32;
            msgBuff = new TraceLoggerMessage[buffSize];
        }

        public TraceLogger(uint bufferSize)
        {
            buffHead = buffTail = 0;
            buffSize = bufferSize;
            msgBuff = new TraceLoggerMessage[buffSize];
        }

        public uint GetBuffSize()
        {
            return buffSize;
        }

        public int GetNumMessagesQueued()
        {
            uint head = buffHead % buffSize;
            uint tail = buffTail % buffSize;

            if (head < tail)
                head += buffSize;

            return ((int)head - (int)tail);
        }

        public bool HasMessages()
        {
            return (GetNumMessagesQueued() > 0);
        }

        private bool IsRoomInBuff()
        {
            int numMsgsQueued = GetNumMessagesQueued();
            return ((numMsgsQueued >= 0) && (numMsgsQueued < buffSize));
        }

        public bool QueueMessage(TraceLoggerMessage msg)
        {
            if (!IsRoomInBuff())
                return false;

            msgBuff[buffHead % buffSize] = msg;
            buffHead++;

            return true;
        }

        public TraceLoggerMessage DeQueueMessage()
        {
            TraceLoggerMessage rtn = msgBuff[buffTail % buffSize];
            buffTail++;

            return rtn;
        }

        public TraceLoggerMessage BuildMessage(string modName, string method, string msg)
        {
            TraceLoggerMessage rtn = new TraceLoggerMessage();

            rtn.moduleName = modName;
            rtn.methodName = method;
            rtn.message = msg;

            return rtn;
        }

        public TraceLoggerMessage[] GetAllMessages()
        {
            int size = GetNumMessagesQueued();
            TraceLoggerMessage[] rtn = new TraceLoggerMessage[size];

            for (int i = 0; i < size; i++)
                rtn[i] = DeQueueMessage();

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
