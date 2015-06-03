#ifndef _FIRMWAREINFO_H
#define _FIRMWAREINFO_H

//---------------------------------------------------------------------------
// Includes
//---------------------------------------------------------------------------
#include <Windows.h>

//---------------------------------------------------------------------------
// Types
//---------------------------------------------------------------------------
class FirmwareInfo
{
public:
	unsigned short nHostMagic;
	unsigned short nFWMagic;
	unsigned short nProtocolHeaderSize;
	unsigned short nProtocolMaxPacketSize;

	unsigned short nOpcodeGetVersion;
	unsigned short nOpcodeKeepAlive;
	unsigned short nOpcodeGetParam;
	unsigned short nOpcodeSetParam;
	unsigned short nOpcodeGetFixedParams;
	unsigned short nOpcodeGetMode;
	unsigned short nOpcodeSetMode;
	unsigned short nOpcodeAlgorithmParams;
	unsigned short nOpcodeReset;
	unsigned short nOpcodeSetCmosBlanking;
	unsigned short nOpcodeGetCmosBlanking;
	unsigned short nOpcodeGetCmosPresets;
	unsigned short nOpcodeGetSerialNumber;
	unsigned short nOpcodeGetFastConvergenceTEC;
	unsigned short nOpcodeGetCMOSReg;
	unsigned short nOpcodeSetCMOSReg;
	unsigned short nOpcodeWriteI2C;
	unsigned short nOpcodeReadI2C;
	unsigned short nOpcodeReadAHB;
	unsigned short nOpcodeWriteAHB;
	unsigned short nOpcodeGetPlatformString;
	unsigned short nOpcodeGetUsbCore;
	unsigned short nOpcodeSetLedState;
	unsigned short nOpcodeEnableEmitter;

	unsigned short nOpcodeGetLog;
	unsigned short nOpcodeTakeSnapshot;
	unsigned short nOpcodeInitFileUpload;
	unsigned short nOpcodeWriteFileUpload;
	unsigned short nOpcodeFinishFileUpload;
	unsigned short nOpcodeDownloadFile;
	unsigned short nOpcodeDeleteFile;
	unsigned short nOpcodeGetFlashMap;
	unsigned short nOpcodeGetFileList;
	unsigned short nOpcodeSetFileAttribute;
	unsigned short nOpcodeExecuteFile;
	unsigned short nOpcodeReadFlash;
	unsigned short nOpcodeBIST;
	unsigned short nOpcodeSetGMCParams;
	unsigned short nOpcodeGetCPUStats;
	unsigned short nOpcodeCalibrateTec;
	unsigned short nOpcodeGetTecData;
	unsigned short nOpcodeCalibrateEmitter;
	unsigned short nOpcodeGetEmitterData;
	unsigned short nOpcodeCalibrateProjectorFault;

	unsigned short nLogStringType;
	unsigned short nLogOverflowType;

	BOOL bMirrorSupported;

	unsigned short nUSBDelayReceive;
	unsigned short nUSBDelayExecutePreSend;
	unsigned short nUSBDelayExecutePostSend;
	unsigned short nUSBDelaySoftReset;
	unsigned short nUSBDelaySetParamFlicker;
	unsigned short nUSBDelaySetParamStream0Mode;
	unsigned short nUSBDelaySetParamStream1Mode;
	unsigned short nUSBDelaySetParamStream2Mode;

	unsigned char nISOAlternativeInterface;
	unsigned char nBulkAlternativeInterface;
	unsigned char nISOLowDepthAlternativeInterface;

	BOOL bGetImageCmosTypeSupported;
	BOOL bImageSupported;
	BOOL bIncreasedFpsCropSupported;
	BOOL bHasFilesystemLock;
// Following arrays to be complete...
// 	 depthModes;
// 	 _imageBulkModes;
// 	 _imageIsoModes;
// 	imageModes;
//  irModes;

};
#endif