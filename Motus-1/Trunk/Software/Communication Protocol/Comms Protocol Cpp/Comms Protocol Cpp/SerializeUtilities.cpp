// SerializeUtilities.cpp

#include "stdafx.h"

#include "SerializeUtilities.h"

#define Int16MaxValue 32767
#define Int16MinValue -32768

static vector<unsigned char> _rtn;

vector<unsigned char> Comms_Protocol_Cpp::SerializeUtilities::ConvertInt16ToByteArray(int i, Endianness e)
{
	if (i > Int16MaxValue)
		i = Int16MaxValue;
	else if (i < Int16MinValue)
		i = Int16MinValue;

	vector<unsigned char> rtn(2);
	if (e == little_endian) {
		rtn[0] = (unsigned char)(i & 0xFF);
		rtn[1] = (unsigned char)((i >> 8) & 0xFF);
	}
	else {
		rtn[1] = (unsigned char)(i & 0xFF);
		rtn[0] = (unsigned char)((i >> 8) & 0xFF);
	}

	return rtn;
}

int Comms_Protocol_Cpp::SerializeUtilities::BufferInt16InToByteArray(int i, vector<unsigned char> &array, int indexToInsertElement, Endianness e)
{
//	try
	// just prevent overrun
	if (array.size() >= indexToInsertElement+2)
	{
		vector<unsigned char> vals(2);
		vals = ConvertInt16ToByteArray(i, e);
		array.at(indexToInsertElement) = vals.at(0);
		array.at(indexToInsertElement+1) = vals.at(1);
		indexToInsertElement+=2;	// only increment if no exception was thrown by the vector at calls
	}
//catch (IndexOutOfRangeException) {}
	return indexToInsertElement;
}

// note: I think the original CSharp code doesnt seem to check if the original value was negative,
// so allways returns a positive value: -20,536 becomes 0xAFC8 becomes 45000 which is clearly wrong!
// I am retaining that behavior here, but we may want to fix it...
int Comms_Protocol_Cpp::SerializeUtilities::ConvertByteArrayToInt16(vector<unsigned char> array, Endianness e)
{
	int rtn = 0;
//	_try
	// just prevent overrun
	if (array.size() >= 2)
	{
		if (e == little_endian)
		{
			rtn = (short)(array.at(1));
			rtn <<= 8;
			rtn |= (short)(array.at(0));
		}
		else
		{
			rtn = (short)(array.at(0));
			rtn <<= 8;
			rtn |= (short)(array.at(1));
		}
	}
	//catch (IndexOutOfRangeException) {}
	return rtn;
}

