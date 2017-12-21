#pragma once
#include <windows.networking.sockets.h>
#include <string>
#include "Packetizer.h"
#include "TraceLogger.h"

#define BUFF_SIZE   256

//class Packetizer;
//class TraceLogger;
//class Packetizer;

namespace VMUV_TCP
{
	enum class Configuration
	{
		client,
		server
	};
	enum class PacketTypes
	{
		test,
		raw_data
	};
	class SocketWrapper
	{


	private:
		packetizer packetizer;
		TraceLogger traceLogger; //TODO figure out what in the world is causing this file not to recognize my TraceLogger class
		SOCKET listener = NULL;
		const int port = 11069;
		unsigned char txDataPing [BUFF_SIZE] = { 0 };  // Do this incase Start() is called before the user sets any data
		unsigned char txDataPong [BUFF_SIZE] = { 0 };  // Do this incase Start() is called before the user sets any data
		unsigned char rxDataPing [BUFF_SIZE] = { 0 };  // Do this incase GetRxData() is called before the user gets any data
		unsigned char rxDataPong [BUFF_SIZE] = { 0 };  // Do this incase GetRxData() is called before the user gets any data
		bool usePing = true;
		Configuration config;
		bool clientIsBusy = false;
		std::string moduleName = "SocketWrapper.cs";
		int numPacketsRead = 0;
		int CurDatPacLen = 0;

	public:
		/// <summary>
		/// Version number of the current release.
		/// </summary>
		const std::string version = "1.0.2";

		/// <summary>
		/// Instantiates a new instance of <c>SocketWrapper</c> configured as either a client or a server.
		/// </summary>
		/// <param name="configuration"></param>
		SocketWrapper(Configuration configuration);

		/// <summary>
		/// Sets the data from <c>payload</c> into the transmit data buffer.
		/// </summary>
		/// <param name="payload"></param>
		/// <param name="type"></param>
		void ServerSetTxData(unsigned char* payload, int payloadLength, PacketTypes type);

		/// <summary>
		/// Acquires the most recently received valid data payload.
		/// </summary>
		/// <returns>byte buffer with a copy of the most recently receieved valid data payload.</returns>
		void ClientGetRxData(unsigned char * dataOut);


		/// <summary>
		/// Call this method only once after instantiation of the <c>SocketWrapper</c> object. This will start the 
		/// server listener for incoming connections.
		/// </summary>
		void StartServer();

		/// <summary>
		/// Call this method in from the main thread to start the next client read process.
		/// </summary>
		void ClientStartRead();

	};

}