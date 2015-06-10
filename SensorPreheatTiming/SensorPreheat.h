#ifndef _SENSOR_PRE_HEAT_HEAD_
#define _SENSOR_PRE_HEAT_HEAD_
//********************************************************/
//Head files
//********************************************************/
//System head files
#include <Windows.h>
#include <SetupAPI.h>
#include <strsafe.h>
#include <malloc.h>
#include <iostream>

//User head files
#include "DevUsbGUID.h"

//********************************************************/
//Structures 
//********************************************************/
typedef struct _USB_Device_Detail
{
	//Create an array in order to save the device path
	TCHAR	DevPathArray[10][256];//MAX_DEVICE_STR_LENGTH = 256, support maximum 10 devices
	INT		DeviceIndex;
}DevDetail, *DevDetailPtr;


#endif	//end _SENSOR_PRE_HEAT_HEAD_