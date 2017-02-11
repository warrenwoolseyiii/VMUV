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

#endif /* MESSAGEPASSING_H_ */
