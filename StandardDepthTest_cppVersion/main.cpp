//*******************************************************************
// ImagetTest
// Acquire the Depth image from OpenNI
//Then process the depth image while counting the
//Pixels with zero values or pixels within default range;
//Results are display in the console.
//********************************************************************
#include <iostream>
#include <OpenNI.h>
#include "opencv2/opencv.hpp"
#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

using namespace openni;
using namespace cv;



int main(int argc, char* argv[])
{
	//-------Display Parameters-------
 	const int xRes = 640;
 	const int yRes = 480;
	const int FPS = 30;
	int designedDistance = 600;// millimeters
	int errorRange = 5;//millimeters
	int lowerValue = designedDistance - errorRange;
	int UpperValue = designedDistance + errorRange;
	bool programRunning = true;

	//-------Initialize OpenNI-------
	Status rc = OpenNI::initialize();
	if (rc != STATUS_OK)
	{
		printf("Initialize failed\n%s\n", OpenNI::getExtendedError());
		return -1;
	}

	//-------open the device-------
	Device device;
	rc = device.open(ANY_DEVICE);
	if (rc != STATUS_OK)
	{
		printf("Device Opening failed\n%s\n", OpenNI::getExtendedError());
		return -1;
	}
	//-------create the depth steam-------
	VideoStream depth;
	rc = depth.create(device, SENSOR_DEPTH);
	if (rc != STATUS_OK)
	{
		printf("Depth creation failed\n%s\n", OpenNI::getExtendedError());
		return -1;
	}
	//-------set the depth image mode-------
	VideoMode mModeDepth;
	//Resolution
	mModeDepth.setResolution(xRes,yRes);
	//FPS
	mModeDepth.setFps(FPS);
	//Pixel Format
	mModeDepth.setPixelFormat(PIXEL_FORMAT_DEPTH_1_MM);
	depth.setVideoMode(mModeDepth);
	
	//-------open the depth stream-------
	rc = depth.start();
	if (rc != STATUS_OK)
	{
		printf("Depth stream generating failed\n%s\n", OpenNI::getExtendedError());
		return -1;
	}
	//-------create open cv window-------
	string DepthImageWindow = "Depth Image";
	string ImageStatistics = "Image Statistics";
	namedWindow(DepthImageWindow, CV_WINDOW_AUTOSIZE);
	namedWindow(ImageStatistics, CV_WINDOW_AUTOSIZE);
	//acquire max depth value
	int iMaxDepth = depth.getMaxPixelValue();

	//-------read data in a loop-------
	VideoFrameRef frameDepth;

 	while(programRunning)
 	{
		//read data frame
		depth.readFrame(&frameDepth);
		const int depthWidth = frameDepth.getWidth();
		const int depthHeight = frameDepth.getHeight();
		//convert depth frame to open cv format
		Mat mImageDepth( depthHeight, depthWidth, CV_16UC1, (void*)frameDepth.getData());
		//convert depth format from CV_16UC1 to CV_8U
		Mat mScaledDepth;
		mImageDepth.convertTo(mScaledDepth, CV_8UC1, 255.0 / iMaxDepth);
		//Create another open cv image for test
		Mat ProcessedImage(depthHeight, depthWidth, CV_8UC3);
		//Create an array and copy the depth data to the array
		DepthPixel* pDepth = (DepthPixel*)frameDepth.getData();
		uint16_t depthArray[xRes * yRes];
		memcpy(depthArray, pDepth, sizeof(depthArray));
		//define some depth parameters
		int middleIndex = (depthHeight+1)*depthWidth/2;
		int count0(0), count1(0),count2(0);
		int ArrayPixelIndex = 0;
		for (int i = 0; i< depthHeight; ++i)
		{	
			for (int j = depthWidth - 1; j >= 0; --j)
			{
				Vec3b* ProcImage_pixel = ProcessedImage.ptr<Vec3b>(i);
				int arrayDepthValue = depthArray[ArrayPixelIndex];
				if (arrayDepthValue == 0)
				{	
					//pixels range value is Zero
					ProcImage_pixel[j][0] = 250;//Blue
					ProcImage_pixel[j][1] = 0;//Green
					ProcImage_pixel[j][2] = 0;//Red
					count0++;
				}
				else if (arrayDepthValue < lowerValue || arrayDepthValue > UpperValue)
				{
					//pixels range in the (1,590)U(610,Max range)
					ProcImage_pixel[j][0] = 0;//Blue
					ProcImage_pixel[j][1] = 0;//Green
					ProcImage_pixel[j][2] = 250;//Red
					count1++;
				}
				else
				{
					//pixels range in the (590,610)
					ProcImage_pixel[j][0] = 0;//Blue
					ProcImage_pixel[j][1] = 250;//Green
					ProcImage_pixel[j][2] = 0;//Red
					count2++;
				}
				++ArrayPixelIndex;
			}
		}
		//Statistics of different pixels
		std::cout<<"Center position distance: "<<pDepth[middleIndex]<<std::endl
			     <<"numbers of pixels with value of 0: "<< count0<<"  ["<<(double)(count0/3072)<<"%]"<<std::endl
			     <<"numbers of pixels out of default range: "<<count1<<"  ["<<(double)(count1/3072)<<"%]"<<std::endl
				 <<"numbers of pixels within the range: "<<count2<<"  ["<<(double)(count2/3072)<<"%]"<<std::endl;

		//Display the depth image
		imshow(DepthImageWindow, mScaledDepth);
		//Display the image
		imshow(ImageStatistics, ProcessedImage);
		//Press ESC to exit
		if (waitKey(10) == 27)
		{
			break;
		}

	}

	//Close data and device
	depth.stop();
	depth.destroy();

	//Close device
	device.close();
	OpenNI::shutdown();

	return 0;
}