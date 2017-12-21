#include "SocketWrapper.h"
#include "Packetizer.h"

VMUV_TCP::SocketWrapper::SocketWrapper(Configuration configuration)
{
	config = configuration;
}

void VMUV_TCP::SocketWrapper::ServerSetTxData(unsigned char* payload, int payloadLength, PacketTypes type)
{
	if (usePing)
	{
		packetizer.packetizeData(payload, payloadLength, txDataPing, (unsigned char)type);
		usePing = false;
	}
	else
	{
		packetizer.packetizeData(payload, payloadLength, txDataPong, (unsigned char)type);
		usePing = true;
	}
}

void VMUV_TCP::SocketWrapper::ClientGetRxData(unsigned char * dataOut)
{
	if (usePing)
	{
		for (int i = 0; i < CurDatPacLen; i++)
			dataOut[i] = rxDataPong[i];
	}
	else
	{
		for (int i = 0; i < CurDatPacLen; i++)
			dataOut[i] = rxDataPing[i];
	}
}

void VMUV_TCP::SocketWrapper::StartServer()
{
	std::string methodName = "StartServer";

	if (config != Configuration::server)
		return;

	IPEndPoint localEP = new IPEndPoint(IPAddress.Loopback, port); //TODO research how to instantiate ip address loopbacks

	
	
	try
	{
		std::string msg = "TCP Server successfully started on port " +  port;

		listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //TODO research how to implement the socket coms

		listener.Bind(localEP);
		listener.Listen(100);
		listener.BeginAccept(new AsyncCallback(AcceptCB), listener);

		traceLogger.QueueMessage(traceLogger.BuildMessage(moduleName, methodName, msg));
	}
	catch (Exception e0)
	{
		string msg = e0.Message + e0.StackTrace;

		traceLogger.QueueMessage(traceLogger.BuildMessage(moduleName, methodName, msg));
		DebugPrint(msg);
	}
}

void VMUV_TCP::SocketWrapper::ClientStartRead()
{
	std::string methodName = "ClientStartRead";

	if (clientIsBusy || (config != Configuration::client))
		return;

	//TODO finish this method once you have figured out IP connections
}
