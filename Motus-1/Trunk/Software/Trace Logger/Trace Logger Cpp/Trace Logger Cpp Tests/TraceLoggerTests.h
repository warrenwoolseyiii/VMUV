// TraceLoggerTests.h

#pragma once

#include <string>

using namespace std;

namespace Trace_Logger_Cpp_UnitTests
{
	class TraceLoggerTests
	{
	public:
		string moduleName = "TraceLoggerUnitTests";
		string methodName = "methodTest";
		string message = "this is a test";

		void TestBuildTraceLoggerMessage();
		void TestGetBuffSize();
		void TestQueueMessage();
		void TestIsRoomInBuff();
		void TestMessageCount();
		void TestDequeueEmptyList();
		void TestDeQueueMessages();
		void TestDeQueueAllMessagesEmptyList();
		void TestDeQueueAllMessages();
		void TestQueueAndDeQueue();
	};
}
