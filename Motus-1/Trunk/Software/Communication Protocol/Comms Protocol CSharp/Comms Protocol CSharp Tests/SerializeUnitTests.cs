using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Comms_Protocol_CSharp;

namespace Comms_Protocol_CSharp_Tests
{
    [TestClass]
    public class SerializeUnitTests
    {
        [TestMethod]
        public void ConvertInt16ToBytesTestEndianness()
        {
            Int16 testVal = 0x6985;
            byte[] bigEndian =
                SerializeUtilities.ConvertInt16ToByteArray(testVal, Endianness.big_endian);
            byte[] littleEndian =
                SerializeUtilities.ConvertInt16ToByteArray(testVal, Endianness.little_endian);
            Assert.AreEqual((byte)((testVal >> 8) & 0xFF), bigEndian[0]);
            Assert.AreEqual((byte)(testVal & 0xFF), bigEndian[1]);
            Assert.AreEqual((byte)(testVal & 0xFF), littleEndian[0]);
            Assert.AreEqual((byte)((testVal >> 8) & 0xFF), littleEndian[1]);
        }

        [TestMethod]
        public void BufferInt16IntoByteArrayBufferOverrunTest()
        {
            Int16 testVal = 0x1234;
            byte[] testBuff = new byte[2];
            int i = 0;
            i = SerializeUtilities.BufferInt16InToByteArray(testVal,
                testBuff, i, Endianness.big_endian);
            i = SerializeUtilities.BufferInt16InToByteArray(testVal,
                testBuff, i, Endianness.big_endian);
            i = SerializeUtilities.BufferInt16InToByteArray(testVal,
                testBuff, i, Endianness.big_endian);
            Assert.AreEqual(2, i);

            i = 0;
            i = SerializeUtilities.BufferInt16InToByteArray(testVal,
                testBuff, i, Endianness.little_endian);
            i = SerializeUtilities.BufferInt16InToByteArray(testVal,
                testBuff, i, Endianness.little_endian);
            i = SerializeUtilities.BufferInt16InToByteArray(testVal,
                testBuff, i, Endianness.little_endian);
            Assert.AreEqual(2, i);
        }

        [TestMethod]
        public void BufferInt16IntoByteArrayTest()
        {
            Int16 testVal = 0x3456;
            byte[] testBuff = new byte[10];
            for (int i = 0; i < testBuff.Length;)
            {
                i = SerializeUtilities.BufferInt16InToByteArray(testVal,
                    testBuff, i, Endianness.big_endian);
            }

            for (int i = 0; i < testBuff.Length;)
            {
                Assert.AreEqual((byte)((testVal >> 8) & 0xFF), testBuff[i++]);
                Assert.AreEqual((byte)(testVal & 0xFF), testBuff[i++]);
            }

            for (int i = 0; i < testBuff.Length;)
            {
                i = SerializeUtilities.BufferInt16InToByteArray(testVal,
                    testBuff, i, Endianness.little_endian);
            }

            for (int i = 0; i < testBuff.Length;)
            {
                Assert.AreEqual((byte)(testVal & 0xFF), testBuff[i++]);
                Assert.AreEqual((byte)((testVal >> 8) & 0xFF), testBuff[i++]);
            }
        }

        [TestMethod]
        public void ConvertByteArrayToInt16()
        {
            Int16 knownVal = 0x1234;
            byte[] littleEndian =
                SerializeUtilities.ConvertInt16ToByteArray(knownVal, Endianness.little_endian);
            byte[] bigEndian =
                SerializeUtilities.ConvertInt16ToByteArray(knownVal, Endianness.big_endian);
            Int16 littleTest =
                SerializeUtilities.ConvertByteArrayToInt16(littleEndian, Endianness.little_endian);
            Int16 bigTest =
                SerializeUtilities.ConvertByteArrayToInt16(bigEndian, Endianness.big_endian);

            Assert.AreEqual((byte)((knownVal >> 8) & 0xFF), bigEndian[0]);
            Assert.AreEqual((byte)(knownVal & 0xFF), bigEndian[1]);

            Assert.AreEqual((byte)((knownVal >> 8) & 0xFF), littleEndian[1]);
            Assert.AreEqual((byte)(knownVal & 0xFF), littleEndian[0]);
        }

        [TestMethod]
        public void ConvertByteArrayToInt16IllegalByteLen()
        {
            byte[] array = new byte[1];
            Int16 testval =
                SerializeUtilities.ConvertByteArrayToInt16(array, Endianness.big_endian);
            Assert.AreEqual(0, testval);

            testval =
                SerializeUtilities.ConvertByteArrayToInt16(array, Endianness.little_endian);
            Assert.AreEqual(0, testval);
        }
    }
}
