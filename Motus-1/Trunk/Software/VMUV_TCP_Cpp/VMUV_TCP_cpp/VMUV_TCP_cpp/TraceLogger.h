#pragma once
#include <string>

namespace VMUV_TCP
{
#define DEFAULT_BUFF_SIZE   32

	struct TraceLoggerMessage
	{
	public:	
		std::string moduleName;
		std::string methodName;
		std::string message;
	};

	class TraceLogger
	{
	private:
		unsigned int buffSize, buffHead, buffTail;
		TraceLoggerMessage* msgBuff;

	public:
		///constructors
		TraceLogger();
		TraceLogger(unsigned int bufferSize);

		///methods
		unsigned int GetBuffSize();
		int GetNumMessagesQueued();
		bool HasMessages();
		bool IsRoomInBuff();
		bool QueueMessage(TraceLoggerMessage msg);
		TraceLoggerMessage DeQueueMessage();
		TraceLoggerMessage BuildMessage(std::string modName, std::string method, std::string msg);
		void GetAllMessages(TraceLoggerMessage* alllllllMessages);
		static std::string TraceLoggerMessageToString(TraceLoggerMessage msg);


	};
}