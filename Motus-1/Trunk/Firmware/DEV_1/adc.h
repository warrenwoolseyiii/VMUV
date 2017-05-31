/*
 * adc.h
 *
 *  Created on: Feb 11, 2017
 *      Author: Warren Woolsey
 */

#ifndef ADC_H_
#define ADC_H_

#define NUM_CHANNELS_TO_READ	9	// 9 Pads

void startNextADCConversion();
uint8_t hasADCStarted();
void stopADCConversions();
void serviceADC();

#endif /* ADC_H_ */
