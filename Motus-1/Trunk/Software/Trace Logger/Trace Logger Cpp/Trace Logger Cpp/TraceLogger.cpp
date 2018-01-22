// TraceLogger.cpp

#include "stdafx.h"

#include "TraceLogger.h"


Trace_Logger_Cpp::TraceLogger::TraceLogger()
{
	_maxListSize = 32;
	_numMessagesQueued = 0;
}

Trace_Logger_Cpp::TraceLogger::TraceLogger(int maximumListSize)
{
	_numMessagesQueued = 0;
	_maxListSize = maximumListSize;

	// STL lists dont throw exceptions other than possibly out of memory
	// so no need to catch them here
	//try
	//{
	//	messageList = new list<TraceLoggerMessage>(_maxListSize);
	//}
	//catch (ArgumentOutOfRangeException e0)
	//{
	//	messageList = new list<TraceLoggerMessage>(32);
	//}
}

Trace_Logger_Cpp::TraceLogger::~TraceLogger()
{
}

// note: does not take ownership of msg - rather it makes a copy
bool Trace_Logger_Cpp::TraceLogger::QueueMessage(Trace_Logger_Cpp::TraceLoggerMessage msg)
{
	if (!IsRoomInBuff())
		return false;

	// inserts a copy of msg at list begenning
	messageList.push_back(msg);
	_numMessagesQueued++;
	return true;
}

// I am guessing that the return struct is somehow automatically destroyed by the calling function
// after being copied to a struct local to that function or ignored
Trace_Logger_Cpp::TraceLoggerMessage Trace_Logger_Cpp::TraceLogger::DeQueueMessage()
{
	TraceLoggerMessage rtn;
	if (_numMessagesQueued > 0)
	{
		--_numMessagesQueued;
		rtn = messageList.back();
		messageList.pop_back();
		return rtn;
	}
	//throw new IndexOutOfRangeException();
	throw INDEXOUTOFRANGEEXCEPTION;
}

// I am guessing that the return struct is somehow automatically destroyed by the calling function
// after being copied to a struct local to that function or ignored
Trace_Logger_Cpp::TraceLoggerMessage Trace_Logger_Cpp::TraceLogger::BuildMessage(string modName, string method, string msg)
{
	TraceLoggerMessage rtn;
	{
		rtn.moduleName = modName;
		rtn.methodName = method;
		rtn.message = msg;
	};

	return rtn;
}

// for now this function will return a copy of the entire list. I will revisit this
// if it turns out that we really do need the messages to be in the form of an array
//Trace_Logger_Cpp::TraceLoggerMessage* Trace_Logger_Cpp::TraceLogger::GetAllMessages()
list<Trace_Logger_Cpp::TraceLoggerMessage> Trace_Logger_Cpp::TraceLogger::GetAllMessages()
{
	//TraceLoggerMessage* rtn = messageList->ToArray();
	//try
	//{
	//	if (_numMessagesQueued > 0)
	//	{
	//		for (int i = _numMessagesQueued - 1; i >= 0; i--)
	//			messageList.RemoveAt(i);
	//	}
	//}
	//catch (ArgumentOutOfRangeException e0)
	//{
	//	// force the list to remove everything
	//	messageList.TrimExcess();
	//	for (int i = 0; i < messageList.Capacity; i++)
	//		messageList.RemoveAt(i);

	//	messageList.Capacity = _maxListSize;
	//}
	list<Trace_Logger_Cpp::TraceLoggerMessage> rtn = messageList;
	messageList.clear();

	_numMessagesQueued = 0;
	return rtn;
}

string Trace_Logger_Cpp::TraceLogger::TraceLoggerMessageToString(Trace_Logger_Cpp::TraceLoggerMessage msg)
{
	return (msg.moduleName + ": " + msg.methodName + ": " + msg.message);
}

