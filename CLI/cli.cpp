#include "stdafx.h"
#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <map>
#include <XnOS.h>
#include "cmd.h"

using namespace std;

class cmd cmd;
typedef bool (*cbfunc)(vector<string>& Command);

map<string, cbfunc> cbs;
map<string, cbfunc> mnemonics;
map<string, string>  helps;

bool atoi2(const char* str, int* pOut)
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


void mainloop(istream& istr, bool prompt)
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
				if (!(*cbs[Command[0]])(Command))
					continue;
			}
			else if (mnemonics.find(Command[0]) != mnemonics.end())
			{
				if (!(*mnemonics[Command[0]])(Command))
					continue;
			}
			else
			{
				cout << "Unknown command \"" << Command[0] << "\"" << endl;
			}
		}
	}
}

bool byebye(vector<string>& /*Command*/)
{
	cout << "Bye bye" << endl;
	return false;
}

bool Version(vector<string>& Command)
{
	cmd.get_version();

	return true;
}

bool Help(vector<string>& Command)
{
	for (map<string, cbfunc>::iterator iter = cbs.begin(); iter != cbs.end(); ++iter)
	{
		cout << "\"" << iter->first << "\" - " << helps[iter->first] << endl;
	}

	return true;
}

bool Upload(vector<string>& Command)
{
	int nOffset;
	const char *file;

	if (Command.size() < 3)
	{
		cout << "Usage: " << Command[0] << " <offset> <file> [<-f>]" << endl;
		return false;
	}


	if (!atoi2(Command[1].c_str(), &nOffset))
	{
		printf("Can't understand %s as offset\n", Command[1].c_str());
		return false;
	}

    if (nOffset<0x10000)
    {
        if (Command.size() < 4)
        {
            printf("Add -f if you want refresh the bootloader \n");
            return false;
        }
        if (strcmp(Command[3].c_str(),"-f"))
        {
            printf("Add -f if you want refresh the bootloader \n");
            return false;
        }
    }




	cout << Command[2].c_str() << endl;
	file = Command[2].c_str();
	cmd.upload_file(nOffset, file);

	return true;
}

bool Tec_LDP_SET(vector<string>& Command)
{
	XnInt32 val;
	if (!atoi2(Command[1].c_str(), &val))
	{
		printf("Can't understand %s as val\n", Command[1].c_str());
		return true;
	}
	cmd.tec_ldp_set((XnUInt32) val);

	return true;
}

bool gain_set(vector<string>& Command)
{
	XnInt32 val;
	if (!atoi2(Command[1].c_str(), &val))
	{
		printf("Can't understand %s as val\n", Command[1].c_str());
		return true;
	}
	cmd.gain_set((XnUInt32) val);

	return true;
}

bool read_flash(vector<string>& Command)
{
	int nOffset;
	int size;

	if (Command.size() < 3)
	{
		cout << "Usage: " << Command[0] << " <offset> <size> " << endl;
		return true;
	}

	if (!atoi2(Command[1].c_str(), &nOffset))
	{
		printf("Can't understand %s as offset\n", Command[1].c_str());
		return true;
	}

	if (!atoi2(Command[2].c_str(), (int*)(&size)))
	{
		printf("Can't understand %s as size\n", Command[1].c_str());
		return true;
	}

	cmd.read_flash(nOffset, size);

	return true;
}

void RegisterCB(string cmd, cbfunc func, const string& strHelp)
{
	for (unsigned int i = 0; i < cmd.size(); i++)
		cmd[i] = (char)tolower(cmd[i]);
	cbs[cmd] = func;
	helps[cmd] = strHelp;
}

void RegisterMnemonic(string strMnemonic, string strCommand)
{
	for (unsigned int i = 0; i < strCommand.size(); i++)
		strCommand[i] = (char)tolower(strCommand[i]);
	for (unsigned int i = 0; i < strMnemonic.size(); i++)
		strMnemonic[i] = (char)tolower(strMnemonic[i]);

	if (cbs.find(strCommand) != cbs.end())
	{
		mnemonics[strMnemonic] = cbs[strCommand];
	}
}

int main(int argc, char **argv)
{
	cmd.init();

	RegisterCB("exit", &byebye, "Exit interactive mode");
	RegisterMnemonic("bye", "exit");

	RegisterCB("help", &Help, "Get list of available commands");
	RegisterMnemonic("?", "help");
	RegisterCB("version", &Version, "Get version");
	RegisterCB("upload", &Upload, "Upload binary files to flash");
	RegisterCB("readflash", &read_flash, "read datas from flash");
	RegisterCB("tec_ldp_set", &Tec_LDP_SET, "enable/disabel tec ldp function, reset system is needed");
	RegisterCB("gain_set", &gain_set, "set IR CMOS GAIN");


	if (argc == 1)
	{
		mainloop(cin, true);
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
			(*cbs[Command[0]])(Command);
		}
		else if (mnemonics.find(Command[0]) != mnemonics.end())
		{
			(*mnemonics[Command[0]])(Command);
		}
		else
		{
			cout << "Unknown command \"" << Command[0] << "\"" << endl;
		}
	}

	return 0;
}
