#include "stdafx.h"
#include "cmd.h"
#include <iostream>
#include <iomanip>

using namespace std;

struct  _device_vid_pid
{
    unsigned short int vid;
    unsigned short int pid;
};


struct _device_vid_pid support_devices_list[]=
{
    { 0x1d27,0x0601},
    { 0x2BC5,0x0400},
    { 0x2BC5,0x0401},
}; 



cmd::cmd(void)
{
	m_vid = 0x1d27;
	m_pid = 0x0601;
	seq_num = 0x00;
}


cmd::~cmd(void)
{
}

int cmd::init(void)
{
	XnUInt32 n = 0;
	XnStatus rc;
	XnUInt actual_len;
	XnUInt8 obuf[512];
	XnUInt8 ibuf[512];

	XnStatus nRetVal = xnUSBInit();
	if (nRetVal == XN_STATUS_USB_ALREADY_INIT)
		nRetVal = XN_STATUS_OK;

    for (int i=0;i<sizeof(support_devices_list)/sizeof(struct  _device_vid_pid);i++)
    {

        rc = xnUSBEnumerateDevices(support_devices_list[i].vid, support_devices_list[i].pid, &m_astrDevicePaths, &n);
        if((rc == STATUS_OK && n!=0))
        {
            m_vid =support_devices_list[i].vid;
            m_pid =support_devices_list[i].pid;
            printf("Device vid=%x,pid=%x\r\n",m_vid,m_pid);
            break;
        }

    }
    if((rc != STATUS_OK)||(n==0))
    {
        printf("No Device\r\n");
        return -1;
    }



	rc = xnUSBOpenDeviceByPath(*m_astrDevicePaths, &m_hUSBDevice);
	if (rc != STATUS_OK)
	{
		cout << " Error: failed to open device" << hex <<m_vid << hex << m_pid << endl;
		return -1;
	}

	return 0;
}

int cmd::init_header(void *buf, XnUInt16 cmd, XnUInt16 data_len)
{
	protocol_header *pheader = (protocol_header *)buf;
	pheader->magic = CMD_HEADER_MAGIC;
	pheader->size =	data_len / 2;
	pheader->opcode = cmd;
	pheader->id = seq_num;
	seq_num++;

	return 0;
}

int cmd::send(void *cmd_req, XnUInt16 req_len, void *cmd_resp, XnUInt16 *resp_len)
{
	XnUInt32 actual_len;

	xnUSBSendControl(m_hUSBDevice, XN_USB_CONTROL_TYPE_VENDOR, 0x00, 0x0000,0x0000, (XnUChar*)cmd_req, req_len, 1000);
	do 
	{
		xnUSBReceiveControl(m_hUSBDevice, XN_USB_CONTROL_TYPE_VENDOR, 0x00, 0x0000,0x0000, (XnUChar *)cmd_resp, 0x200 , &actual_len, 1000);
	} while ((actual_len == 0) || (actual_len == 0x200));

	*resp_len = actual_len;

	return 0;
}

int cmd::get_version(void)
{
	int ret;
	XnUInt16 data_len;
	XnUInt16 resp_len;
	XnUInt16 i;

	data_len = 0;
	ret = init_header(req_buf, CMD_GET_VERSION, data_len);
	if (ret)
	{
		cout << "init header of get_version failed" << endl;
		return ret;
	}

	ret = send(req_buf, CMD_HEADER_LEN + data_len, resp_buf, &resp_len);
	if (ret)
		cout << "send cmd get_version failed" << endl;

	
	if (resp_len)
	{
		
		cout << "    FW Version: " 
			<< hex << setfill('0') << setw(2) << int(resp_buf[10]) << "."
			<< hex << setfill('0') << setw(2) << int(resp_buf[11]) 
			<< endl;
	}

	return ret;
}

int cmd::init_load_file(XnUInt32 offset, XnUInt32 size)
{
	int ret;
	XnUInt16 data_len;
	XnUInt16 resp_len;
	XnUInt16 i;
	XnUInt32 *p = NULL;

	data_len = 10;
	ret = init_header(req_buf, OPCODE_INIT_FILE_UPLOAD, data_len);
	p = (XnUInt32 *)(req_buf + 8);
	*p = offset;
	p = (XnUInt32 *)(req_buf + 12);
	*p = size;

	if (ret)
	{
		cout << "init header of init upload file failed" << endl;
		return ret;
	}

	ret = send(req_buf, CMD_HEADER_LEN + data_len, resp_buf, &resp_len);
	if (ret)
		cout << "send cmd init upload file failed" << endl;
	return ret;
}

