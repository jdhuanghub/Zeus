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

          return PathValidation;
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
          else { labelObject.Text = "端口 " + labelText + ": ";}
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

  }//Class DevFunction
}//namespace
