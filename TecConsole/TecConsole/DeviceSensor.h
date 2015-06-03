#ifndef _DEVICESENSOR_H
#define _DEVICESENSOR_H

//---------------------------------------------------------------------------
// Includes
//---------------------------------------------------------------------------
#include "FirmwareInfo.h"
#include "DeviceSensorIO.h"

//---------------------------------------------------------------------------
// Defines and Constants
//---------------------------------------------------------------------------


//---------------------------------------------------------------------------
// Structures & Enums
//---------------------------------------------------------------------------

typedef struct _DevicePrivateData
{	
	SENSOR_HANDLE  SensorHandle;
	FirmwareInfo	FWInfo;

}DevicePrivateData;

#endif