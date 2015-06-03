//
//Head files includes
//
#include <Windows.h>
//#include <usb.h>
//#include <stdio.h>
#include <SetupAPI.h>
//#include <tchar.h>
#include <strsafe.h>
#include "malloc.h"

#include "USBWin32.h"
#include "DevicePublicHfile.h"
#include "DevUSB.h"

//
//Functions
//

BOOL USBOpenDevice(UsbDevHandle** pDevHandlePtr)
{
	const GUID*		pInterguid = &GUID_CLASS_PSDRVUSB;
	HDEVINFO	devInfo = NULL;
	BOOL		success = TRUE;
	HANDLE      devHandle/* = INVALID_HANDLE_VALUE*/;
	UsbDevHandle*		pDevHandle;

	//Allocate the usb device handle data
	pDevHandle = *pDevHandlePtr;

	devInfo = SetupDiGetClassDevs(pInterguid,NULL,NULL, (DIGCF_PRESENT | DIGCF_DEVICEINTERFACE));
	if (devInfo == INVALID_HANDLE_VALUE)
	{
		printf("error: %x \n", HRESULT_FROM_WIN32(GetLastError()));
		return FALSE;
	}

	SP_DEVICE_INTERFACE_DATA	devInterfaceData;
	//PSP_DEVICE_INTERFACE_DATA	pdevInterfaceData;
	PSP_DEVICE_INTERFACE_DETAIL_DATA	pdevInterfaceDetailData = NULL;
	ULONG	RequiredSize = 0;
	LONG	index = 0;
	BOOL	search = TRUE;

	//Search the devices
	while (search)
	{
		//find the device
		printf("searching devices......%d \n",index);
		devInterfaceData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);
		success = SetupDiEnumDeviceInterfaces(devInfo,NULL,pInterguid,index,&devInterfaceData);
		if (success == FALSE)
		{
			if (GetLastError() == ERROR_NO_MORE_ITEMS)
			{
				printf("Failed,Reach the end, No more items \n");
				break;
			}
			//continue searching...
			search = TRUE;
		}

		//first to get the size
		success = SetupDiGetDeviceInterfaceDetail(devInfo,&devInterfaceData,NULL,0,&RequiredSize,NULL);
		pdevInterfaceDetailData = (PSP_DEVICE_INTERFACE_DETAIL_DATA)malloc(RequiredSize);
		pdevInterfaceDetailData->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);

		//now get the path
		success = SetupDiGetDeviceInterfaceDetail(devInfo,&devInterfaceData,pdevInterfaceDetailData,RequiredSize,&RequiredSize,NULL);
		if ((success == TRUE) | (GetLastError() != ERROR_NO_MORE_ITEMS))
		{
			printf("succeed! we had valid path\n");
		}
		index++;	
	}

	//get the handle
	devHandle=(void*)malloc(sizeof(LONG));
	devHandle = CreateFile(pdevInterfaceDetailData->DevicePath, 
		GENERIC_READ | GENERIC_WRITE, 
		FILE_SHARE_READ | FILE_SHARE_WRITE, 
		NULL, 
		OPEN_EXISTING, 
		FILE_FLAG_OVERLAPPED,/////////// 
		NULL);
	if (devHandle == INVALID_HANDLE_VALUE)
	{
		printf("Get device handle failed : %x \n",HRESULT_FROM_WIN32(GetLastError()));
		return FALSE;
	}
	//construct the path
	StringCchCopy(pDevHandle->DevPath,RequiredSize ,pdevInterfaceDetailData->DevicePath);
	//memcpy(pDevHandle->DevPath,pdevInterfaceDetailData->DevicePath,RequiredSize);


	//Save the device handle;
	pDevHandle->DevIOHandle = devHandle;

	//Mark the device handle as valid
	pDevHandle->DevOpen = TRUE;

	//Clean up memory
	SetupDiDestroyDeviceInfoList(devInfo);

	//All is good
	return TRUE;

}

BOOL USBSendControl(UsbDevHandle* pDevHandle, USBControlType nType, UCHAR nRequest, USHORT nValue, USHORT nIndex, UCHAR *pBuffer, UINT nBufferSize, UINT nTimeOut)
{
	//Variables
	BOOL bResutl = FALSE;
	ULONG	nRetBytes = 0;
	USBDEV_CONTROL_TRANSFER  ControlTransfer;

	//Init the control transfer structure
	ControlTransfer.cDirection = RequestHostToDevice;
	ControlTransfer.cRequestType = (UCHAR)nType;
	ControlTransfer.cRequest = nRequest;
	ControlTransfer.nValue = nValue;
	ControlTransfer.nIndex = nIndex;
	ControlTransfer.nTimeout = nTimeOut;

	//Send the control transfer
	bResutl = DeviceIoControl(pDevHandle->DevIOHandle,
		IOCTL_XDRV_CONTROL_TRANSFER,
		&ControlTransfer,
		sizeof(ControlTransfer),
		pBuffer,
		nBufferSize,
		&nRetBytes,
		NULL);
	if (bResutl != TRUE)
	{
		//Get error instruction
		printf("Error in sending control transfer; %x,\n",HRESULT_FROM_WIN32(GetLastError()));
		if (GetLastError() == ERROR_SEM_TIMEOUT)
		{
			printf("Transfer Timeout.\n");
		}
		return FALSE;
	}

	//Make sure we have sent all the bytes in the buffer
	if (nRetBytes != nBufferSize)
	{
		printf("Transaction got unexpected bytes.\n");
	}

	return bResutl;
}

BOOL USBReceiveControl(UsbDevHandle* pDevHandle, USBControlType nType, UCHAR nRequest, USHORT nValue, USHORT nIndex, UCHAR *pBuffer, UINT nBufferSize, UINT *pnBytesReceived, UINT nTimeOut)
{
	//Variables
	BOOL   bResutl = FALSE;
	USBDEV_CONTROL_TRANSFER ControlTransfer;

	//Make sure we are not sending an empty buffer
	if (nBufferSize == 0)
	{
		printf("Transfer Buffer too small.\n");
		return FALSE;
	}

	//Initial the control transfer structure
	ControlTransfer.cDirection = RequestDeviceToHost;
	ControlTransfer.cRequestType = (UCHAR)nType;
	ControlTransfer.cRequest = nRequest;
	ControlTransfer.nValue = nValue;
	ControlTransfer.nIndex = nIndex;
	ControlTransfer.nTimeout = nTimeOut;

	//Receive the control transfer
	bResutl = DeviceIoControl(pDevHandle->DevIOHandle, 
		IOCTL_XDRV_CONTROL_TRANSFER,
		&ControlTransfer,
		sizeof(ControlTransfer),
		pBuffer,
		nBufferSize,
		(DWORD*)pnBytesReceived,
		NULL);
	if (bResutl != TRUE)
	{
		//Get error instruction
		printf("Error in receiving control transfer; %x,\n",HRESULT_FROM_WIN32(GetLastError()));
		if (GetLastError() == ERROR_SEM_TIMEOUT)
		{
			printf("Transfer Timeout.\n");
		}
		return FALSE;
	}

	// Did we receive an empty buffer?
	if (*pnBytesReceived == 0)
	{
		printf("USB not enough data");
		return FALSE;
	} // Make sure we didn't get more then we asked for
	else if (*pnBytesReceived > nBufferSize)
	{
		printf("USB too much data");
		return FALSE;
	}
	//good
	return bResutl;
}