#include "Packetizer.h"
#include "SocketWrapper.h"
#include <stdlib.h> 

VMUV_TCP::packetizer::packetizer()
{
}

void VMUV_TCP::packetizer::packetizeData(unsigned char * payload, int payloadLength, unsigned char * packetizedPayload, unsigned char type)
{
    if (payload == nullptr)
        return;

	unsigned short len = (unsigned short)(payloadLength & 0xffff);

    if (len > BUFF_SIZE)
        return;

	short chkSum = CalculateCheckSumFromPayload(payload, payloadLength);

	packetizedPayload[0] = sync1;
	packetizedPayload[1] = sync2;
	packetizedPayload[2] = type;
	packetizedPayload[3] = (unsigned char)((len >> 8) & 0xff);
	packetizedPayload[4] = (unsigned char)(len & 0xff);

	for (int i = 0; i < len; i++)
		packetizedPayload[5 + i] = payload[i];

	packetizedPayload[5 + len] = (unsigned char)((chkSum >> 8) & 0xff);
	packetizedPayload[6 + len] = (unsigned char)(chkSum & 0xff);
}

short VMUV_TCP::packetizer::CalculateCheckSumFromPayload(unsigned char * payload, int payloadLength)
{
	short chkSum = 0;

	if (payload != nullptr)
	{
		for (int i = 0; i < payloadLength; i++)
			chkSum += (short)(payload[i] & 0xff);
	}

	return chkSum;
}

bool VMUV_TCP::packetizer::IsPacketValid(unsigned char * packet)
{
	if (packet == nullptr)
		return false;

	if (packet[0] != sync1)
		return false;

	if (packet[1] != sync2)
		return false;

	unsigned char type = packet[2];

	short len = (short)(packet[3]);
	len <<= 8;
	len |= (short)(packet[4] & 0xff);
	
    if (len > BUFF_SIZE)
        return false;

    short recChkSum = packet[5 + len];
    recChkSum <<= 8;
    recChkSum |= packet[6 + len];

    short calcChkSum = CalculateCheckSumFromPayload(packet, len);

    if (calcChkSum != recChkSum)
        return false;

	return true;
}


