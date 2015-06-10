//********************************************************/
//Head files
//********************************************************/
#include "SensorPreheat.h"

//********************************************************/
//Functions declaration and definitions
//********************************************************/
BOOL DevPathSearching(DevDetailPtr pDeviceDetailData)
{
	const GUID*		pInterguid = &GUID_CLASS_OBDRV_USB;
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
	pDeviceDetailData->DeviceIndex = 0;//usb interface starting index

	while(devsearch)
	{
		//printf("searching devices......%d \n",DevPathPtr->DeviceIndex);
		devInterfaceData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);
		searchingResult = SetupDiEnumDeviceInterfaces(devInfo, NULL, pInterguid, 
													  pDeviceDetailData->DeviceIndex, 
													  &devInterfaceData);
		if (searchingResult == TRUE)
		{
			//First to get the size
			searchingResult = SetupDiGetDeviceInterfaceDetail(devInfo, &devInterfaceData, 
															  NULL, 0, 
															  &RequiredSize, NULL);
			pdevInterfaceDetailData = (PSP_DEVICE_INTERFACE_DETAIL_DATA)malloc(RequiredSize);
			pdevInterfaceDetailData->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);

			//Then get the path
			searchingResult = SetupDiGetDeviceInterfaceDetail(devInfo, &devInterfaceData, 
															  pdevInterfaceDetailData, 
															  RequiredSize, 
															  &RequiredSize, NULL);
// 			if ((searchingResult == TRUE) | (GetLastError() != ERROR_NO_MORE_ITEMS))
// 			{
// 				printf("succeed! we had valid path\n");
// 			}
			//Save the path
			StringCchCopy(pDeviceDetailData->DevPathArray[pDeviceDetailData->DeviceIndex],
						  RequiredSize,
						  pdevInterfaceDetailData->DevicePath);
			pDeviceDetailData->DeviceIndex++;
		}
		else if (GetLastError() == ERROR_NO_MORE_ITEMS)
		{
			devsearch = FALSE;
			pDeviceDetailData->DeviceIndex = pDeviceDetailData->DeviceIndex - 1;
			//printf("Failed,Reach the end, No more items \n");
			break;
		}
	}
	SetupDiDestroyDeviceInfoList(devInfo);
	return searchingResult;
}