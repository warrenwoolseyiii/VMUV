/*
 * messagePassing.h
 *
 *  Created on: Feb 11, 2017
 *      Author: Warren Woolsey
 */

#ifndef MESSAGEPASSING_H_
#define MESSAGEPASSING_H_

#include <stdint.h>

void Msg_SetTimer_A_InterruptNotification();
uint8_t Msg_GetTimer_A_InterruptNotification();
void Msg_ClrTimer_A_InterruptNotification();

void Msg_SetADC12_A_ConversionComplete();
uint8_t Msg_GetADC12_A_ConversionComplete();
void Msg_ClrADC12_A_ConversionComplete();
void Msg_SetADC12_A_ConversionResult(uint16_t result);
uint16_t Msg_GetADC12_A_ConversionResult();

void Msg_SetNewPadDataAvailable();
uint8_t Msg_GetNewPadDataAvailable();
void Msg_ClrNewPadDataAvailable();
void Msg_SetPadData(uint16_t *data);
void Msg_GetPadData(uint16_t *data);

#endif /* MESSAGEPASSING_H_ */
