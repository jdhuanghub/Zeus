//********************************************************/
//Head files
//********************************************************/
#include <Windows.h>
#include <SetupAPI.h>
#include <strsafe.h>
#include <malloc.h>
//User head files
#include "DevUsbGUID.h"

//********************************************************/
//Structures 
//********************************************************/
typedef struct _USB_Device_Detail
{
	//Create an array in order to save the device path
	CHAR	DevPathIndex[20][256];//MAX_DEVICE_STR_LENGTH = 256, support maximum 10 devices
}DevDetail;

//********************************************************/
//Functions declaration and definitions
//********************************************************/
BOOL GetUSBDevicePath()
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
	INT		devIndex = 0;
	BOOL	devsearch = TRUE;
	BOOL	searchingResult = FALSE;

	while(devsearch)
	{
		printf("searching devices......%d \n",devIndex);
		devInterfaceData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);
		searchingResult = SetupDiEnumDeviceInterfaces(devInfo, NULL, pInterguid, 
													  devIndex, &devInterfaceData);
		if (searchingResult == TRUE)
		{
			//First to get the size
			searchingResult = SetupDiGetDeviceInterfaceDetail(devInfo, 
															  &devInterfaceData, 
															  NULL, 0, 
															  &RequiredSize, NULL);
			pdevInterfaceDetailData = (PSP_DEVICE_INTERFACE_DETAIL_DATA)malloc(RequiredSize);
			pdevInterfaceDetailData->cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);

			//Then get the path
			searchingResult = SetupDiGetDeviceInterfaceDetail(devInfo, &devInterfaceData, 
															  pdevInterfaceDetailData, 
															  RequiredSize, 
															  &RequiredSize, NULL);
			if ((searchingResult == TRUE) | (GetLastError() != ERROR_NO_MORE_ITEMS))
			{
				printf("succeed! we had valid path\n");
			}
			//Save the path
			StringCchCopy(DevPathIndex[devIndex][256],RequiredSize,pdevInterfaceDetailData->DevicePath);
			devIndex++;
		}
		else if (GetLastError() == ERROR_NO_MORE_ITEMS)
		{
			devsearch = FALSE;
			printf("Failed,Reach the end, No more items \n");
			break;
		}
	}
	return searchingResult;
}

//********************************************************/
//Functions declaration and definitions
//********************************************************/

int main()
{
	BOOL  RetVal = FALSE;

	RetVal = GetUSBDevicePath();
	
	if (RetVal == FALSE)
	{
		printf("Get device path failed!\n");
	}

	printf("Press q to exit...... \n");
	while (TRUE)
	{
		//0x51 stands for key "q",
		//The highest bit tells if key is pressed. The lowest tells if key is toggled
		if (GetKeyState(0x51) & 0x8000)
		{
			break;
		}
	}
	return 0;
}