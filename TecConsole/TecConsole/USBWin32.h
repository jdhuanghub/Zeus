#ifndef _USBWIN32_H
#define _USBWIN32_H

//---------------------------------------------------------------------------
// Includes
//---------------------------------------------------------------------------
//#include <SDKDDKVer.h>
//#include <WinDef.h>
//#include <string>
//---------------------------------------------------------------------------
// Constants
//---------------------------------------------------------------------------
const int MAX_DEVICE_STR_LENGTH = 256;
//---------------------------------------------------------------------------
// Structures & Enum struct
//---------------------------------------------------------------------------
typedef struct _UsbDeviceHandle
{
	BOOL	DevOpen;
	HANDLE	DevIOHandle;
	CHAR	DevPath[MAX_DEVICE_STR_LENGTH];

}UsbDevHandle;


#endif 