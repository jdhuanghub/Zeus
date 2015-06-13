using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AssemblyLinePreheatGUI_Csharp
{
    //
    //Flags
    //
    // Flags controlling what is included in the device information set built by SetupDiGetClassDevs
    [Flags]
    public enum DiGetClassFlags : int
    {
        DIGCF_DEFAULT = 0x00000001,    // only valid with DIGCF_DEVICEINTERFACE
        DIGCF_PRESENT = 0x00000002,
        DIGCF_ALLCLASSES = 0x00000004,
        DIGCF_PROFILE = 0x00000008,
        DIGCF_DEVICEINTERFACE = 0x00000010,
    }
    //
    //Structure Imports
    //
    [StructLayout(LayoutKind.Sequential)]
    public class SP_DEVINFO_DATA
    {
        public uint cbSize;
        public Guid classGuid;
        public uint devInst;
        public IntPtr reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class SP_DEVICE_INTERFACE_DATA
    {
        public Int32 cbSize;
        public Guid interfaceClassGuid;
        public Int32 flags;
        private UIntPtr reserved;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class SP_DEVICE_INTERFACE_DETAIL_DATA
    {
        public int cbSize;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string DevicePath;
    }

    public class Winapi
    {

        //
        //Import setup API......
        //
        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid,
                                                 int Enumerator,
                                                 IntPtr hwndParent,
                                                 int Flags);
        [DllImport(@"setupapi.dll", CharSet=CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo,
                                                                 ref SP_DEVINFO_DATA devInfo,
                                                                 ref Guid interfaceClassGuid,
                                                                 UInt32 memberIndex,
                                                                 ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo,
                                                                     ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
                                                                     ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
                                                                     UInt32 deviceInterfaceDetailDataSize,
                                                                     out UInt32 requiredSize,
                                                                     ref SP_DEVINFO_DATA deviceInfoData);
        [DllImport(@"setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
    }

    public class Inquiry
    {
        //
        //Define the GUID
        //
        public Guid GUID_CLASS_OBDRV_USB = new Guid("c3b5f0225a4219801909ea72095601b1");

        //
        //Define the functions
        //
        public bool DevPathSearching()
        {
            bool searchingResult = false;

            IntPtr devInfo = Winapi.SetupDiGetClassDevs(ref GUID_CLASS_OBDRV_USB, 0, IntPtr.Zero, (int)(DiGetClassFlags.DIGCF_DEVICEINTERFACE | DiGetClassFlags.DIGCF_PRESENT));


            return searchingResult;
        }
    }
}
