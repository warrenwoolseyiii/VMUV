// Motus_1_RawDataPacket.cpp

#include "stdafx.h"

#include "SerializeUtilities.h"
#include "Motus_1_RawDataPacket.h"

Comms_Protocol_Cpp::Motus_1_RawDataPacket::Motus_1_RawDataPacket()
{
	_type = ValidPacketTypes::motus_1_raw_data_packet;
	_expectedLen = 18;
}

void Comms_Protocol_Cpp::Motus_1_RawDataPacket::Serialize(vector<unsigned char> payload)
{
	if (payload.size() != _expectedLen)
		//throw new ArgumentException();
		throw ARGUMENTEXCEPTION;

	_payload = payload;
}

vector<short> Comms_Protocol_Cpp::Motus_1_RawDataPacket::DeSerialize()
{
	vector<short> rtn(_expectedLen / 2);
	int byteIndex = 0;
	vector<unsigned char> bytePayload = _payload;
	try
	{
		for (size_t i = 0; i < rtn.size(); i++)
		{
			vector<unsigned char> tmp(2);
			tmp[0] = bytePayload[byteIndex++];
			tmp[1] = bytePayload[byteIndex++];

			rtn.at(i) = SerializeUtilities::ConvertByteArrayToInt16(tmp, little_endian);
		}
	}
//	catch (IndexOutOfRangeException) {}
	catch (int) {}
	return rtn;
}

string Comms_Protocol_Cpp::Motus_1_RawDataPacket::ToString()
{
	char tmpstr[10];
	string rtn = "";
	vector<short> vals = DeSerialize();
	if (vals.size() == _expectedLen / 2)
	{
		sprintf_s(tmpstr, "%d", vals.at(0));
		rtn = tmpstr;
		for (size_t i = 1; i < vals.size(); i++)
		{
			rtn += ",";
			sprintf_s(tmpstr, "%d", vals.at(i));
			rtn += tmpstr;
		}
	}
	return rtn;
}
