// TCP_Plugin_unreal.cpp : Defines the entry point for the console application.
//
#include "stdafx.h"
#include "packetizer.h"
#include "SocketWrapper.h"
using namespace VMUV_TCP;

#include <vector>
#include <iostream>
using std::cin;
using std::cout;
using std::endl;
using std::vector;

int main()
{
	
	
	/*vector<unsigned char> someData;
	for (int i = 0; i < 8; i++)
		someData.push_back(i);
	
	cout << "Data before packetizing: ";
	for (int i = 0; i < static_cast<int>(someData.size()); i++)
		cout << static_cast<int>(someData[i]) << " ";

	cout << endl;
	

	///////////////Testing packetizer::packetizeData()////////////////////
	packetizer myPacketizer;
	vector<unsigned char> dataPacket = myPacketizer.packetizeData(someData, 3);

	cout << "Data after packetizing: ";
	for (int i = 0; i < static_cast<int>(dataPacket.size()); i++)
		cout << static_cast<int>(dataPacket[i]) << " ";

	cout << endl;
	
	//////////////Testing packetizer::isPacketValid()////////////////////
	cout << "Is our packet legal?: ";
	if (myPacketizer.isPacketValid(dataPacket))
		cout << "Yes! Woo!";
	else
		cout << "Something is up";

	cout << endl;
	
	///////////////Testing packetizer::unpackData()///////////////////////
	vector<unsigned char> unpackedData = myPacketizer.unpackData(dataPacket);
	cout << "Unpacked data: ";
	for (int i = 0; i < static_cast<int>(unpackedData.size()); i++)
		cout << static_cast<int>(unpackedData[i]) << " ";
	cout << endl << endl;

	/////////////Testing socketWrapper::serverSetTxData()/////////////////
	cout << "Can our socket wrapper write to the buffer?" << endl;
	socketWrapper testWrapper(Configuration::server);
	testWrapper.serverSetTxData(someData, PacketTypes::raw_data);
	vector<unsigned char> wrapperDataPing = testWrapper.getTxDataPing();
	vector<unsigned char> wrapperDataPong = testWrapper.getTxDataPong();
	cout << "txDataPing: ";
	for (int i = 0; i < static_cast<int>(wrapperDataPing.size()); i++)
		cout << static_cast<int>(wrapperDataPing[i]) << " ";
	cout << endl << endl << "txDataPong: ";
	for (int i = 0; i < static_cast<int>(wrapperDataPong.size()); i++)
		cout << static_cast<int>(wrapperDataPong[i]) << " ";
	cout << endl;
	
	cout << "Does our serverSetTxData() ping pong?" << endl;
	testWrapper.serverSetTxData(someData, PacketTypes::raw_data);
	wrapperDataPing = testWrapper.getTxDataPing();
	wrapperDataPong = testWrapper.getTxDataPong();
	cout << "txDataPing: ";
	for (int i = 0; i < static_cast<int>(wrapperDataPing.size()); i++)
		cout << static_cast<int>(wrapperDataPing[i]) << " ";
	cout << endl << endl << "txDataPong: ";
	for (int i = 0; i < static_cast<int>(wrapperDataPong.size()); i++)
		cout << static_cast<int>(wrapperDataPong[i]) << " ";
	cout << endl;

	cout << endl;

	////////////////Testing setRxData() and clientGetRxData()/////////////////////
	cout << "Can our socket wrapper write to the buffer?" << endl;
	socketWrapper testClientWrapper(Configuration::client);
	testClientWrapper.setRxData(someData, PacketTypes::raw_data);
	vector<unsigned char> clientWrapperData = testClientWrapper.clientGetRxData();     //working here!!!
	cout << "rxData: ";
	for (int i = 0; i < static_cast<int>(clientWrapperData.size()); i++)
		cout << static_cast<int>(clientWrapperData[i]) << " ";
	cout << endl;*/

	////////////////Testing socketWrapper::startServer()/////////////////////
	//testWrapper.startServer();




	/////////////////Testing Client////////////////////////////
	socketWrapper clientWrapper(Configuration::client);

	while (true)
	{
		clientWrapper.clientStartRead();
		vector<unsigned char> data = clientWrapper.clientGetRxData();
		cout << "Use ping?: " << clientWrapper.getUsePing() << endl;
		for (int j = 0; j < (int)data.size() - 1; j += 2)
		{
			unsigned short tmp = (unsigned short)data[j + 1];
			tmp <<= 8;
			tmp |= (unsigned short)data[j];
			cout << tmp << endl;
		}
	}

    return 0;
}
