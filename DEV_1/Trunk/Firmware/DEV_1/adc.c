/*
 * adc.c
 *
 *  Created on: Feb 11, 2017
 *      Author: Warren Woolsey
 */

#include "msp430.h"
#include "driverlib.h"
#include "adc.h"

static uint8_t adcHasStarted = 0;

void startADC()
{
	adcHasStarted = true;
	ADC12_A_startConversion(ADC12_A_BASE, ADC12_A_MEMORY_15, ADC12_A_REPEATED_SINGLECHANNEL);
}

uint8_t hasADCStarted()
{
	return adcHasStarted;
}

void stopADC()
{
	adcHasStarted = 0;
	ADC12_A_disableConversions(ADC12_A_BASE, false);
}

