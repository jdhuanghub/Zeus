//********************************************************/
//Head files
//********************************************************/
#include "SensorPreheat.h"

//********************************************************/
//Function declaration
//********************************************************/
BOOL DevPathSearching(DevDetailPtr pDeviceDetailData);
BOOL DeviceConnectInquiry(INT &PreviousDevCount,INT &CurrentDevCount);
BOOL DevicePathValidate(DevDetailPtr pDeviceDetailData, INT DevIndex);
BOOL DevicePathIDParsing(DevDetailPtr pDeviceDetailData, INT DevIndex);
//********************************************************/
//     MAIN
//********************************************************/

int main()
{
	BOOL  RetVal = FALSE;
	const INT	  INQUIRY_INTERVALS = 1000;//Milliseconds
	DevDetail		DevDetailStorage;
	DevDetailPtr    DevPathPtr = &DevDetailStorage;

	//For the start, find all the devices and their paths
	RetVal = DevPathSearching(DevPathPtr);
	printf("Number of devices detected : %d \n",DevPathPtr->DeviceIndex + 1);//usb interface start from 0;
	
	//start the time counting loop and check the connection status
	INT		LoopCounter = 0/*[10] = {}*/;//loop timing counter....supporting 10devices.
	while (!(GetKeyState(0x51) & 0x8000))//press q to exit,0x51 stands for key "q"
	{
		//See if there are new devices connecting
		INT		CurrentDevCount;
		RetVal = DeviceConnectInquiry(DevPathPtr->DeviceIndex,CurrentDevCount);//Return TRUE if new device come
		if (RetVal == TRUE)
		{
			DevPathSearching(DevPathPtr);
			printf("%d new device plug in.\n",CurrentDevCount - DevPathPtr->DeviceIndex);
		}
		//Check if a device is disconnect
		for (int i = 0; i <= DevPathPtr->DeviceIndex; i++)
		{
			RetVal = DevicePathValidate(DevPathPtr, i);
			if (RetVal == FALSE)
			{
				//print the unique port ID
				DevicePathIDParsing(DevPathPtr,i);
			}
		}
		LoopCounter++;
		//1 minutes time reached.
		if (LoopCounter >= 60)//1 seconds per loop, 60 is set to 1 minute
		{
			LoopCounter = 0;
			printf("preset time reached, reseting timer\n");
		}
		//system sleep for particular intervals, 1seconds here.
		Sleep(INQUIRY_INTERVALS);
	}
	printf("program exit...... \n");
	int i = 0;
	std::cin >> i;
	return 0;
}