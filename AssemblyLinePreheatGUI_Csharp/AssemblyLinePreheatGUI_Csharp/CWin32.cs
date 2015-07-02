using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace AssemblyLinePreheatGUI_Csharp
{
    public class CWin32
    {
        #region Consts

       // public readonly Guid GUID_OB_DEVICE_USB = new Guid(0xc3b5f022, 0x5a42, 0x1980, 0x19, 0x09, 0xea, 0x72, 0x09, 0x56, 0x01, 0xb1);

//         public const int CM_REGISTRY_HARDWARE = 0;
//         public const int ERROR_INSUFFICIENT_BUFFER = 122;
//         public const int ERROR_INVALID_DATA = 13;
//         public const int ERROR_INVALID_PARAMETER = 87;
//         public const int ERROR_INVALID_HANDLE = 6;
//         public const int ERROR_NO_MORE_ITEMS = 259;
//         public const int KEY_QUERY_VALUE = 1;
//         public const int RegDisposition_OpenExisting = 1;
//         public const int INVALID_HANDLE_VALUE = -1;
//         public const int MAXIMUM_USB_STRING_LENGTH = 255;
// 
//         public const uint FILE_ANY_ACCESS = 0;
//         public const uint FILE_SPECIAL_ACCESS = FILE_ANY_ACCESS;
         public const uint FILE_READ_ACCESS = (0x0001);    // file & pipe
         public const uint FILE_WRITE_ACCESS = (0x0002);    // file & pipe

        #endregion Consts

        #region Structures
        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum DeviceFlags : int
        {
            DigCFDefault = 1,
            DigCFPresent = 0x02,           // return only devices that are currently present
            DigCFAllClasses = 4,        // gets all classes, ignores the guid...
            DigCFProfile = 8,           // gets only classes that are part of the current hardware profile
            DigCDDeviceInterface = 0x10,  // Return devices that expose interfaces of the interface class that are specified by ClassGuid.
        }

        /// <summary>
        /// An overlapped structure used for overlapped IO operations. The structure is
        /// only used by the OS to keep state on pending operations. You don't need to fill anything in if you
        /// unless you want a Windows event to fire when the operation is complete.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct Overlapped
        {
            public uint Internal;
            public uint InternalHigh;
            public uint Offset;
            public uint OffsetHigh;
            public IntPtr Event;
        }
        /// <summary>
        /// Provides details about a single USB device
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct DeviceInterfaceData
        {
            public int Size;
            public Guid InterfaceClassGuid;
            public int Flags;
            public int Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct DevinfoData
        {
            public int Size;
            public Guid ClassGuid;
            public IntPtr DevInst;
            public int Reserved;
        }

        /// <summary>
        /// Access to the path for a device
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DeviceInterfaceDetailData
        {
            public int Size;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }
        /// <summary>
        /// Used when registering a window to receive messages about devices added or removed from the system.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public class DeviceBroadcastInterface
        {
            public int Size;
            public int DeviceType;
            public int Reserved;
            public Guid ClassGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Name;
        }
        /// <summary>
        /// user defined structure for control transfer
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PSUSBDRV_CONTROL_TRANSFER
        {
            public Byte cDirection;
            public Byte cRequestType;
            public Byte cRequest;
            public Int16 nValue;
            public Int16 nIndex;
            public int nTimeout;
        }

        #endregion

        #region Constants
        public const uint FILE_SHARE_READ = 0x00000001;    // file & pipe
        public const uint FILE_SHARE_WRITE = 0x00000002;    // file & pipe
        public const int ERROR_NO_MORE_ITEMS = 259;
        /// <summary>
        /// Device IO control code
        /// </summary>
        public const uint FILE_DEVICE_UNKNOWN = 0x00000022; 
        public const uint IOCTL_INDEX = 0x0000;
        public const uint METHOD_OUT_DIRECT = 2;
        public const uint FILE_ANY_ACCESS = 0;

        /// <summary>WParam for above : A device was inserted</summary>
        public const int DEVICE_ARRIVAL = 0x8000;
        /// <summary>WParam for above : A device was removed</summary>
        public const int DEVICE_REMOVECOMPLETE = 0x8004;
        /// <summary>Used in SetupDiClassDevs to get devices present in the system</summary>
        internal const int DIGCF_PRESENT = 0x02;
        /// <summary>Used in SetupDiClassDevs to get device interface details</summary>
        internal const int DIGCF_DEVICEINTERFACE = 0x10;
        /// <summary>Used when registering for device insert/remove messages : specifies the type of device</summary>
        internal const int DEVTYP_DEVICEINTERFACE = 0x05;
        /// <summary>Used when registering for device insert/remove messages : we're giving the API call a window handle</summary>
        internal const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;

        /// <summary>CreateFile : Open file for read</summary>
        internal const uint GENERIC_READ = 0x80000000;
        /// <summary>CreateFile : Open file for write</summary>
        internal const uint GENERIC_WRITE = 0x40000000;
        /// <summary>CreateFile : Open handle for overlapped operations</summary>
        internal const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        /// <summary>CreateFile : Resource to be "created" must exist</summary>
        internal const uint OPEN_EXISTING = 3;
        /// <summary>ReadFile/WriteFile : Overlapped operation is incomplete.</summary>
        internal const uint ERROR_IO_PENDING = 997;
        /// <summary>Infinite timeout</summary>
        internal const uint INFINITE = 0xFFFFFFFF;
        /// <summary>Simple representation of a null handle : a closed stream will get this handle. Note it is public for comparison by higher level classes.</summary>
        public static IntPtr NullHandle = IntPtr.Zero;
        /// <summary>Simple representation of the handle returned when CreateFile fails.</summary>
        internal static IntPtr InvalidHandleValue = new IntPtr(-1);
        #endregion

        #region P/Invoke
        /// <summary>
        /// Gets the GUID that Windows uses to represent HID class devices
        /// </summary>
        /// <param name="gHid">An out parameter to take the Guid</param>
        [DllImport("hid.dll", SetLastError = true)]
        internal static extern void HidD_GetHidGuid(out Guid gHid);
        /// <summary>
        /// Allocates an InfoSet memory block within Windows that contains details of devices.
        /// </summary>
        /// <param name="gClass">Class guid (e.g. HID guid)</param>
        /// <param name="strEnumerator">Not used</param>
        /// <param name="hParent">Not used</param>
        /// <param name="nFlags">Type of device details required (DIGCF_ constants)</param>
        /// <returns>A reference to the InfoSet</returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern IntPtr SetupDiGetClassDevs(ref Guid gClass, /*[MarshalAs(UnmanagedType.LPStr)] string*/int strEnumerator, IntPtr hParent, uint nFlags);
        /// <summary>
        /// Frees InfoSet allocated in call to above.
        /// </summary>
        /// <param name="lpInfoSet">Reference to InfoSet</param>
        /// <returns>true if successful</returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern int SetupDiDestroyDeviceInfoList(IntPtr lpInfoSet);
        /// <summary>
        /// Gets the DeviceInterfaceData for a device from an InfoSet.
        /// </summary>
        /// <param name="lpDeviceInfoSet">InfoSet to access</param>
        /// <param name="nDeviceInfoData">Not used</param>
        /// <param name="gClass">Device class guid</param>
        /// <param name="nIndex">Index into InfoSet for device</param>
        /// <param name="oInterfaceData">DeviceInterfaceData to fill with data</param>
        /// <returns>True if successful, false if not (e.g. when index is passed end of InfoSet)</returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiEnumDeviceInterfaces(IntPtr lpDeviceInfoSet, uint nDeviceInfoData, ref Guid gClass, uint nIndex, ref DeviceInterfaceData oInterfaceData);
        /// <summary>
        /// SetupDiGetDeviceInterfaceDetail - two of these, overloaded because they are used together in slightly different
        /// ways and the parameters have different meanings.
        /// Gets the interface detail from a DeviceInterfaceData. This is pretty much the device path.
        /// You call this twice, once to get the size of the struct you need to send (nDeviceInterfaceDetailDataSize=0)
        /// and once again when you've allocated the required space.
        /// </summary>
        /// <param name="lpDeviceInfoSet">InfoSet to access</param>
        /// <param name="oInterfaceData">DeviceInterfaceData to use</param>
        /// <param name="lpDeviceInterfaceDetailData">DeviceInterfaceDetailData to fill with data</param>
        /// <param name="nDeviceInterfaceDetailDataSize">The size of the above</param>
        /// <param name="nRequiredSize">The required size of the above when above is set as zero</param>
        /// <param name="lpDeviceInfoData">Not used</param>
        /// <returns></returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr lpDeviceInfoSet, ref DeviceInterfaceData oInterfaceData, IntPtr lpDeviceInterfaceDetailData, uint nDeviceInterfaceDetailDataSize, ref uint nRequiredSize, ref DevinfoData deviceInfoData);
        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr lpDeviceInfoSet, ref DeviceInterfaceData oInterfaceData, ref DeviceInterfaceDetailData oDetailData, uint nDeviceInterfaceDetailDataSize, ref uint nRequiredSize, ref DevinfoData deviceInfoData);
        /// <summary>
        /// Registers a window for device insert/remove messages
        /// </summary>
        /// <param name="hwnd">Handle to the window that will receive the messages</param>
        /// <param name="lpInterface">DeviceBroadcastInterrface structure</param>
        /// <param name="nFlags">set to DEVICE_NOTIFY_WINDOW_HANDLE</param>
        /// <returns>A handle used when unregistering</returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr RegisterDeviceNotification(IntPtr hwnd, DeviceBroadcastInterface oInterface, uint nFlags);
        /// <summary>
        /// Unregister from above.
        /// </summary>
        /// <param name="hHandle">Handle returned in call to RegisterDeviceNotification</param>
        /// <returns>True if success</returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool UnregisterDeviceNotification(IntPtr hHandle);
        /// <summary>
        /// Gets details from an open device. Reserves a block of memory which must be freed.
        /// </summary>
        /// <param name="hFile">Device file handle</param>
        /// <param name="lpData">Reference to the preparsed data block</param>
        /// <returns></returns>
        [DllImport("hid.dll", SetLastError = true)]
        internal static extern bool HidD_GetPreparsedData(IntPtr hFile, out IntPtr lpData);
        /// <summary>
        /// Frees the memory block reserved above.
        /// </summary>
        /// <param name="pData">Reference to preparsed data returned in call to GetPreparsedData</param>
        /// <returns></returns>
        [DllImport("hid.dll", SetLastError = true)]
        internal static extern bool HidD_FreePreparsedData(ref IntPtr pData);
        /// <summary>
        /// Creates/opens a file, serial port, USB device... etc
        /// </summary>
        /// <param name="strName">Path to object to open</param>
        /// <param name="nAccess">Access mode. e.g. Read, write</param>
        /// <param name="nShareMode">Sharing mode</param>
        /// <param name="lpSecurity">Security details (can be null)</param>
        /// <param name="nCreationFlags">Specifies if the file is created or opened</param>
        /// <param name="nAttributes">Any extra attributes? e.g. open overlapped</param>
        /// <param name="lpTemplate">Not used</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPStr)] string strName, uint nAccess, uint nShareMode, IntPtr lpSecurity, uint nCreationFlags, uint nAttributes, IntPtr lpTemplate);
        /// <summary>
        /// Closes a window handle. File handles, event handles, mutex handles... etc
        /// </summary>
        /// <param name="hFile">Handle to close</param>
        /// <returns>True if successful.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int CloseHandle(IntPtr hFile);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="dwIoControlCode"></param>
        /// <param name="InBuffer"></param>
        /// <param name="nInBufferSize"></param>
        /// <param name="OutBuffer"></param>
        /// <param name="nOutBufferSize"></param>
        /// <param name="pBytesReturned"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool DeviceIoControl(
           IntPtr hDevice,
           uint dwIoControlCode,
           Int16[] InBuffer,
           Byte nInBufferSize,
           Byte[] OutBuffer,
           Byte nOutBufferSize,
           ref int pBytesReturned,
           IntPtr lpOverlapped);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="dwIoControlCode"></param>
        /// <param name="InBuffer"></param>
        /// <param name="nInBufferSize"></param>
        /// <param name="OutBuffer"></param>
        /// <param name="nOutBufferSize"></param>
        /// <param name="pBytesReturned"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DeviceIoControl(
           IntPtr hDevice,
           uint dwIoControlCode,
           Int16[] InBuffer,
           Byte nInBufferSize,
           ref Byte[] OutBuffer,
           Byte nOutBufferSize,
           ref int pBytesReturned,
           IntPtr lpOverlapped);
        #endregion

        #region Public methods
        /// <summary>
        /// Registers a window to receive windows messages when a device is inserted/removed. Need to call this
        /// from a form when its handle has been created, not in the form constructor. Use form's OnHandleCreated override.
        /// </summary>
        /// <param name="hWnd">Handle to window that will receive messages</param>
        /// <param name="gClass">Class of devices to get messages for</param>
        /// <returns>A handle used when unregistering</returns>
        public static IntPtr RegisterForUsbEvents(IntPtr hWnd, Guid gClass)
        {
            DeviceBroadcastInterface oInterfaceIn = new DeviceBroadcastInterface();
            oInterfaceIn.Size = Marshal.SizeOf(oInterfaceIn);
            oInterfaceIn.ClassGuid = gClass;
            oInterfaceIn.DeviceType = DEVTYP_DEVICEINTERFACE;
            oInterfaceIn.Reserved = 0;
            return RegisterDeviceNotification(hWnd, oInterfaceIn, DEVICE_NOTIFY_WINDOW_HANDLE);
        }
        /// <summary>
        /// Unregisters notifications. Can be used in form dispose
        /// </summary>
        /// <param name="hHandle">Handle returned from RegisterForUSBEvents</param>
        /// <returns>True if successful</returns>
        public static bool UnregisterForUsbEvents(IntPtr hHandle)
        {
            return UnregisterDeviceNotification(hHandle);
        }
        /// <summary>
        /// Helper to get the HID guid.
        /// </summary>
        public static Guid HIDGuid
        {
            get
            {
                Guid gHid;
                HidD_GetHidGuid(out gHid);
                return gHid;
            }
        }
        #endregion

        #region FunctionsList
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DeviceType"></param>
        /// <param name="Function"></param>
        /// <param name="Method"></param>
        /// <param name="Access"></param>
        /// <returns></returns>
        public static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            return ((DeviceType << 16) | (Access << 14) | (Function << 2) | Method);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public static uint IOCTL_PSDRV_CONTROL_TRANSFER = CTL_CODE(FILE_DEVICE_UNKNOWN, IOCTL_INDEX + 4, METHOD_OUT_DIRECT, FILE_ANY_ACCESS);
        public static uint IOCTL_PSDRV_CONTROL_TRANSFER()
        {
            return CTL_CODE(FILE_DEVICE_UNKNOWN, IOCTL_INDEX + 4, METHOD_OUT_DIRECT, FILE_ANY_ACCESS);
        }
        
        #endregion

    }//class close
}
