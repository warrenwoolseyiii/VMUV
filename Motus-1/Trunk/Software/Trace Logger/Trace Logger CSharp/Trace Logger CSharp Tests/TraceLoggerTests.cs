using System;
using Trace_Logger_CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VMUV_TCP_UnitTests
{
    [TestClass]
    public class TraceLoggerTests
    {
        string moduleName = "TraceLoggerUnitTests";
        string methodName = "methodTest";
        string message = "this is a test";

        // No need for invalid test because the compiler won't let you send an 
        // invalid struct as a parameter

        [TestMethod]
        public void TestBuildTraceLoggerMessage()
        {
            TraceLogger logger = new TraceLogger();
            TraceLoggerMessage msg = logger.BuildMessage(moduleName, methodName, message);
            Assert.AreEqual(moduleName, msg.moduleName);
            Assert.AreEqual(methodName, msg.methodName);
            Assert.AreEqual(message, msg.message);
        }

        [TestMethod]
        public void TestGetBuffSize()
        {
            int buffSize = 128;
            TraceLogger logger = new TraceLogger(buffSize);
            Assert.AreEqual(logger.GetBuffSize(), buffSize);
        }

        [TestMethod]
        public void TestQueueMessage()
        {
            TraceLogger logger = new TraceLogger();
            TraceLoggerMessage msg = logger.BuildMessage(moduleName, methodName, message);
            
            // buffer maximum amount of messages
            for (int i = 0; i < logger.GetBuffSize(); i++)
                Assert.AreEqual(true, logger.QueueMessage(msg));

            Assert.AreEqual(logger.GetNumMessagesQueued(), logger.GetBuffSize());
            Assert.AreEqual(false, logger.QueueMessage(msg));
        }

        [TestMethod]
        public void TestIsRoomInBuff()
        {
            TraceLogger logger = new TraceLogger();
            TraceLoggerMessage msg = logger.BuildMessage(moduleName, methodName, message);

            while (logger.IsRoomInBuff())
                Assert.AreEqual(true, logger.QueueMessage(msg));

            Assert.AreEqual(logger.GetNumMessagesQueued(), logger.GetBuffSize());
        }

        [TestMethod]
        public void TestMessageCount()
        {
            TraceLogger logger = new TraceLogger();
            TraceLoggerMessage msg = logger.BuildMessage(moduleName, methodName, message);

            Assert.AreEqual(logger.HasMessages(), false);
            Assert.AreEqual(logger.GetNumMessagesQueued(), 0);
            Assert.AreEqual(true, logger.QueueMessage(msg));
            Assert.AreEqual(logger.HasMessages(), true);
            Assert.AreEqual(logger.GetNumMessagesQueued(), 1);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException), "Attempted to dequeue an empty list")]
        public void TestDequeueEmptyList()
        {
            TraceLogger logger = new TraceLogger();
            logger.DeQueueMessage();
        }

        [TestMethod]
        public void TestDeQueueMessages()
        {
            TraceLogger logger = new TraceLogger();
            TraceLoggerMessage msg = logger.BuildMessage(moduleName, methodName, message);

            while (logger.IsRoomInBuff())
                Assert.AreEqual(true, logger.QueueMessage(msg));

            while (logger.HasMessages())
            {
                TraceLoggerMessage rtn = logger.DeQueueMessage();
                Assert.AreEqual(rtn.moduleName, msg.moduleName);
                Assert.AreEqual(rtn.methodName, msg.methodName);
                Assert.AreEqual(rtn.message, msg.message);
            }

            Assert.AreEqual(logger.GetNumMessagesQueued(), 0);
        }

        [TestMethod]
        public void TestDeQueueAllMessagesEmptyList()
        {
            TraceLogger logger = new TraceLogger();
            TraceLoggerMessage[] all = logger.GetAllMessages();
            Assert.AreEqual(all.Length, 0);
        }

        [TestMethod]
        public void TestDeQueueAllMessages()
        {
            TraceLogger logger = new TraceLogger();
            TraceLoggerMessage msg = logger.BuildMessage(moduleName, methodName, message);

            while (logger.IsRoomInBuff())
                Assert.AreEqual(true, logger.QueueMessage(msg));

            TraceLoggerMessage[] rtn = logger.GetAllMessages();
            Assert.AreEqual(rtn.Length, logger.GetBuffSize());
            Assert.AreEqual(logger.GetNumMessagesQueued(), 0);

            foreach (TraceLoggerMessage item in rtn)
            {
                Assert.AreEqual(item.moduleName, msg.moduleName);
                Assert.AreEqual(item.methodName, msg.methodName);
                Assert.AreEqual(item.message, msg.message);
            }
        }

        [TestMethod]
        public void TestQueueAndDeQueue()
        {
            TraceLogger logger = new TraceLogger();
            TraceLoggerMessage msg = logger.BuildMessage(moduleName, methodName, message);

            while (logger.IsRoomInBuff())
                Assert.AreEqual(true, logger.QueueMessage(msg));

            // get the last message 
            TraceLoggerMessage rtn = logger.DeQueueMessage();
            Assert.AreEqual(rtn.moduleName, msg.moduleName);
            Assert.AreEqual(rtn.methodName, msg.methodName);
            Assert.AreEqual(rtn.message, msg.message);

            // requeue the last message and make sure the values are good
            msg.message = "im gay";
            msg.methodName = "you are gay";
            msg.moduleName = "everyone is gay";
            Assert.AreEqual(true, logger.QueueMessage(msg));
            rtn = logger.DeQueueMessage();
            Assert.AreEqual(rtn.moduleName, msg.moduleName);
            Assert.AreEqual(rtn.methodName, msg.methodName);
            Assert.AreEqual(rtn.message, msg.message);

            // get the rest and assure they are good
            TraceLoggerMessage[] rtnBuf = logger.GetAllMessages();
            Assert.AreEqual(rtnBuf.Length, logger.GetBuffSize() - 1);

            msg = logger.BuildMessage(moduleName, methodName, message);
            foreach (TraceLoggerMessage item in rtnBuf)
            {
                Assert.AreEqual(item.moduleName, msg.moduleName);
                Assert.AreEqual(item.methodName, msg.methodName);
                Assert.AreEqual(item.message, msg.message);
            }

            // fill it up and dump it one more time
            while (logger.IsRoomInBuff())
                Assert.AreEqual(true, logger.QueueMessage(msg));
            Assert.AreEqual(logger.GetBuffSize(), logger.GetNumMessagesQueued());
            logger.GetAllMessages();
            Assert.AreEqual(logger.GetNumMessagesQueued(), 0);
        }
    }
}
