#include "TraceLogger.h"
#include <stdlib.h>

VMUV_TCP::TraceLogger::TraceLogger()
{
	buffHead = buffTail = 0;
	buffSize = DEFAULT_BUFF_SIZE;
	msgBuff = (TraceLoggerMessage *)malloc(sizeof(TraceLoggerMessage) * buffSize);
}

VMUV_TCP::TraceLogger::TraceLogger(unsigned int bufferSize)
{
	buffHead = buffTail = 0;
	buffSize = bufferSize;
	msgBuff = (TraceLoggerMessage *)malloc(sizeof(TraceLoggerMessage) * buffSize);
}

unsigned int VMUV_TCP::TraceLogger::GetBuffSize()
{
	return buffSize;
}

int VMUV_TCP::TraceLogger::GetNumMessagesQueued()
{
	unsigned int head = buffHead % buffSize;
	unsigned int tail = buffTail % buffSize;

	if (head < tail)
		head += buffSize;

	return ((int)head - (int)tail);
}

bool VMUV_TCP::TraceLogger::HasMessages()
{
	return(GetNumMessagesQueued() > 0);
}

bool VMUV_TCP::TraceLogger::IsRoomInBuff()
{
	int numMsgsQueued = GetNumMessagesQueued();
	return ((numMsgsQueued >= 0) && (numMsgsQueued < buffSize));
}

bool VMUV_TCP::TraceLogger::QueueMessage(TraceLoggerMessage msg)
{
	if (!IsRoomInBuff())
		return false;

	msgBuff[buffHead % buffSize] = msg;
	buffHead++;

	return true;
}

VMUV_TCP::TraceLoggerMessage VMUV_TCP::TraceLogger::DeQueueMessage()
{
	TraceLoggerMessage rtn = msgBuff[buffTail % buffSize];
	buffTail++;

	return rtn;
}

VMUV_TCP::TraceLoggerMessage VMUV_TCP::TraceLogger::BuildMessage(std::string modName, std::string method, std::string msg)
{
	TraceLoggerMessage rtn = TraceLoggerMessage();

	rtn.moduleName = modName;
	rtn.methodName = method;
	rtn.message = msg;
	
	return rtn;
}

void VMUV_TCP::TraceLogger::GetAllMessages(TraceLoggerMessage * alllllllMessages)
{
	int size = GetNumMessagesQueued();

    // wipe the buffer if the size is crazy
    if (size > buffSize)
    {
        buffTail = buffHead;
        return;
    }

    if (alllllllMessages == nullptr)
        return;

	for (int i = 0; i < size; i++)
		alllllllMessages[i] = DeQueueMessage();
}

std::string VMUV_TCP::TraceLogger::TraceLoggerMessageToString(TraceLoggerMessage msg)
{
	return (msg.moduleName + ": " + msg.methodName + ": " + msg.message);
}


/* Calling GetAllMessages properly 
{
    if (tLogger.HasMessages()
    {
        int numMsg = tLogger.GetNumMessagesQueued();
        TraceLoggerMessage * ptr = (TraceLoggerMessage *)malloc(sizeof(TraceLoggerMessage) * numMsg);

        tLogger.GetAllMessages(ptr);

        ** do something with the messages here...

        free((void *)ptr);
    }
}*/