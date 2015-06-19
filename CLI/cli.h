
#pragma once

#include <XnPlatform.h>
#include <XnOS.h>
#include <XnUSB.h>
#include <vector>
#include <map>

typedef bool (*cbfunc)(vector<string>& Command);

struct _dcmd
{
	char* cmd;
	cbfunc func;
	char* strHelp ;
};