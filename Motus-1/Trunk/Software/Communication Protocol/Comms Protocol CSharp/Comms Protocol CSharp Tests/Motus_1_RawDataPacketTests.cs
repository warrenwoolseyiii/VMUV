using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Comms_Protocol_CSharp;

namespace Comms_Protocol_CSharp_Tests
{
    [TestClass]
    public class Motus_1_RawDataPacketTests
    {
        [TestMethod]
        public void Motus_1_PacketTestConstructor()
        {
            Motus_1_RawDataPacket packet = new Motus_1_RawDataPacket();
            Assert.AreEqual(ValidPacketTypes.motus_1_raw_data_packet,
                packet.Type);
            Assert.AreEqual(18, packet.ExpectedLen);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Accepted illegal payload")]
        public void Motus_1_PacketTestSerializeInvalidPayload()
        {
            Motus_1_RawDataPacket packet = new Motus_1_RawDataPacket();
            byte[] invalidBytePayload = new byte[12];
            packet.Serialize(invalidBytePayload);
        }

        [TestMethod]
        public void Motus_1_PacketTestSerializeBytes()
        {
            Motus_1_RawDataPacket packet = new Motus_1_RawDataPacket();
            byte[] payload = new byte[packet.ExpectedLen];
            Int16[] intPayload = new Int16[packet.ExpectedLen / 2];

            int j = 0;
            for (byte i = 0; i < packet.ExpectedLen; i++)
            {
                if ((i % 2) == 0)
                    payload[i] = (byte)j++;
                else
                    payload[i] = 0;
            }

            for (int i = 0; i < packet.ExpectedLen / 2; i++)
                intPayload[i] = (short)i;

            packet.Serialize(payload);
            byte[] rtnPayload = packet.Payload;
            Int16[] rtnIntPayload = packet.DeSerialize();

            Assert.AreEqual(packet.ExpectedLen, rtnPayload.Length);
            Assert.AreEqual(packet.ExpectedLen / 2, rtnIntPayload.Length);

            for (int i = 0; i < rtnPayload.Length; i++)
                Assert.AreEqual(payload[i], rtnPayload[i]);

            for (int i = 0; i < rtnIntPayload.Length; i++)
                Assert.AreEqual(intPayload[i], rtnIntPayload[i]);
        }

        [TestMethod]
        public void TestMotus1PacketToString()
        {
            Motus_1_RawDataPacket packet = new Motus_1_RawDataPacket();
            byte[] payload = new byte[packet.ExpectedLen];

            int j = 0;
            for (byte i = 0; i < packet.ExpectedLen; i++)
            {
                if ((i % 2) == 0)
                    payload[i] = (byte)j++;
                else
                    payload[i] = 0;
            }

            packet.Serialize(payload);
            Int16[] intPayload = packet.DeSerialize();
            string known = intPayload[0].ToString();
            for (int i = 1; i < intPayload.Length; i++)
            {
                known += ",";
                known += intPayload[i].ToString();
            }

            Assert.AreEqual(known, packet.ToString());
        }
    }
}
