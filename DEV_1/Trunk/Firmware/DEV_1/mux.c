/*
 * mux.c
 *
 *  Created on: Feb 12, 2017
 *      Author: Warren Woolsey
 */

#include "msp430.h"
#include "driverlib.h"
#include "mux.h"

#define MUX_PORT	GPIO_PORT_P1
#define MUX_A0		GPIO_PIN0
#define MUX_A1		GPIO_PIN1
#define MUX_A2		GPIO_PIN2
#define MUX_A3		GPIO_PIN3
#define MUX_ALL		(MUX_A0 | MUX_A1 | MUX_A2 | MUX_A3)

void clearAllChannels()
{
	GPIO_setOutputLowOnPin(MUX_PORT, (MUX_ALL));
}

void muxChannel(uint8_t channel)
{
	clearAllChannels();

	switch (channel)
	{
	case 0:
		// Nothing to do
	break;
	case 1:
		// TODO: Fixes a hardware mixup
		GPIO_setOutputHighOnPin(MUX_PORT, MUX_A1);
	break;
	case 2:
		// TODO: Fixes a hardware mixup
		GPIO_setOutputHighOnPin(MUX_PORT, MUX_A0);
	break;
	case 3:
		GPIO_setOutputHighOnPin(MUX_PORT, MUX_A0 | MUX_A1);
	break;
	case 4:
		GPIO_setOutputHighOnPin(MUX_PORT, MUX_A2);
	break;
	case 5:
		// TODO: Fixes a hardware mixup
		GPIO_setOutputHighOnPin(MUX_PORT, MUX_A2 | MUX_A1);
	break;
	case 6:
		// TODO: Fixes a hardware mixup
		GPIO_setOutputHighOnPin(MUX_PORT, MUX_A2 | MUX_A0);
	break;
	case 7:
		GPIO_setOutputHighOnPin(MUX_PORT, MUX_A2 | MUX_A1 | MUX_A0);
	break;
	case 8:
		GPIO_setOutputHighOnPin(MUX_PORT, MUX_A3);
	break;
	case 9:
		GPIO_setOutputHighOnPin(MUX_PORT, MUX_A3 | MUX_A0);
	break;
	default:
		break;
	}
}


