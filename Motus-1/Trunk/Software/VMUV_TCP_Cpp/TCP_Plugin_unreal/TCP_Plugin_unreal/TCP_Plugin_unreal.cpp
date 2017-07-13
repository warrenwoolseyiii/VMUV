// TCP_Plugin_unreal.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "packetizer.h"
using namespace VMUV_TCP;

#include <vector>
#include <iostream>
using std::cin;
using std::cout;
using std::endl;
using std::vector;

int main()
{
	vector<unsigned char> someData;
	for (int i = 0; i < 8; i++)
		someData.push_back(i);
	
	cout << "Data before packetizing: ";
	for (int i = 0; i < static_cast<int>(someData.size()); i++)
		cout << static_cast<int>(someData[i]) << " ";

	cout << endl;
	
	packetizer myPacketizer;
	vector<unsigned char> dataPacket = myPacketizer.packetizeData(someData, 3);

	cout << "Data after packetizing: ";
	for (int i = 0; i < static_cast<int>(dataPacket.size()); i++)
		cout << static_cast<int>(dataPacket[i]) << " ";

	cout << endl;

	cout << "Is our packet legal?: ";

	if (myPacketizer.isPacketValid(dataPacket))
		cout << "Yes! Woo!";
	else
		cout << "Something is up";

	cout << endl;

	vector<unsigned char> unpackedData = myPacketizer.unpackData(dataPacket);

	cout << "Unpacked data: ";
	for (int i = 0; i < static_cast<int>(unpackedData.size()); i++)
		cout << static_cast<int>(unpackedData[i]) << " ";

	cout << endl;
	while (true)
	{ }

    return 0;
}

