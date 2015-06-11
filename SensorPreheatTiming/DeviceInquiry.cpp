//********************************************************/
//Head files
//********************************************************/
#include "SensorPreheat.h"

//********************************************************/
//Functions declaration and definitions
//********************************************************/

//This method can get the amount of devices and see if there is 
//a connection lost, it CANNOT know which one is unplug.
BOOL DeviceConnectInquiry(INT &PreDevIndex,INT &CurrentDevCount)
{
	const GUID*		pInterguid = &GUID_CLASS_OBDRV_USB;
	HDEVINFO		devInfo = NULL;
	BOOL			DevConnectStatus;
	BOOL			NewDevice = FALSE;// False if no new device plug in, set to be false as default
	CurrentDevCount = 0;
	// Enumerate all devices exposing the interface
	devInfo = SetupDiGetClassDevs(pInterguid, NULL, NULL, (DIGCF_PRESENT | DIGCF_DEVICEINTERFACE));
	if (devInfo == INVALID_HANDLE_VALUE)
	{
		printf("Error! No devices found : %x \n", HRESULT_FROM_WIN32(GetLastError()));
		return FALSE;
	}
	//Get the interface of particular index in the index lists
	SP_DEVINFO_DATA		pDeviceInfoData;
	pDeviceInfoData.cbSize = sizeof(SP_DEVINFO_DATA);	
	while(TRUE)
	{
		DevConnectStatus = SetupDiEnumDeviceInfo(devInfo, CurrentDevCount, &pDeviceInfoData);
		if ((DevConnectStatus == FALSE()) & (GetLastError() == ERROR_NO_MORE_ITEMS))
		{
			//printf("%d connection lost \n", DevIndex);
			break;
		}
		CurrentDevCount++;
	}
	INT PreviousDevCount = PreDevIndex + 1;
	if (PreviousDevCount < CurrentDevCount)
	{
		NewDevice = TRUE;
	}
	
	//Now clear the info list
	SetupDiDestroyDeviceInfoList(devInfo);
	return NewDevice;
}

//Use the device path to check if the device is currently connecting.
BOOL DevicePathValidate(DevDetailPtr pDeviceDetailData, INT DevIndex)
{
	BOOL	PathValidation = TRUE;
	HANDLE	devHandle;
	
	devHandle = CreateFile(pDeviceDetailData->DevPathArray[DevIndex],
						   GENERIC_READ | GENERIC_WRITE,
						   FILE_SHARE_READ | FILE_SHARE_WRITE,
						   NULL, OPEN_EXISTING,
						   FILE_FLAG_OVERLAPPED, NULL);

	if (devHandle == INVALID_HANDLE_VALUE)
	{
		printf("Port device connection lost\n");
		PathValidation = FALSE;
	}

	return PathValidation;
}

//After finding out which one is disconnected, get its path id string.
BOOL DevicePathIDParsing(DevDetailPtr pDeviceDetailData, INT DevIndex)
{
	//print the unique port ID
	INT		DevUSBPortID[20];
	INT		IdStartIndex;
	INT		IdEndIndex;

	printf("USB Port ID : ");
	CHAR	*pathStringSearch;
	pathStringSearch = strchr(pDeviceDetailData->DevPathArray[DevIndex],'&');//Fist '&' is not for the id

	pathStringSearch = strchr(pathStringSearch +1, '&');//The second '&" is the start of the id
	IdStartIndex = pathStringSearch - pDeviceDetailData->DevPathArray[DevIndex];
	pathStringSearch = strchr(pathStringSearch +1, '&');//The third '&' is the end of the id
	IdEndIndex = pathStringSearch - pDeviceDetailData->DevPathArray[DevIndex];

	for (int j = IdStartIndex; j <= IdEndIndex; j++)
	{
		DevUSBPortID[j - IdStartIndex] = pDeviceDetailData->DevPathArray[DevIndex][j];
		printf("%c",DevUSBPortID[j - IdStartIndex]);
	}
	printf("\n");
	return TRUE;
}