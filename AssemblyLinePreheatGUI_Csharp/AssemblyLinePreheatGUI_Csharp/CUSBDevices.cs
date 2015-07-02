using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AssemblyLinePreheatGUI_Csharp
{
    public class DevFunction : CWin32
  {
      //Device data structure
      [StructLayout(LayoutKind.Sequential, Pack = 1)]
      public struct DevDetail
      {
          public string[] DevPathArray;
          public uint DeviceIndex;
      }
      // data strut for usb command header
      [StructLayout(LayoutKind.Sequential, Pack = 1)]
      public struct HostProtocolHeader
      {
          public UInt16 nMagic;
          public UInt16 nSize;
          public UInt16 nOpcode;
          public UInt16 nId;
      }

      public static bool DevPathSearching(ref DevDetail customDetailData, Guid GuidSource)
      {
          //bool searchResult = false;
          IntPtr    hDevInfoSet;

          hDevInfoSet = SetupDiGetClassDevs(ref GuidSource, 0, IntPtr.Zero, (int)(DeviceFlags.DigCFPresent | DeviceFlags.DigCDDeviceInterface));
          if (hDevInfoSet == InvalidHandleValue)
          {
              //do sth to show info message.
              //MessageBox.Show()
              customDetailData.DevPathArray[0] = string.Empty;
              return false;
          }
         
          //search the devices and get the path
          DevinfoData   infoData = new DevinfoData();
          DeviceInterfaceData interfaceData = new DeviceInterfaceData();
          DeviceInterfaceDetailData detailData = new DeviceInterfaceDetailData();

          uint RequiredSize = 0;
          bool devicesearch = true;
          bool searchingResult = false;
          uint SearchIndex = 0;

          while (devicesearch)
          {
              
              interfaceData.Size = Marshal.SizeOf(typeof(DeviceInterfaceData));
              searchingResult = SetupDiEnumDeviceInterfaces(hDevInfoSet, 0, ref GuidSource,
                                                            SearchIndex, ref interfaceData);

              infoData.Size = Marshal.SizeOf(typeof(DevinfoData));
              if (searchingResult == true)
              {
                  //Get the size
                  searchingResult = SetupDiGetDeviceInterfaceDetail(hDevInfoSet, ref interfaceData, IntPtr.Zero, 0, ref RequiredSize, ref infoData);

                  //detailData.Size = Marshal.SizeOf(typeof(DeviceInterfaceDetailData));
                  detailData.Size = 5;
                  //get the path
                  searchingResult = SetupDiGetDeviceInterfaceDetail(hDevInfoSet, ref interfaceData, ref detailData, RequiredSize, ref RequiredSize, ref infoData);

                  if (!searchingResult)
                  {
                      //System.Windows.Forms.MessageBox.Show(Marshal.GetLastWin32Error().ToString());
                      //throw (new ApplicationException("[CCore::GetDeviceInfo] Can not get SetupDiGetDeviceInterfaceDetail"));
                  }

                  //save the path
                  customDetailData.DevPathArray[SearchIndex] = detailData.DevicePath;
                  SearchIndex++;

              }
              else if (Marshal.GetLastWin32Error() == ERROR_NO_MORE_ITEMS)
              {
                  devicesearch = false;
                  SearchIndex = SearchIndex - 1;
                  break;
              }
            }
          customDetailData.DeviceIndex = SearchIndex;
          SetupDiDestroyDeviceInfoList(hDevInfoSet);
          return searchingResult;
      }

      public static bool DeviceConnectInquiry(ref uint PreDevIndex, ref uint CurrentDevCount, Guid GuidSource)
      {
          IntPtr hDevInfo;
          bool DevConnectStatus;
          bool NewDevice = false;

          
          //Enumerate all devices exposing the interface
          hDevInfo = SetupDiGetClassDevs(ref GuidSource, 0, IntPtr.Zero, (int)(DeviceFlags.DigCFPresent | DeviceFlags.DigCDDeviceInterface));
          if (hDevInfo == InvalidHandleValue)
          {
              //do sth to show info message.
              System.Windows.Forms.MessageBox.Show(Marshal.GetLastWin32Error().ToString(), "Error!");
              return false;
          }

          //Get the interface of particular index in the index lists
          DeviceInterfaceData interfaceData = new DeviceInterfaceData();
          interfaceData.Size = Marshal.SizeOf(typeof(DeviceInterfaceData));
          while (true)
          {
              DevConnectStatus = SetupDiEnumDeviceInterfaces(hDevInfo, 0, ref GuidSource,
                                                            CurrentDevCount, ref interfaceData);
              if ((DevConnectStatus == false) & (Marshal.GetLastWin32Error() == ERROR_NO_MORE_ITEMS))
              {
                  break;
              }
              CurrentDevCount++;
          }
          uint PreviousDevCount = CurrentDevCount;
          if (PreviousDevCount < CurrentDevCount)
          {
              NewDevice = true;
          }

          //Now clear the info list
          SetupDiDestroyDeviceInfoList(hDevInfo);
          return NewDevice;
      }

      public static bool DevicePathValidate(ref DevDetail customDetailData, uint DevIndex)
      {
          bool PathValidation = true;
          IntPtr devHandle;

          devHandle = CreateFile(customDetailData.DevPathArray[DevIndex],
                                 GENERIC_READ | GENERIC_WRITE,
                                 FILE_SHARE_READ | FILE_SHARE_WRITE,
                                 IntPtr.Zero, OPEN_EXISTING,
                                 FILE_FLAG_OVERLAPPED, IntPtr.Zero);
          if (devHandle == InvalidHandleValue)
          {
              //System.Windows.Forms.MessageBox.Show("Port device connection lost", "Error!");
              PathValidation = false;
          }
          CloseHandle(devHandle);
          return PathValidation;
      }

      public static bool EnableLaser(ref DevDetail customDetailData, uint DevIndex,int laserStatus)
      {
          bool enableStatus = true;

          IntPtr devHandle;
          devHandle = CreateFile(customDetailData.DevPathArray[DevIndex],
                       GENERIC_READ | GENERIC_WRITE,
                       FILE_SHARE_READ | FILE_SHARE_WRITE,
                       IntPtr.Zero, OPEN_EXISTING,
                       FILE_FLAG_OVERLAPPED, IntPtr.Zero);
          if (devHandle == InvalidHandleValue)
          {
              enableStatus = false;
          }

          #region SendControlcmd

          int nBytesReturn = new int();
          Byte outBufferSize = 10;
          Byte inBufferSize = 11;
          Byte[] msgOutBuffer = new Byte[512];//Max packet size

          InitialProtocolHeader(ref msgOutBuffer, 512, laserStatus);

          IntPtr transBuffer;
          PSUSBDRV_CONTROL_TRANSFER ControlTransfer = new PSUSBDRV_CONTROL_TRANSFER();
          ControlTransfer.cDirection = 0x00;
          ControlTransfer.cRequestType = 0x02;
          ControlTransfer.cRequest = 0x00;
          ControlTransfer.nValue = 0x0000;
          ControlTransfer.nIndex = 0x0000;
          ControlTransfer.nTimeout = 0x000003e8;
          int allocMsgSize = Marshal.SizeOf(ControlTransfer);
          transBuffer = Marshal.AllocHGlobal(allocMsgSize);
          Int16[] msgInBuffer = new Int16[allocMsgSize];
          Marshal.StructureToPtr(ControlTransfer, transBuffer, true);
          Marshal.Copy(transBuffer, msgInBuffer, 0, allocMsgSize);
          try
          {
              enableStatus = DeviceIoControl(devHandle, IOCTL_PSDRV_CONTROL_TRANSFER(),
                              msgInBuffer, inBufferSize,
                              msgOutBuffer, outBufferSize,
                              ref nBytesReturn, IntPtr.Zero);
          }
          catch (System.Exception ex)
          {
          }
          Marshal.FreeHGlobal(transBuffer);

          #endregion //SendControlcmd

          #region ReceivedControlcmd

          int actualBytesReturn = new int();
          Byte outBufferSize_R = 10;
          Byte inBufferSize_R = 11;
          PSUSBDRV_CONTROL_TRANSFER ControlTransfer_R = new PSUSBDRV_CONTROL_TRANSFER();
          ControlTransfer_R.cDirection = 0x01;
          ControlTransfer_R.cRequestType = 0x02;
          ControlTransfer_R.cRequest = 0x00;
          ControlTransfer_R.nValue = 0x0000;
          ControlTransfer_R.nIndex = 0x0000;
          ControlTransfer_R.nTimeout = 0x000001f4;
          int allocMsgSize_R = Marshal.SizeOf(ControlTransfer_R);
          IntPtr transBuffer_R = Marshal.AllocHGlobal(allocMsgSize_R);
          Marshal.StructureToPtr(ControlTransfer_R, transBuffer_R, true);
          Int16[] msgInBuffer_R = new Int16[allocMsgSize_R];
          Marshal.Copy(transBuffer_R, msgInBuffer_R, 0, allocMsgSize_R);
          try
          {
              enableStatus = DeviceIoControl(devHandle, IOCTL_PSDRV_CONTROL_TRANSFER(),
                            msgInBuffer_R, inBufferSize_R,
                            ref msgOutBuffer, outBufferSize_R,
                            ref actualBytesReturn, IntPtr.Zero);
          }
          catch (System.Exception ex)
          {
          }
          Marshal.FreeHGlobal(transBuffer_R);

          #endregion//ReceivedControlcmd

          CloseHandle(devHandle);
          return enableStatus;
      }

      public static void InitialProtocolHeader(ref Byte[] msgBuffer, int msgSize,int bitValue)
      {
          UInt16 devNid = 0;
          IntPtr pBuffer;
          HostProtocolHeader InitHeadStruct = new HostProtocolHeader();
          InitHeadStruct.nMagic = 0x4d47;//magic num
          InitHeadStruct.nSize = 0x0001;//size origin/2
          InitHeadStruct.nOpcode = 0x0055;//in decimal is 85, custom opcode for orbbec device
          InitHeadStruct.nId = devNid;
          devNid++;

          pBuffer = Marshal.AllocHGlobal(msgSize);
          Marshal.StructureToPtr(InitHeadStruct, pBuffer, true);
          Marshal.Copy(pBuffer, msgBuffer, 0, msgSize);
          msgBuffer[8] = (Byte)bitValue;//Key value for controlling emitter
          msgBuffer[9] = (Byte)0x00;
          Marshal.FreeHGlobal(pBuffer);
      }

      public static bool DevicePathIDParsing(ref DevDetail customDetailData, uint DevIndex, ref string IDstring)
      {
          if (string.IsNullOrEmpty(customDetailData.DevPathArray[0]))
          {
              IDstring = null;
              return false;
          }
          //Set the path string pass in function
          string InString = customDetailData.DevPathArray[DevIndex];
          //get rid of first "&"
          int startIndex = InString.IndexOf("&") + "&".Length;
          string InStringSub = InString.Substring(startIndex, InString.Length - startIndex);
          //now get the id string
          int first = InStringSub.IndexOf("&") + "&".Length;
          int last = InStringSub.LastIndexOf("&");

          IDstring = InStringSub.Substring(first, last - first - 2);

          return true;
      }

      public static void progressBarStatus(ProgressBarEx.ProgressBarEx probressBarObject, System.Windows.Forms.Label labelObject, int labelText, int percentComplete)
      {
          probressBarObject.Value = percentComplete;
          if (percentComplete != 0)
          {
              labelObject.Text = "端口 " + labelText + ": 监控中";
              if (percentComplete != 100)
              {
                  probressBarObject.ProgressColor = System.Drawing.Color.Red;
              }
              if (percentComplete == 100)
              {
                  probressBarObject.ProgressColor = System.Drawing.Color.Lime;
              }
          }
          else { labelObject.Text = "端口 " + labelText + ": "; }
      }

      public static void progressSetValue(string IDarray,ref int workPercentage, ref int progressBarCountNum, int secondsToExecute)
      {
          workPercentage = (int)((float)progressBarCountNum / (float)secondsToExecute * 100);
          if (!string.IsNullOrEmpty(IDarray))
          {
              if (workPercentage == 100)
              {
                  workPercentage = 100;
              }
              else
              {
                  progressBarCountNum++;
              }
          }
          else
          {
              progressBarCountNum = 0;
          }
      }

      public static void ArrayValueSet(char[] arr, char value)
      {
          for (int i = 0; i < arr.Length; i++)
          {
              arr[i] = value;
          }
      }

  }//Class DevFunction
}//namespace
