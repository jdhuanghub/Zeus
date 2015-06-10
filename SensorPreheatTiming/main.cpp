//********************************************************/
//Head files
//********************************************************/
#include "SensorPreheat.h"

//********************************************************/
//Function declaration
//********************************************************/
BOOL DevPathSearching(DevDetailPtr pDeviceDetailData);
BOOL DeviceConnectInquiry(DevDetailPtr pDeviceDetailData, INT DevIndex);

//********************************************************/
//     MAIN
//********************************************************/

int main()
{
	BOOL  RetVal = FALSE;
	const INT	  INQUIRY_INTERVALS = 1000;//Milliseconds
	DevDetail		DevDetailStorage;
	DevDetailPtr    DevPathPtr = &DevDetailStorage;

	RetVal = DevPathSearching(DevPathPtr);
	printf("Number of devices detected : %d \n",DevPathPtr->DeviceIndex + 1);//usb interface start from 0;
	
	//press q to exit
	//0x51 stands for key "q"
	INT	SearchLoopIndex = 0;
	while (!(GetKeyState(0x51) & 0x8000))
	{
		if (SearchLoopIndex > DevPathPtr->DeviceIndex)
		{
			SearchLoopIndex = 0;
		}
		RetVal = DeviceConnectInquiry(DevPathPtr,SearchLoopIndex);
		//True if all devices inquiries success
		if (RetVal == FALSE)
		{
			printf("%d connection lost"/*method to count lost connection*/);
		}
		SearchLoopIndex++;
		Sleep(INQUIRY_INTERVALS);
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