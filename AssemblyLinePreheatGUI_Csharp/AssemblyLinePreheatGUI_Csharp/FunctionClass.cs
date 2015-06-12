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
    public enum DiGetClassFlags : uint
    {
        DIGCF_DEFAULT = 0x00000001,    // only valid with DIGCF_DEVICEINTERFACE
        DIGCF_PRESENT = 0x00000002,
        DIGCF_ALLCLASSES = 0x00000004,
        DIGCF_PROFILE = 0x00000008,
        DIGCF_DEVICEINTERFACE = 0x00000010,
    }

    public class FunctionClass
    {
        //
        //Structure Imports
        //
        [StructLayout(LayoutKind.Sequential)]
        struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid classGuid;
            public uint devInst;
            public IntPtr reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SP_DEVICE_INTERFACE_DATA
        {
            public Int32 cbSize;
            public Guid interfaceClassGuid;
            public Int32 flags;
            private UIntPtr reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        //
        //Import setup API......
        //
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid,
                                                 [MarshalAs(UnmanagedType.LPTStr)] string Enumerator,
                                                 IntPtr hwndParent,
                                                 uint Flags);
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
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
        //
        //Define the GUID
        //
        Guid GUID_CLASS_OBDRV_USB = new Guid(0xc3b5f022, 0x5a42, 0x1980, 0x19, 0x09, 0xea, 0x72, 0x09, 0x56, 0x01, 0xb1);

        //
        //Define the functions
        //
        
        public bool DevPathSearching()
        {
            bool searchingResult = false;
            IntPtr devInfo;

            DiGetClassFlags DevFlags = DiGetClassFlags.DIGCF_PRESENT | DiGetClassFlags.DIGCF_DEVICEINTERFACE;
            devInfo = SetupDiGetClassDevs(ref GUID_CLASS_OBDRV_USB, 0, IntPtr.Zero, DevFlags);


            return searchingResult;
        }
    }
}
