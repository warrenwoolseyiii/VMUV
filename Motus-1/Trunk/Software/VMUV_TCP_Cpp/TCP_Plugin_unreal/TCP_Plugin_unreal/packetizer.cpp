#include "stdafx.h"
#include "packetizer.h"
#include <iostream>
using std::cout;
using std::endl;

#define BUFF_SIZE   256

unsigned char VMUV_TCP::packetizer::getPacketType(vector<unsigned char> packet)
{
	if (isPacketValid(packet))
		return packet[2];
	else
		return 0xff;
}

short VMUV_TCP::packetizer::calculateCheckSum(vector<unsigned char> payload)
{
	short chksum = 0;

	if (payload.size() != 0)
	{
		for (int i = 0; i < static_cast<int>(payload.size()); i++)
			chksum += static_cast<short>(payload[i] & 0xff);
	}
	
	return chksum;
}

bool VMUV_TCP::packetizer::isPacketValid(vector<unsigned char> packet)
{
	if (static_cast<int>(packet.size()) == 7)
		return false;

	if (packet[0] != sync1)
		return false;

	if (packet[1] != sync2)
		return false;

	unsigned char  type = packet[2];
	short len = static_cast<short>(packet[3]);
	len << 8;
	len |= static_cast<short>(packet[4] & 0xff);

	if (len > BUFF_SIZE)
		return false;

	short chkSum = packet[5 + len];
	chkSum << 8;
	chkSum |= packet[6 + len];

	vector<unsigned char> extractedPayload;

	for (int i = 0; i < len; i++)
		extractedPayload.push_back(packet[i + 5]);

	short calcChkSum = calculateCheckSum(extractedPayload);

	if (calcChkSum != chkSum)
		return false;
	
	
	return true;
}

vector<unsigned char> VMUV_TCP::packetizer::packetizeData(vector<unsigned char> payload, int type)
{
	if (payload.size() == 0)
		cout << "You are trying to packetize nothing" << endl;

	unsigned short len = static_cast<unsigned short>(payload.size());

	if (len > BUFF_SIZE)
		cout << "Your payload is too big yo" << endl;

	short chkSum = calculateCheckSum(payload);

	vector<unsigned char> packetizedData;
	packetizedData.push_back(sync1);
	packetizedData.push_back(sync2);
	packetizedData.push_back(type);
	packetizedData.push_back(static_cast<unsigned char>((len >> 8) & 0xff));
	packetizedData.push_back(static_cast<unsigned short>(len & 0xff));

	for (int i = 0; i < len; i++)
		packetizedData.push_back(payload[i]);

	packetizedData.push_back(static_cast<unsigned char>((chkSum >> 8) & 0xff));
	packetizedData.push_back(static_cast<unsigned char>(chkSum & 0xff));
	
	return packetizedData;
}

vector<unsigned char> VMUV_TCP::packetizer::unpackData(vector<unsigned char> packet)
{
	short len = static_cast<short>(packet[3] & 0xff);
	len << 8;
	len |= static_cast<short>(packet[4] & 0xff);

	vector<unsigned char> unpackedData;

	for (short i = 0; i < len; i++)
		unpackedData.push_back(packet[i + 5]);
	
	return unpackedData;
}
