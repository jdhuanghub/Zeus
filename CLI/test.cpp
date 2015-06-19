// test.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include <XnPlatform.h>
#include <XnOS.h>
#include <iostream>
#include "cli.h"

using namespace std;

class cli cli;

int _tmain(int argc, _TCHAR* argv[])
{
#if 0
	XnChar* strFileName;
	XnUInt64 nFileSize;
	XnStatus rc;

	rc = xnOSGetFileSize64(strFileName, &nFileSize);
	XN_IS_STATUS_OK(rc);
#endif

	return 0;
}

