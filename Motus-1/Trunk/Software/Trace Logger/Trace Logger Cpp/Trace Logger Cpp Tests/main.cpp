// main.cpp

#include "stdafx.h"

#include <iostream>
#include <stdio.h>

#include "TraceLoggerTests.h"

using namespace Trace_Logger_Cpp_UnitTests;

int main()
{
	TraceLoggerTests logger;
	logger.TestBuildTraceLoggerMessage();
	logger.TestGetBuffSize();
	logger.TestQueueMessage();
	logger.TestIsRoomInBuff();
	logger.TestMessageCount();
	logger.TestDequeueEmptyList();
	logger.TestDeQueueMessages();
	logger.TestDeQueueAllMessagesEmptyList();
	logger.TestDeQueueAllMessages();
	logger.TestQueueAndDeQueue();

	char c;
	cout << "\nall tests complete - press enter to quit...";
	c = getchar();

	return 0;
}

