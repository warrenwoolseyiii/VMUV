/*
 * messagePassing.c
 *
 *  Created on: Feb 11, 2017
 *      Author: Warren Woolsey
 */

#include "msp430.h"
#include "driverlib.h"
#include "messagePassing.h"

#define NUM_PADS 9

static volatile uint8_t timer_A_InterruptNotification = false;

static volatile uint8_t adc12_A_ConversionComplete = false;
static volatile uint16_t adc12_A_ConversionResult = 0;

static uint8_t newPadDataAvailable = false;
volatile uint16_t padData[NUM_PADS];

// TIMER A
void Msg_SetTimer_A_InterruptNotification()
{
	timer_A_InterruptNotification = true;
}

uint8_t Msg_GetTimer_A_InterruptNotification()
{
	return timer_A_InterruptNotification;
}

void Msg_ClrTimer_A_InterruptNotification()
{
	timer_A_InterruptNotification = false;
}

// ADC12
void Msg_SetADC12_A_ConversionComplete()
{
	adc12_A_ConversionComplete = true;
}

uint8_t Msg_GetADC12_A_ConversionComplete()
{
	return adc12_A_ConversionComplete;
}

void Msg_ClrADC12_A_ConversionComplete()
{
	adc12_A_ConversionComplete = false;
}

void Msg_SetADC12_A_ConversionResult(uint16_t result)
{
	adc12_A_ConversionResult = result;
}

uint16_t Msg_GetADC12_A_ConversionResult()
{
	return adc12_A_ConversionResult;
}

// Pad Data
void Msg_SetNewPadDataAvailable()
{
	newPadDataAvailable = true;
}

uint8_t Msg_GetNewPadDataAvailable()
{
	return newPadDataAvailable;
}

void Msg_ClrNewPadDataAvailable()
{
	newPadDataAvailable = false;
}

void Msg_SetPadData(uint16_t *data)
{
	memcpy((void *)padData, (void *)data, (sizeof(uint16_t) * NUM_PADS));
}

void Msg_GetPadData(uint16_t *data)
{
	memcpy((void *)data, (void *)padData, (sizeof(uint16_t) * NUM_PADS));
}
