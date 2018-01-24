// Motus_1_RawDataPacket.h

#pragma once

#include <string>

#include "DataPacket.h"

#define INDEXOUTOFRANGEEXCEPTION 1
#define ARGUMENTEXCEPTION 2

using namespace std;

namespace Comms_Protocol_Cpp
{
	class Motus_1_RawDataPacket : public DataPacket
	{
	public:
		Motus_1_RawDataPacket();

		void Serialize(vector<unsigned char> payload);

		vector<short> DeSerialize();

		string ToString();
	};
}
