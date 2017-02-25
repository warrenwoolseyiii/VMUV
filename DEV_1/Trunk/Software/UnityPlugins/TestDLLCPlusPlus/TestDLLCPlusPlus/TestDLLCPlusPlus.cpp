// TestDLLCPlusPlus.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "TestDLLCPlusPlus.h"

using namespace Windows::Devices;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Devices::HumanInterfaceDevice;

__declspec(dllexport) int Add(int a, int b)
{
    return a + b;
}
