//********************************************************/
//Head files
//********************************************************/
#include "SensorPreheat.h"

//********************************************************/
//Functions declaration and definitions
//********************************************************/
BOOL DeviceConnectInquiry(DevDetailPtr pDeviceDetailData, INT DevIndex)
{
	const GUID*		pInterguid = &GUID_CLASS_OBDRV_USB;
	HDEVINFO		devInfo = NULL;
	BOOL			DevConnectStatus;
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
	DevConnectStatus = SetupDiEnumDeviceInfo(devInfo, DevIndex, &pDeviceInfoData);
	if (DevConnectStatus == FALSE()/* & (GetLastError() == ERROR_NO_MORE_ITEMS)*/)
	{
		//printf("%d connection lost \n", DevIndex);
		return FALSE;
	}
	//Now clear the info list
	SetupDiDestroyDeviceInfoList(devInfo);
	return TRUE;
}