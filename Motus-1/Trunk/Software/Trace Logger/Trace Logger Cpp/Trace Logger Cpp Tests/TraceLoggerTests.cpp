// TraceLoggerTests.cpp

#include "stdafx.h"

#include <iostream>

#include "Common.h"

#include "..\Trace Logger Cpp\TraceLogger.h"
#include "TraceLoggerTests.h"


using namespace Trace_Logger_Cpp;

void Trace_Logger_Cpp_UnitTests::TraceLoggerTests::TestBuildTraceLoggerMessage()
{
	TraceLogger logger;
	TraceLoggerMessage msg = logger.BuildMessage(moduleName, methodName, message);
	AssertAreEqualStrings(moduleName, msg.moduleName, "TraceLoggerTests::TestBuildTraceLoggerMessage()", 1);
	AssertAreEqualStrings(methodName, msg.methodName, "TraceLoggerTests::TestBuildTraceLoggerMessage()", 2);
	AssertAreEqualStrings(message, msg.message, "TraceLoggerTests::TestBuildTraceLoggerMessage()", 3);
}

void Trace_Logger_Cpp_UnitTests::TraceLoggerTests::TestGetBuffSize()
{
	int buffSize = 128;
	TraceLogger logger(buffSize);
	AssertAreEqual(logger.GetBuffSize(), buffSize, "TraceLoggerTests::TestGetBuffSize()", 1);
}

void Trace_Logger_Cpp_UnitTests::TraceLoggerTests::TestQueueMessage()
{
	TraceLogger logger;
	TraceLoggerMessage msg = logger.BuildMessage(moduleName, methodName, message);

	// buffer maximum amount of messages
	for (int i = 0; i < logger.GetBuffSize(); i++) {
		AssertAreEqual(true, logger.QueueMessage(msg), "TraceLoggerTests::TestQueueMessage()", 1);
	}

	AssertAreEqual(logger.GetNumMessagesQueued(), logger.GetBuffSize(), "TraceLoggerTests::TestQueueMessage()", 2);
	AssertAreEqual(false, logger.QueueMessage(msg), "TraceLoggerTests::TestQueueMessage()", 3);
}

void Trace_Logger_Cpp_UnitTests::TraceLoggerTests::TestIsRoomInBuff()
{
	TraceLogger logger;
	TraceLoggerMessage msg = logger.BuildMessage(moduleName, methodName, message);

	while (logger.IsRoomInBuff()) {
		AssertAreEqual(true, logger.QueueMessage(msg), "TraceLoggerTests::TestIsRoomInBuff()", 1);
	}

	AssertAreEqual(logger.GetNumMessagesQueued(), logger.GetBuffSize(), "TraceLoggerTests::TestIsRoomInBuff()", 2);
}

void Trace_Logger_Cpp_UnitTests::TraceLoggerTests::TestMessageCount()
{
	TraceLogger logger;
	TraceLoggerMessage msg = logger.BuildMessage(moduleName, methodName, message);

	AssertAreEqual(logger.HasMessages(), false, "TraceLoggerTests::TestMessageCount()", 1);
	AssertAreEqual(logger.GetNumMessagesQueued(), 0, "TraceLoggerTests::TestMessageCount()", 2);
	AssertAreEqual(true, logger.QueueMessage(msg), "TraceLoggerTests::TestMessageCount()", 3);
	AssertAreEqual(logger.HasMessages(), true, "TraceLoggerTests::TestMessageCount()", 4);
	AssertAreEqual(logger.GetNumMessagesQueued(), 1, "TraceLoggerTests::TestMessageCount()", 5);
}

void Trace_Logger_Cpp_UnitTests::TraceLoggerTests::TestDequeueEmptyList()
{
	TraceLogger logger;
	try {
		logger.DeQueueMessage();
	}
	catch (int) {
		cout << "unit test TraceLoggerTests::TestDequeueEmptyList() correctly threw exception\n";
		return;
	}
	cout << "unit test TraceLoggerTests::TestDequeueEmptyList() FAILED to throw exception\n";
}

void Trace_Logger_Cpp_UnitTests::TraceLoggerTests::TestDeQueueMessages()
{
	TraceLogger logger;
	TraceLoggerMessage msg = logger.BuildMessage(moduleName, methodName, message);

	while (logger.IsRoomInBuff()) {
		AssertAreEqual(true, logger.QueueMessage(msg), "TraceLoggerTests::TestDeQueueMessages()", 1);
	}

	while (logger.HasMessages()) {
		TraceLoggerMessage rtn = logger.DeQueueMessage();
		AssertAreEqualStrings(rtn.moduleName, msg.moduleName, "TraceLoggerTests::TestDeQueueMessages()", 2);
		AssertAreEqualStrings(rtn.methodName, msg.methodName, "TraceLoggerTests::TestDeQueueMessages()", 3);
		AssertAreEqualStrings(rtn.message, msg.message, "TraceLoggerTests::TestDeQueueMessages()", 4);
	}

	AssertAreEqual(logger.GetNumMessagesQueued(), 0, "TraceLoggerTests::TestDeQueueMessages()", 5);
}

