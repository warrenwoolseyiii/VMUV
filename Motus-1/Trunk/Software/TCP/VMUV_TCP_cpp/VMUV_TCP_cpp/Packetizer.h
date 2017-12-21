#pragma once


namespace VMUV_TCP
{
	class packetizer
	{
	private:
		const unsigned char sync1 = 0x69;
		const unsigned char sync2 = 0xee;
		const int numOverHeadBytes = 7;

	public:
		void packetizeData(unsigned char * payload, int payloadLength, unsigned char * packetizedPayload, unsigned char type);
		short CalculateCheckSumFromPayload(unsigned char* payload, int payloadLength);
		bool IsPacketValid(unsigned char* packet);
		packetizer();
		
	};
}