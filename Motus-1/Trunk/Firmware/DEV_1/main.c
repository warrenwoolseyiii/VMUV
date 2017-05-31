/* --COPYRIGHT--,BSD
 * Copyright (c) 2014, Texas Instruments Incorporated
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * *  Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 *
 * *  Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 *
 * *  Neither the name of Texas Instruments Incorporated nor the names of
 *    its contributors may be used to endorse or promote products derived
 *    from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
 * OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
 * EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * --/COPYRIGHT--*/

#include <string.h>
#include "driverlib.h"
#include "USB_config/descriptors.h"
#include "USB_API/USB_Common/device.h"
#include "USB_API/USB_Common/usb.h"                 // USB-specific functions
#include "USB_API/USB_HID_API/UsbHid.h"
#include "hal.h"
#include "messagePassing.h"
#include "adc.h"


#define LED_PORT    GPIO_PORT_P6
#define LED_PIN1    GPIO_PIN4
#define LED_PIN2	GPIO_PIN5

typedef struct
{
	uint16_t padValuesInCnts[9];
} t_DEV_1_Rpt;

t_DEV_1_Rpt gDev1Rpt;

void initDEV1();
void usbStateMain();
void servicePeripherals();
void serviceUSBConnection();
void serviceUSBDisconnect();

void main (void)
{
	initDEV1();

	while (1)
		usbStateMain();
}

void initDEV1()
{
	WDT_A_hold(WDT_A_BASE);

	// Minumum Vcore setting required for the USB API is PMM_CORE_LEVEL_2 .
	PMM_setVCore(PMM_CORE_LEVEL_2);
	USBHAL_initPorts();
	USBHAL_initClocks(8000000);   // Config clocks. MCLK=SMCLK=FLL=8MHz; ACLK=REFO=32kHz
	USBHAL_initADC12_A();
	USB_setup(TRUE, TRUE); // Init USB & events; if a host is present, connect

	__enable_interrupt();
}

void usbStateMain()
{
	switch (USB_getConnectionState())
	{
	case ST_ENUM_ACTIVE:
		servicePeripherals();
		serviceUSBConnection();
		break;

		// These cases are executed while your device is disconnected from
		// the host (meaning, not enumerated); enumerated but suspended
		// by the host, or connected to a powered hub without a USB host
		// present.
	case ST_PHYS_DISCONNECTED:
	case ST_ENUM_SUSPENDED:
	case ST_PHYS_CONNECTED_NOENUM_SUSP:
		serviceUSBDisconnect();
		break;

		// The default is executed for the momentary state
		// ST_ENUM_IN_PROGRESS.  Usually, this state only last a few
		// seconds.  Be sure not to enter LPM3 in this state; USB
		// communication is taking place here, and therefore the mode must
		// be LPM0 or active-CPU.
	case ST_ENUM_IN_PROGRESS:
	default:;
	}
}

void servicePeripherals()
{
	serviceADC();
}

void serviceUSBConnection()
{
	if (Msg_GetNewPadDataAvailable())
	{
		Msg_ClrNewPadDataAvailable();
		Msg_GetPadData(gDev1Rpt.padValuesInCnts);
		USBHID_sendReport((void *)&gDev1Rpt, HID0_INTFNUM);
		GPIO_toggleOutputOnPin(LED_PORT, LED_PIN1);
	}
}

void serviceUSBDisconnect()
{
	stopADCConversions();
	__bis_SR_register(LPM3_bits + GIE);
	_NOP();
}
