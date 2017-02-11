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


