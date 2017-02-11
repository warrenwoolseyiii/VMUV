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


#define LED_PORT    GPIO_PORT_P1
#define LED_PIN     GPIO_PIN0

typedef struct {
	uint16_t padValuesInCnts[9];
} t_DEV_1_Rpt;

t_DEV_1_Rpt dev1Rpt;        // HID report, to be sent to the PC.
uint16_t gTestPadVal = 0;

void initDEV1();

void main (void)
{
	initDEV1();

	while (1)
	{
		// Check the USB state and directly main loop accordingly
		switch (USB_getConnectionState())
		{
		// This case is executed while your device is enumerated on the
		// USB host
		case ST_ENUM_ACTIVE:

			// Start Timer
			Timer_A_startCounter(TIMER_A0_BASE,
					TIMER_A_UP_MODE);

			if (Msg_GetTimer_A_InterruptNotification())
			{
				Msg_ClrTimer_A_InterruptNotification();
				// Build the report
				// TODO:
				{
					int i;
					for (i = 0; i < 9; i++)
					{
						if (i % 2)
							dev1Rpt.padValuesInCnts[i] = gTestPadVal;
						else
							dev1Rpt.padValuesInCnts[i] = (4095 - gTestPadVal);
					}

					gTestPadVal++;
					if (gTestPadVal > 4095)
						gTestPadVal = 0;
				}

				// Send the report
				USBHID_sendReport((void *)&dev1Rpt, HID0_INTFNUM);

				// Toggle LED on P1.0
				GPIO_toggleOutputOnPin(LED_PORT, LED_PIN);
			}
			break;


			// These cases are executed while your device is disconnected from
			// the host (meaning, not enumerated); enumerated but suspended
			// by the host, or connected to a powered hub without a USB host
			// present.
		case ST_PHYS_DISCONNECTED:
		case ST_ENUM_SUSPENDED:
		case ST_PHYS_CONNECTED_NOENUM_SUSP:
			TA0CTL &= ~MC_1;
			P1OUT &= ~BIT0;
			__bis_SR_register(LPM3_bits + GIE);
			_NOP();
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
}

void initDEV1()
{
	WDT_A_hold(WDT_A_BASE);

	// Minumum Vcore setting required for the USB API is PMM_CORE_LEVEL_2 .
	PMM_setVCore(PMM_CORE_LEVEL_2);
	USBHAL_initPorts();
	USBHAL_initClocks(8000000);   // Config clocks. MCLK=SMCLK=FLL=8MHz; ACLK=REFO=32kHz
	USBHAL_initTimer_A();
	USB_setup(TRUE, TRUE); // Init USB & events; if a host is present, connect

	__enable_interrupt();
}
