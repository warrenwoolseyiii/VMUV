// SerializeUnitTests.h

#pragma once

namespace Comms_Protocol_Cpp_Tests
{
	class SerializeUnitTests
	{
	public:
		void ConvertInt16ToBytesTestEndianness();
		void BufferInt16IntoByteArrayBufferOverrunTest();
		void BufferInt16IntoByteArrayTest();
		void ConvertByteArrayToInt16();
		void ConvertByteArrayToInt16IllegalByteLen();
	};
}
