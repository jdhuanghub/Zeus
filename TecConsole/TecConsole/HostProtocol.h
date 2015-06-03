#ifndef _HOSTPROTOCOL_H
#define _HOSTPROTOCOL_H
//---------------------------------------------------------------------------
// Includes
//---------------------------------------------------------------------------


//---------------------------------------------------------------------------
// Types
//---------------------------------------------------------------------------
typedef struct usbHostProtocolHeader
{
	unsigned short nMagic;
	unsigned short nSize;
	unsigned short nOpcode;
	unsigned short nId;
}HostProtocolHeader;

typedef struct
{
	unsigned short nErrorCode;
} HostProtocolReplyHeader;




#endif