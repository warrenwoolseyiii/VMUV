// TraceLogger.h

#pragma once

#include <list>
#include <string>

#define INDEXOUTOFRANGEEXCEPTION 1

using namespace std;

namespace Trace_Logger_Cpp
{
	struct TraceLoggerMessage
	{
		string moduleName;
		string methodName;
		string message;
	};

	class TraceLogger
	{
	private:
		list<TraceLoggerMessage> messageList;
		int _numMessagesQueued;
		int _maxListSize;

	public:
		TraceLogger();
		TraceLogger(int maximumListSize);
		virtual ~TraceLogger();

		int GetNumMessagesQueued()
		{
			return _numMessagesQueued;
		}

		int GetBuffSize()
		{
			return _maxListSize;
		}

		bool HasMessages()
		{
			return (_numMessagesQueued > 0);
		}

		bool IsRoomInBuff()
		{
			return _numMessagesQueued < _maxListSize;
		}

		bool QueueMessage(TraceLoggerMessage msg);

		TraceLoggerMessage DeQueueMessage();

		TraceLoggerMessage BuildMessage(string modName, string method, string msg);

		list<Trace_Logger_Cpp::TraceLoggerMessage> Trace_Logger_Cpp::TraceLogger::GetAllMessages();

		static string TraceLoggerMessageToString(TraceLoggerMessage msg);
	};

} // namespace
