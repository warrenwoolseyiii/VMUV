#pragma once
#ifndef SOCKETWRAPPER_H
#define SOCKETWRAPPER_H

#include "packetizer.h"
#include <WinSock2.h>
#include <string>
#include <iostream>
#include <vector>
#pragma comment(lib,"WS2_32")
using std::string;
using std::cout;
using std::endl;
using std::vector;

#define NETWORK_ERROR -1
#define NETWORK_OK 0

namespace VMUV_TCP
{
	enum class Configuration
	{
		server,
		client
	};

	enum class PacketTypes
	{
		test,
		raw_data
	};

	class socketWrapper
	{
	public:
		socketWrapper(Configuration configuration);

		string getVersion() const;
		void serverSetTxData(vector<unsigned char> payload, PacketTypes type);
		void setRxData(vector<unsigned char> payload, PacketTypes type);
		vector<unsigned char> clientGetRxData() const;
		void startServer();
		void clientStartRead();
		
		bool getUsePing() const;
		vector<unsigned char> getTxDataPing() const;
		vector<unsigned char> getTxDataPong() const;
	private:
		packetizer packetMaker;
		SOCKET listener, client;
		const int port = 11069;
		const int loopBackIpAddr = 16777343;
		vector<unsigned char> txDataPing;
		vector<unsigned char> txDataPong;
		vector<unsigned char> rxDataPing;
		vector<unsigned char> rxDataPong;
		bool usePing = true;
		Configuration config;
		bool clientIsBusy = false;
		string moduleName = "SocketWrapper.cpp";
		int numPacketsRead = 0;
		const string version = "1.0.0";

		void reportError(int, const char*);
	};
}




#endif SOCKETWRAPPER_H
