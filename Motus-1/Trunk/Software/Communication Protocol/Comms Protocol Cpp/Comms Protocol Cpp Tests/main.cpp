// main.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include <iostream>
#include <stdio.h>

#include "SerializeUnitTests.h"
#include "Motus_1_RawDataPacketTests.h"

using namespace std;
using namespace Comms_Protocol_Cpp_Tests;

int main()
{
	SerializeUnitTests serializer;
	serializer.ConvertInt16ToBytesTestEndianness();
	serializer.BufferInt16IntoByteArrayBufferOverrunTest();
	serializer.BufferInt16IntoByteArrayTest();
	serializer.ConvertByteArrayToInt16();
	serializer.ConvertByteArrayToInt16IllegalByteLen();

	Motus_1_RawDataPacketTests rawpacket;
	rawpacket.Motus_1_PacketTestConstructor();
	rawpacket.Motus_1_PacketTestSerializeInvalidPayload();
	rawpacket.Motus_1_PacketTestSerializeBytes();
	rawpacket.TestMotus1PacketToString();

	char c;
	cout << "\nall tests complete - press enter to quit...";
	c = getchar();

	return 0;
}

