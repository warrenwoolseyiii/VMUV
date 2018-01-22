// SerializeUnitTests.cpp

#include "stdafx.h"

#include "Common.h"

#include "..\Comms Protocol Cpp\SerializeUtilities.h"
#include "SerializeUnitTests.h"

using namespace Comms_Protocol_Cpp;

void Comms_Protocol_Cpp_Tests::SerializeUnitTests::ConvertInt16ToBytesTestEndianness()
{
	Int16 testVal = 0x6985;
	vector<unsigned char> bigEndian =
		SerializeUtilities::ConvertInt16ToByteArray(testVal, Endianness::big_endian);
	vector<unsigned char> littleEndian =
		SerializeUtilities::ConvertInt16ToByteArray(testVal, Endianness::little_endian);
	AssertAreEqual((byte)((testVal >> 8) & 0xFF), bigEndian[0],		"SerializeUnitTests::ConvertInt16ToBytesTestEndianness()", 1);
	AssertAreEqual((byte)(testVal & 0xFF), bigEndian[1],			"SerializeUnitTests::ConvertInt16ToBytesTestEndianness()", 2);
	AssertAreEqual((byte)(testVal & 0xFF), littleEndian[0],			"SerializeUnitTests::ConvertInt16ToBytesTestEndianness()", 3);
	AssertAreEqual((byte)((testVal >> 8) & 0xFF), littleEndian[1],	"SerializeUnitTests::ConvertInt16ToBytesTestEndianness()", 4);
}

void Comms_Protocol_Cpp_Tests::SerializeUnitTests::BufferInt16IntoByteArrayBufferOverrunTest()
{
	Int16 testVal = 0x1234;
	vector<unsigned char> testBuff(2);
	int i = 0;
	i = SerializeUtilities::BufferInt16InToByteArray(testVal,
		testBuff, i, Endianness::big_endian);
	i = SerializeUtilities::BufferInt16InToByteArray(testVal,
		testBuff, i, Endianness::big_endian);
	i = SerializeUtilities::BufferInt16InToByteArray(testVal,
		testBuff, i, Endianness::big_endian);
	AssertAreEqual(2, i, "SerializeUnitTests::BufferInt16IntoByteArrayBufferOverrunTest()", 1);

	i = 0;
	i = SerializeUtilities::BufferInt16InToByteArray(testVal,
		testBuff, i, Endianness::little_endian);
	i = SerializeUtilities::BufferInt16InToByteArray(testVal,
		testBuff, i, Endianness::little_endian);
	i = SerializeUtilities::BufferInt16InToByteArray(testVal,
		testBuff, i, Endianness::little_endian);
	AssertAreEqual(2, i, "SerializeUnitTests::BufferInt16IntoByteArrayBufferOverrunTest()", 2);
}

void Comms_Protocol_Cpp_Tests::SerializeUnitTests::BufferInt16IntoByteArrayTest()
{
	Int16 testVal = 0x3456;
	vector<unsigned char> testBuff(10);
	for (int i = 0; i < testBuff.size();)
	{
		i = SerializeUtilities::BufferInt16InToByteArray(testVal,
			testBuff, i, Endianness::big_endian);
	}

	for (int i = 0; i < testBuff.size();)
	{
		AssertAreEqual((byte)((testVal >> 8) & 0xFF), testBuff[i++], "SerializeUnitTests::BufferInt16IntoByteArrayTest()", 1);
		AssertAreEqual((byte)(testVal & 0xFF), testBuff[i++], "SerializeUnitTests::BufferInt16IntoByteArrayTest()", 2);
	}

	for (int i = 0; i < testBuff.size();)
	{
		i = SerializeUtilities::BufferInt16InToByteArray(testVal,
			testBuff, i, Endianness::little_endian);
	}

	for (int i = 0; i < testBuff.size();)
	{
		AssertAreEqual((byte)(testVal & 0xFF), testBuff[i++], "SerializeUnitTests::BufferInt16IntoByteArrayTest()", 3);
		AssertAreEqual((byte)((testVal >> 8) & 0xFF), testBuff[i++], "SerializeUnitTests::BufferInt16IntoByteArrayTest()", 4);
	}
}

void Comms_Protocol_Cpp_Tests::SerializeUnitTests::ConvertByteArrayToInt16()
{
	Int16 knownVal = 0x1234;
	vector<unsigned char> littleEndian =
		SerializeUtilities::ConvertInt16ToByteArray(knownVal, Endianness::little_endian);
	vector<unsigned char> bigEndian =
		SerializeUtilities::ConvertInt16ToByteArray(knownVal, Endianness::big_endian);
	Int16 littleTest =
		SerializeUtilities::ConvertByteArrayToInt16(littleEndian, Endianness::little_endian);
	Int16 bigTest =
		SerializeUtilities::ConvertByteArrayToInt16(bigEndian, Endianness::big_endian);

	AssertAreEqual((byte)((knownVal >> 8) & 0xFF), bigEndian[0], "SerializeUnitTests::ConvertByteArrayToInt16()", 1);
	AssertAreEqual((byte)(knownVal & 0xFF), bigEndian[1], "SerializeUnitTests::ConvertByteArrayToInt16()", 2);

	AssertAreEqual((byte)((knownVal >> 8) & 0xFF), littleEndian[1], "SerializeUnitTests::ConvertByteArrayToInt16()", 3);
	AssertAreEqual((byte)(knownVal & 0xFF), littleEndian[0], "SerializeUnitTests::ConvertByteArrayToInt16()", 4);
}

void Comms_Protocol_Cpp_Tests::SerializeUnitTests::ConvertByteArrayToInt16IllegalByteLen()
{
	vector<unsigned char> array(1);
	Int16 testval =
		SerializeUtilities::ConvertByteArrayToInt16(array, Endianness::big_endian);
	AssertAreEqual(0, testval, "SerializeUnitTests::ConvertByteArrayToInt16IllegalByteLen()", 1);

	testval =
		SerializeUtilities::ConvertByteArrayToInt16(array, Endianness::little_endian);
	AssertAreEqual(0, testval, "SerializeUnitTests::ConvertByteArrayToInt16IllegalByteLen()", 2);
}
