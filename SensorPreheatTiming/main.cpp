//********************************************************/
//Head files
//********************************************************/
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

//********************************************************/
//Functions declaration and definitions
//********************************************************/
BOOL GetUSBDevicePath(DevDetailPtr DevPathPtr)
{
	const GUID*		pInterguid = &GUID_LCASS_OBDRVUSB;
	HDEVINFO		devInfo = NULL;

	devInfo = SetupDiGetClassDevs(pInterguid, NULL, NULL, (DIGCF_PRESENT | DIGCF_DEVICEINTERFACE));
	if (devInfo == INVALID_HANDLE_VALUE)
	{
		printf("Error: %x \n", HRESULT_FROM_WIN32(GetLastError()));
		return FALSE;
	}

	SP_DEVICE_INTERFACE_DATA	devInterfaceData;
	PSP_DEVICE_INTERFACE_DETAIL_DATA	pdevInterfaceDetailData = NULL;
	


	//Searching the devices 
	ULONG	RequiredSize = 0;
	BOOL	devsearch = TRUE;
	BOOL	searchingResult = FALSE;

	while(devsearch)
	{
		//printf("searching devices......%d \n",DevPathPtr->DeviceIndex);
		devInterfaceData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);
		searchingResult = SetupDiEnumDeviceInterfaces(devInfo, NULL, pInterguid, 
													  DevPathPtr->DeviceIndex, 
													  &devInterfaceData);
		if (searchingResult == TRUE)
		{
			//First to get the size
			searchingResult = SetupDiGetDeviceInterfaceDetail(devInfo, 
															  &devInterfaceData, 
															  NULL, 0, 
															  &RequiredSize, NULL);
			pdevInterfaceDetailData = (PSP_DEVICE_INTERFACE_DETAIL_DATA)malloc(RequiredSize);
			pdevInterfaceDetailData->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);

			//Then get the path
			searchingResult = SetupDiGetDeviceInterfaceDetail(devInfo, &devInterfaceData, 
															  pdevInterfaceDetailData, 
															  RequiredSize, 
															  &RequiredSize, NULL);
			if ((searchingResult == TRUE) | (GetLastError() != ERROR_NO_MORE_ITEMS))
			{
				//printf("succeed! we had valid path\n");
			}
			//Save the path
			StringCchCopy(DevPathPtr->DevPathArray[DevPathPtr->DeviceIndex],RequiredSize,
						  pdevInterfaceDetailData->DevicePath);
			DevPathPtr->DeviceIndex++;
		}
		else if (GetLastError() == ERROR_NO_MORE_ITEMS)
		{
			devsearch = FALSE;
			//printf("Failed,Reach the end, No more items \n");
			break;
		}
	}
	return searchingResult;
}

BOOL DeviceInquiryLoop(DevDetailPtr DevPathPtr)
{
	BOOL	CheckConnection = TRUE;
	void*	devHandle;
	INT		InquiryTimes = 0;
	while(InquiryTimes < DevPathPtr->DeviceIndex)
	{
		devHandle = CreateFile(DevPathPtr->DevPathArray[InquiryTimes], 
			GENERIC_READ | GENERIC_WRITE, 
			FILE_SHARE_READ | FILE_SHARE_WRITE, 
			NULL, 
			OPEN_EXISTING, 
			FILE_FLAG_OVERLAPPED,
			NULL);
		if (devHandle == INVALID_HANDLE_VALUE)
		{
			printf("Connection lost! PortNumber: %d Error code : %x \n",DevPathPtr->DeviceIndex - 1 ,HRESULT_FROM_WIN32(GetLastError()));
			CheckConnection = FALSE;
			//break;
		}
		InquiryTimes++;
	}
	return CheckConnection;
}

//********************************************************/
//     MAIN
//********************************************************/

int main()
{
	BOOL  RetVal = FALSE;
	const INT	  INQUIRY_INTERVALS = 1000;//Milliseconds
	DevDetail		DevDetailStorage;
	DevDetailPtr    DevPathPtr = &DevDetailStorage;

	while (!(GetKeyState(0x51) & 0x8000))
	{
		DevDetailStorage.DeviceIndex = 0;//Fixed value for start of device searching 
		RetVal = GetUSBDevicePath(DevPathPtr);
		printf("Number of devices detected : %d \n",DevPathPtr->DeviceIndex);
		RetVal = DeviceInquiryLoop(DevPathPtr);
		//True if all devices inquiries success
		if (RetVal == TRUE)
		{

		}
		Sleep(INQUIRY_INTERVALS);
		//press q to exit
		//0x51 stands for key "q"
	}
	printf("program exit...... \n");
	


// 	if (RetVal == FALSE)
// 	{
// 		printf("Get device path failed!\n");
// 	}

//	printf("Press any key to exit...... \n");
	int i = 0;
	std::cin >> i;
// 	while (TRUE)
// 	{
// 		//0x51 stands for key "q",
// 		//The highest bit tells if key is pressed. The lowest tells if key is toggled
// 		if (GetKeyState(0x51) & 0x8000)
// 		{
// 			break;
// 		}
// 	}
	return 0;
}