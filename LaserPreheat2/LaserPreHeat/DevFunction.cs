using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace LaserPreHeat
{
    class DevFunction : win32ApiWrap
    {

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HostProtocolHeader
        {
            public UInt16 nMagic;
            public UInt16 nSize;
            public UInt16 nOpcode;
            public UInt16 nId;
        }

        public static void InitialProtocolHeader(ref Byte[] msgBuffer, int msgSize, int bitValue)
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

        public static bool EnableLaser(string devPathUri, int laserStatus)
        {
            bool enableStatus = true;

            IntPtr devHandle;
            devHandle = CreateFile(devPathUri,
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
    }
}
