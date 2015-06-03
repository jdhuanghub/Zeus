//
//Includes
//

#include "HostProtocol.h"
#include "DeviceSensor.h"
#include "USBWin32.h"
#include "DevUSB.h"
#include <stdio.h>
#include "ConsoleHeadfile.h"
//
//Defines and constants
//
const int MAX_PACKET_SIZE = 512;
//
//Declarations
//
BOOL USBSendControl(UsbDevHandle* pDevHandle, USBControlType nType, UCHAR nRequest, USHORT nValue, USHORT nIndex, UCHAR *pBuffer, UINT nBufferSize, UINT nTimeOut);

BOOL USBReceiveControl(UsbDevHandle* pDevHandle, USBControlType nType, UCHAR nRequest, USHORT nValue, USHORT nIndex, UCHAR *pBuffer, UINT nBufferSize, UINT *pnBytesReceived, UINT nTimeOut);


//
//Functions
//
BOOL HostProtocolUSBSend(const DevicePrivateData* pDevicePrivateData, unsigned char* pBuffer, unsigned short nSize, unsigned int nTimeout)
{
	BOOL nRetVal = TRUE;

	nRetVal = USBSendControl(pDevicePrivateData->SensorHandle.USBDevice, XN_USB_CONTROL_TYPE_VENDOR, 0, 0, 0, pBuffer, nSize, nTimeout);

	return nRetVal;
}

BOOL HostProtocolUSBReceive(const DevicePrivateData* pDevicePrivateData, unsigned char* pBuffer, unsigned short nSize,unsigned int &nRead, unsigned int nTimeOut)
{
	BOOL nRetVal;

	nRetVal = USBReceiveControl(pDevicePrivateData->SensorHandle.USBDevice, 
								XN_USB_CONTROL_TYPE_VENDOR, 
								0,
								0,
								0,
								pBuffer,
								nSize,
								&nRead,
								nTimeOut);

	return nRetVal;
}

BOOL ValidateReply(const DevicePrivateData* pDevicePrivateData, unsigned char* pBuffer, unsigned int nBufferSize, unsigned int nExpectedOpcode, unsigned short nRequestId, unsigned short& nDataSize, unsigned char** pDataBuf)
{
	unsigned short nHeaderOffset = 0;
	HostProtocolHeader* pHeader = (HostProtocolHeader*)pBuffer;

	pHeader->nMagic = (pHeader->nMagic);//16978

	pHeader->nId = (pHeader->nId);//54
	pHeader->nOpcode = (pHeader->nOpcode);//30
	pHeader->nSize = (pHeader->nSize);//12

	HostProtocolReplyHeader* pReply = (HostProtocolReplyHeader*)(pBuffer+nHeaderOffset+pDevicePrivateData->FWInfo.nProtocolHeaderSize);
	pReply->nErrorCode = pReply->nErrorCode;
	//......
	//......
	// Check reply length is reasonable for opcode

	nDataSize = pHeader->nSize - sizeof(HostProtocolReplyHeader)/sizeof(unsigned short);

	if (pDataBuf)
	{
		*pDataBuf = pBuffer + nHeaderOffset+pDevicePrivateData->FWInfo.nProtocolHeaderSize+sizeof(HostProtocolReplyHeader);
	}

	return TRUE;
}

BOOL HostProtocolReceiveReply(const DevicePrivateData* pDevicePrivateData, unsigned char *pBuffer, unsigned int nTimeOut, unsigned short nOpcode, unsigned short nRequestID, unsigned int* pnReadBytes, unsigned short *pnDataSize, unsigned char **ppRelevantBuffer)
{
	BOOL rc;

	rc = HostProtocolUSBReceive(pDevicePrivateData, pBuffer, pDevicePrivateData->FWInfo.nProtocolMaxPacketSize, *pnReadBytes, nTimeOut);

	//Validate the reply
	rc = ValidateReply(pDevicePrivateData, pBuffer, *pnReadBytes, nOpcode, nRequestID, *pnDataSize, ppRelevantBuffer);

	return rc;
}

BOOL HostProtocolExecute(const DevicePrivateData* pDevicePrivateData, unsigned char* pBuffer, unsigned short nSize, unsigned short nOpcode, unsigned char** ppRelevantBuffer, unsigned short& nDataSize)
{
	BOOL rc;
	unsigned int nRead = 0;
	unsigned int nTimeOut = 5000;
	//Store request
	unsigned char request[MAX_PACKET_SIZE];
	memcpy(request, pBuffer, nSize);
	//Send
	rc = HostProtocolUSBSend(pDevicePrivateData, request, nSize, nTimeOut);
	if (rc != TRUE)
	{
		printf("Sending protocol failed");
		return rc;
	}

	//Sleep before reading the reply
	Sleep(500);

	//Receive....
	unsigned short nRequestId;
	nRequestId = ((HostProtocolHeader*)(pBuffer))->nId;
	rc = HostProtocolReceiveReply(pDevicePrivateData, pBuffer, nTimeOut, nOpcode, nRequestId, &nRead, &nDataSize, ppRelevantBuffer);

	//Get rest of Data
	int nCur = nRead;//read so far

	nRead -= (pDevicePrivateData->FWInfo.nProtocolHeaderSize + sizeof(HostProtocolReplyHeader));//data read so far
	
// 	while(nRead < nDataSize*2U)
// 	{
// 		unsigned int dummy = 0;
// 		rc = HostProtocolUSBReceive(pDevicePrivateData, pBuffer+nCur, pDevicePrivateData->FWInfo.nProtocolMaxPacketSize, dummy, nTimeOut);
// 		if (rc != TRUE)
// 		{
// 			return rc;
// 		}
// 
// 		nCur += dummy;
// 		nRead += dummy;
// 	}

	return rc;
}

