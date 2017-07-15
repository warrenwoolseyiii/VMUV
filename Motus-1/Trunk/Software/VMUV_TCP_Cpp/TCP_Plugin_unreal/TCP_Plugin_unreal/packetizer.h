#pragma once
#ifndef PACKETIZER_H
#define PACKETIZER_H

#include <vector>
using std::vector;

namespace VMUV_TCP
{
	class packetizer
	{
	public:
		unsigned char getPacketType(vector<unsigned char> packet);
		short calculateCheckSum(vector<unsigned char> payload);
		bool isPacketValid(vector<unsigned char> payload);
		vector<unsigned char> packetizeData(vector<unsigned char> payload, int type);
		vector<unsigned char> unpackData(vector<unsigned char> packet);
	private:
		const unsigned char sync1 = 0x69;
		const unsigned char sync2 = 0xee;
		const int numOverheadBytes = 7;
	};
}





#endif //PACKETIZER_H

