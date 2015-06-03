#ifndef _DEVICEPUBLICHFILE_H
#define _DEVICEPUBLICHFILE_H

//---------------------------------------------------------------------------
// Includes
//---------------------------------------------------------------------------
#include <initguid.h>

//---------------------------------------------------------------------------
// GUIDs
//---------------------------------------------------------------------------
// c3b5f022-5a42-1980-1909-ea7209560
DEFINE_GUID(GUID_CLASS_PSDRVUSB, 0xc3b5f022, 0x5a42, 0x1980, 0x19, 0x09, 0xea, 0x72, 0x09, 0x56, 0x01, 0xb1);

//Define Control code index****************************************************
#define IOCTL_INDEX             0x0000
//Define device get_config control CODE
#define IOCTL_XDRV_GET_CONFIG_DESCRIPTOR CTL_CODE(FILE_DEVICE_UNKNOWN,     \
	IOCTL_INDEX + 0,			\
	METHOD_BUFFERED,         \
	FILE_ANY_ACCESS)
//Control transfer
#define IOCTL_XDRV_CONTROL_TRANSFER      CTL_CODE(FILE_DEVICE_UNKNOWN,     \
	IOCTL_INDEX + 4,			\
	METHOD_OUT_DIRECT,       \
	FILE_ANY_ACCESS)
//Define device version control CODE
#define IOCTL_PSDRV_GETDRIVER_VERSION    CTL_CODE(FILE_DEVICE_UNKNOWN,     \
	IOCTL_INDEX + 6,			\
	METHOD_BUFFERED,         \
	FILE_ANY_ACCESS)
//Define speed control CODE
#define IOCTL_XDRV_GETDEVICE_SPEED    CTL_CODE(FILE_DEVICE_UNKNOWN,		\
	IOCTL_INDEX + 7,			\
	METHOD_BUFFERED,         \
	FILE_ANY_ACCESS)
//Define the get interface function control CODE
#define IOCTL_PSDRV_GET_INTERFACE		CTL_CODE(FILE_DEVICE_UNKNOWN,		\
	IOCTL_INDEX + 9,			\
	METHOD_BUFFERED,         \
	FILE_ANY_ACCESS)



//---------------------------------------------------------------------------
// Structs & Enums
//---------------------------------------------------------------------------
typedef enum _USBDEV_CONTROL_REQUEST_DIRECTION
{
	RequestHostToDevice = 0,
	RequestDeviceToHost,
} USBDEV_CONTROL_REQUEST_DIRECTION;

typedef struct _USBDEV_CONTROL_TRANSFER
{
	unsigned char  cDirection;
	unsigned char  cRequestType;
	unsigned char  cRequest;
	unsigned short nValue;
	unsigned short nIndex;
	unsigned int   nTimeout;
}USBDEV_CONTROL_TRANSFER;


#endif