BOOL HostProtocolInitHeader(const DevicePrivateData* pDevicePrivateData, void* pBuffer, unsigned int nSize, unsigned short nOpcode)
{
	static unsigned short nId = 54;//after acquiring fwInfo from the device, the static num is 65,(and 55 without color sensor) 
	HostProtocolHeader* pHeader = (HostProtocolHeader*)pBuffer;
	pHeader->nMagic = 19783;/*pDevicePrivateData->FWInfo.nHostMagic*/
	pHeader->nSize = unsigned short(nSize/sizeof(unsigned short));
	pHeader->nOpcode = nOpcode;
	pHeader->nId = nId++;

	return TRUE;
}

BOOL HostProtocolGetTecFastConvergenceData(DevicePrivateData* pDevicePrivateData, TecFastConvergenceData* pTecData)
{
	unsigned char buffer[MAX_PACKET_SIZE] = {0};
	unsigned short  nDataSize;
	BOOL rc;

	pDevicePrivateData->FWInfo.nProtocolHeaderSize = 8;
	pDevicePrivateData->FWInfo.nOpcodeGetFastConvergenceTEC = 30;//38 from open ni(30 for sensor without color)
	pDevicePrivateData->FWInfo.nProtocolMaxPacketSize = 512;

	HostProtocolInitHeader(pDevicePrivateData, buffer, 0, pDevicePrivateData->FWInfo.nOpcodeGetFastConvergenceTEC);

	TecFastConvergenceData* pResult;

	rc = HostProtocolExecute(pDevicePrivateData, 
							 buffer, 
							 pDevicePrivateData->FWInfo.nProtocolHeaderSize,
							 pDevicePrivateData->FWInfo.nOpcodeGetFastConvergenceTEC,
							 (unsigned char**)(&pResult),
							 nDataSize);
	
// 	pTecData->m_SetPointVoltage = 0;
// 	pTecData->m_CompensationVoltage = 0;
// 	pTecData->m_TecDutyCycle = TecFastConvergenceData.m_TecDutyCycle;
// 	pTecData->m_HeatMode = TecFastConvergenceData.m_HeatMode;
// 	pTecData->m_ProportionalError = TecFastConvergenceData.m_ProportionalError;
// 	pTecData->m_IntegralError = TecFastConvergenceData.m_IntegralError;
// 	pTecData->m_DerivativeError = TecFastConvergenceData.m_DerivativeError;

	return rc;
}

BOOL HostProtocolGetTecData(DevicePrivateData* pDevicePrivateData, TecData* pTecData)
{
	unsigned char buffer[MAX_PACKET_SIZE] = {0};
	unsigned short	nDataSize;
	BOOL rc;
	
	pDevicePrivateData->FWInfo.nOpcodeGetTecData = 30;
	pDevicePrivateData->FWInfo.nProtocolHeaderSize = 8;
	pDevicePrivateData->FWInfo.nProtocolMaxPacketSize = 512;

	rc = HostProtocolInitHeader(pDevicePrivateData, buffer, 0, pDevicePrivateData->FWInfo.nOpcodeGetTecData);

	TecData * pResult;

	rc = HostProtocolExecute(pDevicePrivateData,
							 buffer,
							 pDevicePrivateData->FWInfo.nProtocolHeaderSize,
							 pDevicePrivateData->FWInfo.nOpcodeGetTecData,
							 (unsigned char**)(&pResult),
							 nDataSize);

	pTecData->m_SetPointVoltage = (pResult->m_SetPointVoltage);
	pTecData->m_CompensationVoltage = (pResult->m_CompensationVoltage);
	pTecData->m_TecDutyCycle = (pResult->m_TecDutyCycle);
	pTecData->m_HeatMode = (pResult->m_HeatMode);
	pTecData->m_ProportionalError = (pResult->m_ProportionalError);
	pTecData->m_IntegralError = (pResult->m_IntegralError);
	pTecData->m_DerivativeError = (pResult->m_DerivativeError);
	pTecData->m_ScanMode = (pResult->m_ScanMode);
// 	TecFastConvergenceData DeviceTecFastConvergenceData;
// 
// 	rc = HostProtocolGetTecFastConvergenceData(pDevicePrivateData, &DeviceTecFastConvergenceData);
// 
// 	pTecData->m_SetPointVoltage = 0;
// 	pTecData->m_CompensationVoltage = 0;
// 	pTecData->m_TecDutyCycle = DeviceTecFastConvergenceData.m_TecDutyCycle;
// 	pTecData->m_HeatMode = DeviceTecFastConvergenceData.m_HeatMode;
// 	pTecData->m_ProportionalError = DeviceTecFastConvergenceData.m_ProportionalError;
// 	pTecData->m_IntegralError = DeviceTecFastConvergenceData.m_IntegralError;
// 	pTecData->m_DerivativeError = DeviceTecFastConvergenceData.m_DerivativeError;

	return rc;
}