#pragma once
#include <windows.networking.sockets.h>
#include "Packetizer.h"


namespace VMUV_TCP
{
	class StateObject
	{
	public:
		SOCKET workSocket = NULL;
		const int BufferSize = 1024;
        unsigned char buffer[1024];
		packetizer packetizer;

		StateObject();
	};
}