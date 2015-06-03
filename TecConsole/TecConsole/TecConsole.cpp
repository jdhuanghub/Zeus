//------------------------------------------------------------------------------------------
//TecConsole.cpp 
//Includes
//------------------------------------------------------------------------------------------
#include <Windows.h>
#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <map>

#include "DeviceSensor.h"
//#include "DeviceSensorIO.h"
#include "ConsoleHeadfile.h"
#include <malloc.h>

using namespace std;
//------------------------------------------------------------------------------------------
//Types and Defines
//and function declarations
//------------------------------------------------------------------------------------------
typedef bool (*cbfunc)(DevicePrivateData& Device, vector<string>& Command);

map<string, cbfunc> cbs;
map<string, cbfunc> mnemonics;
map<string, string> helps;

BOOL HostProtocolGetTecData(DevicePrivateData* pDevicePrivateData, TecData* pTecData);
BOOL USBOpenDevice(UsbDevHandle** pDevHandlePtr);
//------------------------------------------------------------------------------------------
//Functions
//
//------------------------------------------------------------------------------------------
bool atio2(const char* str, int* pOut)
{
	int output = 0;
	int base = 10;
	int start = 0;

	if (strlen(str) > 1 && str[0] == '0' && str[1] == 'x')
	{
		start = 2;
		base = 16;
	}

	for (size_t i = start; i < strlen(str); i++)
	{
		output *= base;
		if (str[i] >= '0' && str[i] <= '9')
			output += str[i]-'0';
		else if (base == 16 && str[i] >= 'a' && str[i] <= 'f')
			output += 10+str[i]-'a';
		else if (base == 16 && str[i] >= 'A' && str[i] <= 'F')
			output += 10+str[i]-'A';
		else
			return false;
	}
	*pOut = output;
	return true;
}

void mainloop(DevicePrivateData& Device, istream& istr, bool prompt)
{
	char buf[256];
	string str;

	vector<string> Command;

	while (istr.good())
	{
		if (prompt)
			cout << "> ";
		Command.clear();
		istr.getline(buf, 256);
		str = buf;
		size_t previous = 0, next = 0;

		while (1)
		{
			next = str.find(' ', previous);

			if (next != previous && previous != str.size())
				Command.push_back(str.substr(previous, next-previous));

			if (next == str.npos)
				break;

			previous = next+1;
		}

		if (Command.size() > 0)
		{
			if (Command[0][0] == ';')
				continue;

			for (unsigned int i = 0; i < Command[0].size(); i++)
				Command[0][i] = (char)tolower(Command[0][i]);

			if (cbs.find(Command[0]) != cbs.end())
			{
				if (!(*cbs[Command[0]])(Device, Command))
					return;
			}
			else if (mnemonics.find(Command[0]) != mnemonics.end())
			{
				if (!(*mnemonics[Command[0]])(Device, Command))
					return;
			}
			else
			{
				cout << "Unknown command \"" << Command[0] << "\"" << endl;
			}
		}
	}
}

bool byebye(DevicePrivateData& , vector<string>&)
{
	cout<<"Bye Bye"<<endl;
	return false;
}

bool Help(DevicePrivateData& , vector<string>& )
{
	for (map<string, cbfunc>::iterator iter = cbs.begin(); iter != cbs.end(); ++iter)
	{
		cout << "\"" << iter->first << "\" - " << helps[iter->first] << endl;
	}
	return true;
}

void RegisterCB(string cmd, cbfunc func, const string& strHelp)
{
	for (unsigned int i = 0; i < cmd.size(); i++)
	{
		cmd[i] = (char)tolower(cmd[i]);
	}
	cbs[cmd] = func;
	helps[cmd] = strHelp;
}

void RegisterMnemonic(string strMnemonic, string strCommand)
{
	for (unsigned int i = 0; i < strCommand.size(); i++)
	{
		strCommand[i] = (char)tolower(strCommand[i]);
	}
	for (unsigned int i = 0; i < strMnemonic.size(); i++)
	{
		strMnemonic[i] = (char)tolower(strMnemonic[i]);
	}
	if (cbs.find(strCommand) != cbs.end())
	{
		mnemonics[strMnemonic] = cbs[strCommand];
	}
}

bool OpenDevice(DevicePrivateData* Device)
{
	BOOL			Status = TRUE;
	//Opening the device
	Status = USBOpenDevice(&Device->SensorHandle.USBDevice);

	return true;
}

bool GetTecData(DevicePrivateData* pDevicePrivateData, vector<string>& )
{
	TecData         myTecData;
	BOOL			Status = TRUE;

	//Allocate mem for the sensorIO
	pDevicePrivateData = (DevicePrivateData*)malloc(sizeof(DevicePrivateData));
	pDevicePrivateData->SensorHandle.USBDevice = (UsbDevHandle*)malloc(sizeof(UsbDevHandle));
	
	//Opening the device
	Status = USBOpenDevice(&pDevicePrivateData->SensorHandle.USBDevice);

	Status = HostProtocolGetTecData(pDevicePrivateData,&myTecData);

	printf("\nSetPointVoltage: %hd\nCompensationVoltage: %hd\nDutyCycle: %hd\nHeatMode: %hd\nProportionalError: %d\nIntegralError: %d\nDeriativeError: %d\nScanMode: %hd\n",
		myTecData.m_SetPointVoltage, myTecData.m_CompensationVoltage, myTecData.m_TecDutyCycle, 
		myTecData.m_HeatMode, myTecData.m_ProportionalError, myTecData.m_IntegralError, 
		myTecData.m_DerivativeError, myTecData.m_ScanMode);

	return true;
}
bool Tec(DevicePrivateData& Device, vector<string>& Command)
{
	if (Command.size() > 1)
	{
		if (Command[1] == "calib")
		{
			printf("Command to be completed... \n");
		}
		else if (Command[1] == "get")
		{
			printf("getting Tec Data \n");
			return GetTecData(&Device, Command);
		}
	}

	cout << "Usage: " << Command[0] << "<calib/get> ..." << endl;
	return true;
}


//------------------------------------------------------------------------------------------
//Main Function
//
//------------------------------------------------------------------------------------------
int main (int argc, char *argv[])
{
	DevicePrivateData Device;

	printf("**********Console for getting Tec data Only********** \n");

	RegisterCB("help", &Help, "Get list of available commands");
	RegisterMnemonic("?", "help");

	RegisterCB("exit", &byebye, "Exit interactive mode");
	RegisterMnemonic("bye", "exit");

	RegisterCB("tec", &Tec, "Calibrate/Read TEC");

	if (argc == 1)
	{
		mainloop(Device, cin, true);
	}
	else
	{
		vector<string> Command;
		for (int i = 1; i < argc; i++)
		{
			Command.push_back(argv[i]);
		}

		for (unsigned int i = 0; i < Command[0].size(); i++)
			Command[0][i] = (char)tolower(Command[0][i]);

		if (Command[0].size() == 0)
		{
			//
		}
		else if (cbs.find(Command[0]) != cbs.end())
		{
			(*cbs[Command[0]])(Device, Command);
		}
		else if (mnemonics.find(Command[0]) != mnemonics.end())
		{
			(*mnemonics[Command[0]])(Device, Command);
		}
		else
		{
			cout << "Unknown command \"" << Command[0] << "\"" << endl;
		}

	}


	return 0;
}

