/*
 * adc.c
 *
 *  Created on: Feb 11, 2017
 *      Author: Warren Woolsey
 */

#include "msp430.h"
#include "driverlib.h"
#include "messagePassing.h"
#include "adc.h"
#include "mux.h"

static uint8_t adcHasStarted = 0;
static uint8_t adcReadNdx = 0;
static uint16_t adcReadBuff[NUM_CHANNELS_TO_READ];

void startNextADCConversion()
{
	adcHasStarted = true;
	ADC12_A_startConversion(ADC12_A_BASE, ADC12_A_MEMORY_15, ADC12_A_SINGLECHANNEL);
}

uint8_t hasADCStarted()
{
	return adcHasStarted;
}

void stopADCConversions()
{
	adcHasStarted = 0;
	ADC12_A_disableConversions(ADC12_A_BASE, false);
}

void serviceADC()
{
	if (!hasADCStarted())
		startNextADCConversion();

	if (Msg_GetADC12_A_ConversionComplete())
	{
		Msg_ClrADC12_A_ConversionComplete();
		adcReadBuff[adcReadNdx++] = Msg_GetADC12_A_ConversionResult();

		if (adcReadNdx >= NUM_CHANNELS_TO_READ)
		{
			Msg_SetPadData(adcReadBuff);
			Msg_SetNewPadDataAvailable();
			adcReadNdx = 0;
		}

		muxChannel(adcReadNdx);
		startNextADCConversion();
	}
}
