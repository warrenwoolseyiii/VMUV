#pragma once
#ifndef SOCKETWRAPPER_H
#define SOCKETWRAPPER_H

#include "packetizer.h"
#include <string>
#include <vector>
using std::string;
using std::vector;

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
		vector<unsigned char> clientGetRxData() const;
		
		
		vector<unsigned char> getTxDataPing() const;
		vector<unsigned char> getTxDataPong() const;
	private:
		packetizer packetMaker;
		const int port = 11069;
		vector<unsigned char> txDataPing;
		vector<unsigned char> txDataPong;
		vector<unsigned char> rxDataPing;
		vector<unsigned char> rxDataPong;
		bool usePing = true;
		Configuration config;
		bool clientIsBusy = false;
		string moduleName = "SocketWrapper.cpp";
		int numPacketsRead = 0;
		const string version = "1.0.2";
	};
}




#endif SOCKETWRAPPER_H
