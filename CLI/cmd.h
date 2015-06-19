
#pragma once

#include <XnPlatform.h>
#include <XnOS.h>
#include <XnUSB.h>

enum EPsProtocolOpCodes
{
	OPCODE_GET_VERSION = 0,
	OPCODE_KEEP_ALIVE = 1,
	OPCODE_GET_PARAM = 2,
	OPCODE_SET_PARAM = 3,
	OPCODE_GET_FIXED_PARAMS = 4,
	OPCODE_GET_MODE = 5,
	OPCODE_SET_MODE = 6,
	OPCODE_GET_LOG = 7,
	OPCODE_RESERVED_0 = 8,
	OPCODE_RESERVED_1 = 9,
	OPCODE_I2C_WRITE = 10,
	OPCODE_I2C_READ = 11,
	OPCODE_TAKE_SNAPSHOT = 12,
	OPCODE_INIT_FILE_UPLOAD = 13,
	OPCODE_WRITE_FILE_UPLOAD = 14,
	OPCODE_FINISH_FILE_UPLOAD = 15,
	OPCODE_DOWNLOAD_FILE = 16,
	OPCODE_DELETE_FILE = 17,
	OPCODE_GET_FLASH_MAP = 18,
	OPCODE_GET_FILE_LIST = 19,
	OPCODE_READ_AHB = 20,
	OPCODE_WRITE_AHB = 21,
	OPCODE_ALGORITM_PARAMS = 22,
	OPCODE_SET_FILE_ATTRIBUTES = 23,
	OPCODE_EXECUTE_FILE = 24,
	OPCODE_READ_FLASH = 25,
	OPCODE_SET_GMC_PARAMS = 26,
	OPCODE_GET_CPU_STATS = 27,
	OPCODE_BIST = 28,
	OPCODE_CALIBRATE_TEC = 29,
	OPCODE_GET_TEC_DATA = 30,
	OPCODE_CALIBRATE_EMITTER = 31,
	OPCODE_GET_EMITTER_DATA = 32,
	OPCODE_CALIBRATE_PROJECTOR_FAULT = 33,
	OPCODE_SET_CMOS_BLANKING = 34,
	OPCODE_GET_CMOS_BLANKING = 35,
	OPCODE_GET_CMOS_PRESETS = 36,
	OPCODE_GET_SERIAL_NUMBER = 37,
	OPCODE_GET_FAST_CONVERGENCE_TEC = 38,
	OPCODE_GET_PLATFORM_STRING = 39,
	OPCODE_GET_USB_CORE_TYPE = 40,
	OPCODE_SET_LED_STATE = 41,
	OPCODE_ENABLE_EMITTER = 42,
	CMD_GET_VERSION = 80,
	CMD_SET_TEC_LDP = 81,
	CMD_GAIN_SET = 87,
	OPCODE_KILL = 999,
};


typedef struct {
	XnUInt16 magic;
	XnUInt16 size;
	XnUInt16 opcode;
	XnUInt16 id;
} protocol_header;

typedef enum
{
	STATUS_OK = 0,
	STATUS_ERROR = 1,
	STATUS_NOT_IMPLEMENTED = 2,
	STATUS_NOT_SUPPORTED = 3,
	STATUS_BAD_PARAMETER = 4,
	STATUS_OUT_OF_FLOW = 5,
	STATUS_NO_DEVICE = 6,
	STATUS_TIME_OUT = 102,
} Status;

#define CMD_HEADER_MAGIC	(0x4d47)
#define CMD_HEADER_LEN		(0x08)

class cmd
{
public:
	cmd(void);
	~cmd(void);
public:
	int init(void);
	int get_version(void);
	int upload_file(XnUInt32 offset, const char *file);
	int tec_ldp_set(XnUInt32 val);
	int read_flash(XnUInt32 offset, XnUInt16 size);
	int gain_set(XnUInt32 val);
private:
	XnUInt16 seq_num;
	XnUInt8	req_buf[512];
	XnUInt8	resp_buf[512];
	XnUInt16 m_vid;
	XnUInt16 m_pid;
	XN_USB_DEV_HANDLE m_hUSBDevice;	
	const XnUSBConnectionString* m_astrDevicePaths;
private:
	int init_header(void *buf, XnUInt16 cmd, XnUInt16 data_len);
    int send(void *cmd_req, XnUInt16 req_len, void *cmd_resp, XnUInt16 *resp_len);
	int init_load_file(XnUInt32 offset, XnUInt32 size);
	int write_upload_file(XnUInt32 offset, void *buf, XnUInt32 size);
};

