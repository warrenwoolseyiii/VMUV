using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VMUV_TCP;

namespace VMUV_TCP_UnitTests
{
    [TestClass]
    public class PacketizerTests
    {
        Packetizer packetizer = new Packetizer();

        [TestMethod]
        public void packetizeWithNullPayload()
        {
            byte type = 0;
            short chkSumExpected = packetizer.CalculateCheckSumFromPayload(null);
            byte[] expected = {Packetizer.sync1, Packetizer.sync2, type, 0, 0,
                (byte)(chkSumExpected >> 8 & 0xFF), (byte)(chkSumExpected & 0xFF)};

            byte[] rtn = packetizer.PacketizeData(null, 0);

            Assert.AreEqual(expected.Length, rtn.Length, "Null payload length incorrect");

            for (int i = 0; i < rtn.Length; i++)
                Assert.AreEqual(expected[i], rtn[i], "Packetized null payload incorrectly");
        }
    }
}
