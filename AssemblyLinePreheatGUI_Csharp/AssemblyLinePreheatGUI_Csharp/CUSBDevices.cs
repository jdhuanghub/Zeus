using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AssemblyLinePreheatGUI_Csharp
{
    internal delegate void ProgressEventHandler(string Message);

    internal class CUSBDevices
    {
        internal event ProgressEventHandler ProgressEvent;
        internal event EventHandler StartProgress;
        internal event EventHandler EndProgress;
        public CUSBDevices()
        {

        }

        internal void Start(string Path)
        {
            if (StartProgress != null)
                StartProgress(this, null);

            string strPath = string.Empty;


            Guid OB_USB_DEV = new Guid("c3b5f0225a4219801909ea72095601b1");//sensor usb guid

            IntPtr hInfoSet = CWin32.SetupDiGetClassDevs(ref OB_USB_DEV, null, IntPtr.Zero, CWin32.DIGCF_PRESENT | CWin32.DIGCF_DEVICEINTERFACE);
            // this gets a list of all devices currently connected to the computer (InfoSet)
            try
            {
                CWin32.DeviceInterfaceData oInterface = new CWin32.DeviceInterfaceData();
                // build up a device interface data block
                oInterface.Size = Marshal.SizeOf(oInterface);
                // Now iterate through the InfoSet memory block assigned within Windows in the call to SetupDiGetClassDevs
                // to get device details for each device connected

                int nIndex = 0;
                while (CWin32.SetupDiEnumDeviceInterfaces(hInfoSet, 0, ref OB_USB_DEV, (uint)nIndex, ref oInterface))
                {
                    // this gets the device interface information for a device at index 'nIndex' in the memory block
                    // get the device path (see helper method 'GetDevicePath')
                    string strDevicePath = GetDevicePath(hInfoSet, ref oInterface);
                    

                    nIndex++;// if we get here, we didn't find our device. So move on to the next one.
                }
            }
            finally
            {
                // Before we go, we have to free up the InfoSet memory reserved by SetupDiGetClassDevs
                CWin32.SetupDiDestroyDeviceInfoList(hInfoSet);
            }
            //didn't find device
            if (EndProgress != null)
                EndProgress(this, null);

        }
        /// <summary>
        /// Helper method to return the device path given a DeviceInterfaceData structure and an InfoSet handle.
        /// Used in 'FindDevice' so check that method out to see how to get an InfoSet handle and a DeviceInterfaceData.
        /// </summary>
            private static string GetDevicePath (IntPtr hInfoSet, ref CWin32.DeviceInterfaceData oInterface)
            {
                uint nRequiredSize = 0;
                // Get the device interface details
                if ( !CWin32.SetupDiGetDeviceInterfaceDetail( hInfoSet, ref oInterface, IntPtr.Zero, 0, ref nRequiredSize, IntPtr.Zero ))
                {
                    CWin32.DeviceInterfaceDetailData oDetail = new CWin32.DeviceInterfaceDetailData();
                     oDetail.Size = 5;//sth that works.....
                    if (CWin32.SetupDiGetDeviceInterfaceDetail( hInfoSet, ref oInterface, ref oDetail, nRequiredSize, ref nRequiredSize, IntPtr.Zero ))
                    {
                        return oDetail.DevicePath;
                    }

                }
                return null;
            }

        
    }//close class
}//namespace
