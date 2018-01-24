// DataPacket.h

#pragma once
#include <vector>

using namespace std;

namespace Comms_Protocol_Cpp
{
	enum ValidPacketTypes
	{
		test_packet = 0,
		motus_1_raw_data_packet = 1,
		end_valid_packet_types
	};

	class DataPacket
	{
	protected:
		vector<unsigned char> _payload;
		ValidPacketTypes _type;

	public:
		short _expectedLen;

		DataPacket() {
			_type = ValidPacketTypes::test_packet;
			_expectedLen = -1;
		}

		vector<unsigned char> getPayload()
		{ 
			return _payload;
		}
		void setPayload(vector<unsigned char> pay)
		{
			_payload = pay;
		}

		ValidPacketTypes getType()
		{
			return _type;
		}
		void setType(ValidPacketTypes type)
		{
			_type = type;
		}

		short getExpectedLen(void)
		{
			return _expectedLen;
		}
		void setExpectedLen(short len)
		{
			_expectedLen = len;
		}
	};
}