int cmd::write_upload_file(XnUInt32 offset, void *buf, XnUInt32 size)
{
	int ret;
	XnUInt16 data_len;
	XnUInt16 resp_len;
	XnUInt16 i;
	XnUInt32 *p = NULL;

	data_len = size - 8;
	ret = init_header(buf, OPCODE_WRITE_FILE_UPLOAD, data_len);
	p = (XnUInt32 *)(req_buf + 8);
	*p = offset;

	if (ret)
	{
		cout << "write_upload_file failed" << endl;
		return ret;
	}

	ret = send(buf, CMD_HEADER_LEN + data_len, resp_buf, &resp_len);
	if (ret)
		cout << "send cmd write_upload_file failed" << endl;
	return ret;
}

int cmd::upload_file(XnUInt32 nOffset, const char *file)
{
	XnStatus nRetVal = XN_STATUS_OK;
	XnStatus rc;
	XnUInt64 nFileSize;
	XN_FILE_HANDLE UploadFile;
	XnUInt32 n;
	XnUInt32 nFileNextOffset = 0;
	XnUInt32 nFlashOffsetNext;
	XnUInt32 uploaded = 0;

	nFlashOffsetNext = nOffset;

		cout << "1" << endl;
	rc = xnOSGetFileSize64(file, &nFileSize);
	XN_IS_STATUS_OK(rc);
	cout << "2" << endl;
	rc = xnOSOpenFile(file, XN_OS_FILE_READ, &UploadFile);
	cout << "3" << endl;
	XN_IS_STATUS_OK(rc);

	cout << "Upload fie: " << file
		<< " offset: " << nOffset 
		<< "size: " << nFileSize << endl;

	n = (XnUInt32)nFileSize;
	init_load_file(nOffset, n);

	do {
		if (nFileSize >= 256)
			n = 256;
		else
			n = (XnUInt32)nFileSize;

		xnOSSeekFile64(UploadFile, XN_OS_SEEK_SET, nFileNextOffset);
		xnOSReadFile(UploadFile, req_buf + 12, &n);

		rc = write_upload_file(nFlashOffsetNext, req_buf, n + 12);
		if (rc != XN_STATUS_OK)
		{
			printf("failed\n");
			xnOSCloseFile(&UploadFile);
			return (rc);
		}
		uploaded += n;
		cout << "Uploaded " << uploaded << "bytes," << " remain " << nFileSize << endl;
		nFileNextOffset += n;
		nFileSize -= n;
		nFlashOffsetNext += n;
	} while(nFileSize);
	
	xnOSCloseFile(&UploadFile);

	return (XN_STATUS_OK);
}

int cmd::tec_ldp_set(XnUInt32 val)
{
	int ret;
	XnUInt16 data_len;
	XnUInt16 resp_len;
	XnUInt16 i;

	data_len = 2;
	ret = init_header(req_buf, CMD_SET_TEC_LDP, data_len);
	if (ret)
	{
		cout << "init header of set tec ldp failed" << endl;
		return ret;
	}

	req_buf[8] = val;
	req_buf[9] = (val & 0xff00) >> 8;
	ret = send(req_buf, CMD_HEADER_LEN + data_len, resp_buf, &resp_len);
	if (ret)
		cout << "send cmd  set tec ldp failed" << endl;

	return ret;
}

int cmd::read_flash(XnUInt32 offset, XnUInt16 size)
{
	int ret;
	XnUInt16 data_len;
	XnUInt16 resp_len;
	XnUInt16 i;
	XnUInt32 *p = NULL;

	data_len = 6;
	ret = init_header(req_buf, OPCODE_READ_FLASH, data_len);
	if (ret)
	{
		cout << "init header of set tec ldp failed" << endl;
		return ret;
	}

	p = (XnUInt32 *)(req_buf + 8);
	*p = offset;
	req_buf[12] = size;
	req_buf[13] = size >> 8;

	ret = send(req_buf, CMD_HEADER_LEN + data_len, resp_buf, &resp_len);
	if (ret)
		cout << "send cmd  set tec ldp failed" << endl;
	
	for (i = 0; i < resp_len - 10; i++)
	{
		if (i > 0 && i % 16 ==0)
			printf("\n");
		printf("%02x ", *(resp_buf + 10 + i));
	}
	printf("\n");

	return ret;
}

int cmd::gain_set(XnUInt32 val)
{
	int ret;
	XnUInt16 data_len;
	XnUInt16 resp_len;
	XnUInt16 i;

	data_len = 2;
	ret = init_header(req_buf, CMD_GAIN_SET, data_len);
	if (ret)
	{
		cout << "init header of set tec ldp failed" << endl;
		return ret;
	}

	req_buf[8] = val;
	req_buf[9] = (val & 0xff00) >> 8;
	ret = send(req_buf, CMD_HEADER_LEN + data_len, resp_buf, &resp_len);
	if (ret)
		cout << "send cmd  set tec ldp failed" << endl;

	return ret;
}