#include <iostream>
#include <chrono>
#include <thread>
#include <Windows.h>
#include "Program.h"
#include "SocketWrapper.h"


void VMUV_TCP::Program::Main()
{
	
	SelfTest();
}

void VMUV_TCP::Program::SelfTest()
{
	SocketWrapper server = SocketWrapper(Configuration::server);
	SocketWrapper client = SocketWrapper(Configuration::client);
	unsigned char testNigga [] = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };

	server.ServerSetTxData(testNigga, 18, PacketTypes::test);
	server.StartServer();

	while (true)
	{
		client.ClientStartRead(); 
		std::cout << "Got " << client.ClientGetRxData().Length.ToString() << " bytes";
		std::this_thread::sleep_for(std::chrono::milliseconds(500));
		
	}
}