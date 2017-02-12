/*
 * isr.c
 *
 *  Created on: Feb 11, 2017
 *      Author: Warren Woolsey
 */

#include "msp430.h"
#include "driverlib.h"
#include "USB_API/USB_Common/usb.h"
#include "messagePassing.h"

#pragma vector = UNMI_VECTOR
__interrupt void UNMI_ISR (void)
{
	switch (__even_in_range(SYSUNIV, SYSUNIV_BUSIFG ))
	{
	case SYSUNIV_NONE:
		__no_operation();
		break;
	case SYSUNIV_NMIIFG:
		__no_operation();
		break;
	case SYSUNIV_OFIFG:
		UCS_clearFaultFlag(UCS_XT2OFFG);
		UCS_clearFaultFlag(UCS_DCOFFG);
		SFR_clearInterrupt(SFR_OSCILLATOR_FAULT_INTERRUPT);
		break;
	case SYSUNIV_ACCVIFG:
		__no_operation();
		break;
	case SYSUNIV_BUSIFG:
		// If the CPU accesses USB memory while the USB module is
		// suspended, a "bus error" can occur.  This generates an NMI.  If
		// USB is automatically disconnecting in your software, set a
		// breakpoint here and see if execution hits it.  See the
		// Programmer's Guide for more information.
		SYSBERRIV = 0; //clear bus error flag
		USB_disable(); //Disable
	}
}

#pragma vector=TIMER0_A0_VECTOR
__interrupt void TIMER0_A0_ISR (void)
{
	Msg_SetTimer_A_InterruptNotification();
}

#pragma vector=ADC12_VECTOR
__interrupt void ADC12ISR (void)
{
	Msg_SetADC12_A_ConversionResult(ADC12_A_getResults(ADC12_A_BASE, ADC12_A_MEMORY_15));
	Msg_SetADC12_A_ConversionComplete();
}
