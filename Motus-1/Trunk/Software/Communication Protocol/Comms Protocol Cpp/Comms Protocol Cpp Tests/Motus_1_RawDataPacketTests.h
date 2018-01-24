// Motus_1_RawDataPacketTests.h

#pragma once

namespace Comms_Protocol_Cpp_Tests
{
	class Motus_1_RawDataPacketTests
	{
	public:
		void Motus_1_PacketTestConstructor();
		void Motus_1_PacketTestSerializeInvalidPayload();
		void Motus_1_PacketTestSerializeBytes();
		void TestMotus1PacketToString();
	};
}
