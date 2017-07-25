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
	if (config == Configuration::client)
		return;
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

void VMUV_TCP::socketWrapper::setRxData(vector<unsigned char> payload, PacketTypes type)
{
	if (config == Configuration::server)
		return;
	if (usePing)
	{
		rxDataPing.clear();
		rxDataPing = packetMaker.packetizeData(payload, static_cast<int>(type));
		usePing = false;
	}
	else
	{
		rxDataPong.clear();
		rxDataPong = packetMaker.packetizeData(payload, static_cast<int>(type));
		usePing = true;
	}
}

vector<unsigned char> VMUV_TCP::socketWrapper::clientGetRxData() const
{
	if (config == Configuration::server)
		return vector<unsigned char>();
	if (usePing)
		return rxDataPong;
	else
		return rxDataPing;
}

void VMUV_TCP::socketWrapper::startServer()
{
	if (config == Configuration::client)
		return;

	WORD sockVersion;
	WSADATA wsaData;
	int nret;

	sockVersion = MAKEWORD(1, 0, 1);
	WSAStartup(sockVersion, &wsaData);

	listener = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

	if (listener == INVALID_SOCKET)
	{
		nret = WSAGetLastError();
		reportError(nret, "socket()");

		WSACleanup();
		return;
	}
	cout << "yay! initialized the socket!" << endl;

	SOCKADDR_IN serverInfo;
	serverInfo.sin_family = AF_INET;
	serverInfo.sin_addr.s_addr = INADDR_ANY;
	serverInfo.sin_port = htons(port);

	nret = bind(listener, (LPSOCKADDR)&serverInfo, sizeof(serverInfo));


	if (nret == SOCKET_ERROR)
	{
		nret = WSAGetLastError();
		reportError(nret, "bind()");

		WSACleanup();
		return;
	}
	cout << "yay! bound the socket to the right address!" << endl;

	nret = listen(listener, 10);
	if (nret == SOCKET_ERROR)
	{
		nret = WSAGetLastError();
		reportError(nret, "listen()");

		WSACleanup();
		return;
	}

	cout << "listening for client" << endl;

	SOCKET theClient;

	theClient = accept(listener, NULL, NULL);

	if (theClient == INVALID_SOCKET)
	{
		nret = WSAGetLastError();
		reportError(nret, "accept()");

		WSACleanup();
		return;
	}

}

void VMUV_TCP::socketWrapper::clientStartRead()
{
	if (clientIsBusy || config == Configuration::server)
		return;
	
	WORD sockVersion;
	WSADATA wsaData;
	int nret;
	char readBuff[256];

	sockVersion = MAKEWORD(1, 0, 1);
	WSAStartup(sockVersion, &wsaData);

	//initiallize the client socket
	client = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

	if (client == INVALID_SOCKET)
	{
		nret = WSAGetLastError();
		reportError(nret, "socket()");

		WSACleanup();
		return;
	}
	
	//prepare for connection by filling sockaddr struct with info about the server
	SOCKADDR_IN serverInfo;
	serverInfo.sin_family = AF_INET;
	serverInfo.sin_addr.s_addr = loopBackIpAddr;//*((LPIN_ADDR)*hostEntry->h_addr_list);
	serverInfo.sin_port = htons(port);
	cout << "server info initiallized" << endl;
	
	//connect to the server!
	nret = connect(client, (LPSOCKADDR)&serverInfo, sizeof(serverInfo));
	if (nret == SOCKET_ERROR)
	{
		nret = WSAGetLastError();
		reportError(nret, "connect()");

		WSACleanup();
		return;
	}

	//get a data packet from the tcp connection
	nret = recv(client, readBuff, 7 + 19, 0);
	if (nret == SOCKET_ERROR)
	{
		nret = WSAGetLastError();
		reportError(nret, "connect()");

		WSACleanup();
		return;
	}
	if (nret != 26)
	{
		closesocket(client);
		WSACleanup();
		return;
	}
		
	
	vector<unsigned char> packet, data;
	for (int i = 0; i < 7 + 19; i++)
		packet.push_back(readBuff[i]);

	cout << "got :" << nret << " bytes..." << endl;
	bool rtn = packetMaker.isPacketValid(packet);
	if (rtn)
	{
		data = packetMaker.unpackData(packet);

		setRxData(data, PacketTypes::raw_data);
	}
	
}

bool VMUV_TCP::socketWrapper::getUsePing() const
{
	return usePing;
}

vector<unsigned char> VMUV_TCP::socketWrapper::getTxDataPing() const
{
	return txDataPing;
}

vector<unsigned char> VMUV_TCP::socketWrapper::getTxDataPong() const
{
	return txDataPong;
}

void VMUV_TCP::socketWrapper::reportError(int errorCode, const char *whichFunc)
{
	char errorBuff[92];

	ZeroMemory(errorBuff, 92);     //clears 92 units of memory starting at the location of errorBuff

	sprintf(errorBuff, "Call to %s returned error %d!", (char *)whichFunc, errorCode);  //stores error in errorBuff

	WCHAR errorMessage[92];
	for (int i = 0; i < 92; i++)						//converting regular chars to wide chars so they can be output using messagebox which uses unicode
		errorMessage[i] = WCHAR(errorBuff[i]);

	MessageBox(NULL, LPCWSTR(errorMessage), L"SocketIndicationError", MB_OK);  //creates a WINAPI message box to display errorBuff as a 'Long Pointer to Constant Wide String'(stores characters in 2-byte chars)
}