void Trace_Logger_Cpp_UnitTests::TraceLoggerTests::TestDeQueueAllMessagesEmptyList()
{
	TraceLogger logger;
	list<Trace_Logger_Cpp::TraceLoggerMessage> all = logger.GetAllMessages();
	AssertAreEqual(all.size(), 0, "TraceLoggerTests::TestDeQueueAllMessagesEmptyList()", 1);
}
void Trace_Logger_Cpp_UnitTests::TraceLoggerTests::TestDeQueueAllMessages()
{
	TraceLogger logger;
	TraceLoggerMessage msg = logger.BuildMessage(moduleName, methodName, message);

	while (logger.IsRoomInBuff()) {
		AssertAreEqual(true, logger.QueueMessage(msg), "TraceLoggerTests::TestDeQueueAllMessages()", 1);
	}

	list<Trace_Logger_Cpp::TraceLoggerMessage> rtn = logger.GetAllMessages();
	AssertAreEqual(rtn.size(), logger.GetBuffSize(), "TraceLoggerTests::TestDeQueueAllMessages()", 2);
	AssertAreEqual(logger.GetNumMessagesQueued(), 0, "TraceLoggerTests::TestDeQueueAllMessages()", 3);

	list<Trace_Logger_Cpp::TraceLoggerMessage>::iterator itr;
	for (itr = rtn.begin(); itr != rtn.end(); ++itr) {
		Trace_Logger_Cpp::TraceLoggerMessage item = *itr;

		AssertAreEqualStrings(item.moduleName, msg.moduleName, "TraceLoggerTests::TestDeQueueAllMessages()", 4);
		AssertAreEqualStrings(item.methodName, msg.methodName, "TraceLoggerTests::TestDeQueueAllMessages()", 5);
		AssertAreEqualStrings(item.message, msg.message, "TraceLoggerTests::TestDeQueueAllMessages()", 6);
	}
}
void Trace_Logger_Cpp_UnitTests::TraceLoggerTests::TestQueueAndDeQueue()
{
	TraceLogger logger;
	TraceLoggerMessage msg = logger.BuildMessage(moduleName, methodName, message);

	while (logger.IsRoomInBuff()) {
		AssertAreEqual(true, logger.QueueMessage(msg), "TraceLoggerTests::TestQueueAndDeQueue()", 1);
	}

	// get the last message 
	TraceLoggerMessage rtn = logger.DeQueueMessage();
	AssertAreEqualStrings(rtn.moduleName, msg.moduleName, "TraceLoggerTests::TestQueueAndDeQueue()", 2);
	AssertAreEqualStrings(rtn.methodName, msg.methodName, "TraceLoggerTests::TestQueueAndDeQueue()", 3);
	AssertAreEqualStrings(rtn.message, msg.message, "TraceLoggerTests::TestQueueAndDeQueue()", 4);

	// requeue the last message and make sure the values are good
	msg.message = "im gay";
	msg.methodName = "you are gay";
	msg.moduleName = "everyone is gay";
	AssertAreEqual(true, logger.QueueMessage(msg), "TraceLoggerTests::TestQueueAndDeQueue()", 5);
	rtn = logger.DeQueueMessage();
	AssertAreEqualStrings(rtn.moduleName, msg.moduleName, "TraceLoggerTests::TestQueueAndDeQueue()", 6);
	AssertAreEqualStrings(rtn.methodName, msg.methodName, "TraceLoggerTests::TestQueueAndDeQueue()", 7);
	AssertAreEqualStrings(rtn.message, msg.message, "TraceLoggerTests::TestQueueAndDeQueue()", 8);

	// get the rest and assure they are good
	list<Trace_Logger_Cpp::TraceLoggerMessage> rtnBuf = logger.GetAllMessages();
	AssertAreEqual(rtnBuf.size(), logger.GetBuffSize() - 1, "TraceLoggerTests::TestQueueAndDeQueue()", 9);

	msg = logger.BuildMessage(moduleName, methodName, message);

	list<Trace_Logger_Cpp::TraceLoggerMessage>::iterator itr;
	for (itr = rtnBuf.begin(); itr != rtnBuf.end(); ++itr) {
		Trace_Logger_Cpp::TraceLoggerMessage item = *itr;

		AssertAreEqualStrings(item.moduleName, msg.moduleName, "TraceLoggerTests::TestQueueAndDeQueue()", 10);
		AssertAreEqualStrings(item.methodName, msg.methodName, "TraceLoggerTests::TestQueueAndDeQueue()", 11);
		AssertAreEqualStrings(item.message, msg.message, "TraceLoggerTests::TestQueueAndDeQueue()", 12);
	}

	// fill it up and dump it one more time
	while (logger.IsRoomInBuff()) {
		AssertAreEqual(true, logger.QueueMessage(msg), "TraceLoggerTests::TestQueueAndDeQueue()", 13);
	}
	AssertAreEqual(logger.GetBuffSize(), logger.GetNumMessagesQueued(), "TraceLoggerTests::TestQueueAndDeQueue()", 14);
	logger.GetAllMessages();
	AssertAreEqual(logger.GetNumMessagesQueued(), 0, "TraceLoggerTests::TestQueueAndDeQueue()", 15);
}


