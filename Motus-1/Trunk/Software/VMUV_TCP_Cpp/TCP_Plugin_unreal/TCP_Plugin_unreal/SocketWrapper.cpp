#include "stdafx.h"
#include "SocketWrapper.h"

VMUV_TCP::socketWrapper::socketWrapper(Configuration configuration) : config(configuration)
{
	txDataPing.reserve(256);
	txDataPong.reserve(256);
	rxDataPing.reserve(256);
	rxDataPong.reserve(256);
}

string VMUV_TCP::socketWrapper::getVersion() const
{
	return version;
}

void VMUV_TCP::socketWrapper::serverSetTxData(vector<unsigned char> payload, PacketTypes type)
{
	if (usePing)
	{
		txDataPing.clear();
		txDataPing = packetMaker.packetizeData(payload, static_cast<int>(type));
		usePing = false;
	}
	else
	{
		txDataPong.clear();
		txDataPong = packetMaker.packetizeData(payload, static_cast<int>(type));
		usePing = true;
	}
}

vector<unsigned char> VMUV_TCP::socketWrapper::clientGetRxData() const
{
	if (usePing)
		return rxDataPong;
	else
		return rxDataPing;
}

void VMUV_TCP::socketWrapper::startServer()
{
}

vector<unsigned char> VMUV_TCP::socketWrapper::getTxDataPing() const
{
	return txDataPing;
}

vector<unsigned char> VMUV_TCP::socketWrapper::getTxDataPong() const
{
	return txDataPong;
}

