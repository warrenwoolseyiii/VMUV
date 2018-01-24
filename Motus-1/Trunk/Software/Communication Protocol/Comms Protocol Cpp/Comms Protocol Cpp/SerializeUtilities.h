// SerializeUtilities.h

#pragma once
#include <vector>

using namespace std;


namespace Comms_Protocol_Cpp
{
	enum Endianness
	{
		little_endian = 0,
		big_endian = 1
	};

	class SerializeUtilities
	{
	public:
		static vector<unsigned char> ConvertInt16ToByteArray(int i, Endianness e);

		static int BufferInt16InToByteArray(int i, vector<unsigned char> &array, int indexToInsertElement, Endianness e);

		static int ConvertByteArrayToInt16(vector<unsigned char> array, Endianness e);
	};

}
