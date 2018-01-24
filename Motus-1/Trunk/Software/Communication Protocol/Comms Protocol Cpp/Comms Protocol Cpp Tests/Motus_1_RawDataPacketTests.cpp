// Motus_1_RawDataPacketTests.cpp

#include "stdafx.h"

#include "Common.h"

#include "..\Comms Protocol Cpp\Motus_1_RawDataPacket.h"
#include "Motus_1_RawDataPacketTests.h"

using namespace Comms_Protocol_Cpp;

void Comms_Protocol_Cpp_Tests::Motus_1_RawDataPacketTests::Motus_1_PacketTestConstructor()
{
	Motus_1_RawDataPacket packet;
	AssertAreEqual(ValidPacketTypes::motus_1_raw_data_packet, packet.getType(), "Motus_1_RawDataPacketTests::Motus_1_PacketTestConstructor()", 1);
	AssertAreEqual(18, packet.getExpectedLen(), "Motus_1_RawDataPacketTests::Motus_1_PacketTestConstructor()", 2);
}

//[ExpectedException(typeof(ArgumentException), "Accepted illegal payload")]
void Comms_Protocol_Cpp_Tests::Motus_1_RawDataPacketTests::Motus_1_PacketTestSerializeInvalidPayload()
{
	Motus_1_RawDataPacket packet;
	vector<unsigned char> invalidBytePayload(12);
	try {
		packet.Serialize(invalidBytePayload);
	}
	catch (int) {
		cout << "unit test Motus_1_RawDataPacketTests::Motus_1_PacketTestSerializeInvalidPayload() correctly threw exception\n";
		return;
	}
	cout << "unit test Motus_1_RawDataPacketTests::Motus_1_PacketTestSerializeInvalidPayload() FAILED to throw exception\n";
}

void Comms_Protocol_Cpp_Tests::Motus_1_RawDataPacketTests::Motus_1_PacketTestSerializeBytes()
{
	Motus_1_RawDataPacket packet;
	vector<unsigned char> payload(packet.getExpectedLen());
	vector<short> intPayload(packet.getExpectedLen() / 2);

	int j = 0;
	for (byte i = 0; i < packet.getExpectedLen(); i++)
	{
		if ((i % 2) == 0)
			payload[i] = (byte)j++;
		else
			payload[i] = 0;
	}

	for (int i = 0; i < packet.getExpectedLen() / 2; i++)
		intPayload[i] = (short)i;

	packet.Serialize(payload);
	vector<unsigned char> rtnPayload = packet.getPayload();
	vector<short> rtnIntPayload = packet.DeSerialize();

	AssertAreEqual(packet.getExpectedLen(), rtnPayload.size(), "Motus_1_RawDataPacketTests::Motus_1_PacketTestSerializeBytes()", 1);
	AssertAreEqual(packet.getExpectedLen() / 2, rtnIntPayload.size(), "Motus_1_RawDataPacketTests::Motus_1_PacketTestSerializeBytes()", 2);

	for (int i = 0; i < rtnPayload.size(); i++)
		AssertAreEqual(payload[i], rtnPayload[i], "Motus_1_RawDataPacketTests::Motus_1_PacketTestSerializeBytes()", 3);

	for (int i = 0; i < rtnIntPayload.size(); i++)
		AssertAreEqual(intPayload[i], rtnIntPayload[i], "Motus_1_RawDataPacketTests::Motus_1_PacketTestSerializeBytes()", 4);
}

void Comms_Protocol_Cpp_Tests::Motus_1_RawDataPacketTests::TestMotus1PacketToString()
{
	char tmpstr[100];
	Motus_1_RawDataPacket packet;
	vector<unsigned char> payload(packet.getExpectedLen());

	int j = 0;
	for (byte i = 0; i < packet.getExpectedLen(); i++)
	{
		if ((i % 2) == 0)
			payload[i] = (byte)j++;
		else
			payload[i] = 0;
	}

	packet.Serialize(payload);
	vector<short> intPayload = packet.DeSerialize();
	sprintf_s(tmpstr, "%d", intPayload[0]);
	string known = tmpstr;

	for (int i = 1; i < intPayload.size(); i++)
	{
		known += ",";
		sprintf_s(tmpstr, "%d", intPayload[i]);
		known += tmpstr;
	}

	AssertAreEqualStrings(known, packet.ToString(), "Motus_1_RawDataPacketTests::TestMotus1PacketToString()", 1);
}
