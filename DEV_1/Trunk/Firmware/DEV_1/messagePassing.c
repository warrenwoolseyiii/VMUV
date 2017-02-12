/*
 * messagePassing.c
 *
 *  Created on: Feb 11, 2017
 *      Author: Warren Woolsey
 */

#include "msp430.h"
#include "driverlib.h"
#include "messagePassing.h"

static volatile uint8_t timer_A_InterruptNotification = false;

static volatile uint8_t adc12_A_ConversionComplete = false;
static volatile uint16_t adc12_A_ConversionResult = 0;

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
