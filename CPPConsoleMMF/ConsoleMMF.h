#pragma once
#include <iostream>
#include <string>
#include <thread>
using namespace std;
#define WIN32_LEAN_AND_MEAN
#define _WIN32_WINNT _WIN32_WINNT_WINXP
#include <Windows.h>
#include <WinSock2.h>

//Data Paddig Refernce
//https://en.wikipedia.org/wiki/Data_structure_alignment
struct SharedData
{
	int integerData;       //4 byte
	double doubleData;     //8 byte
	char stringData[256];  //256 byte
}; //But, Actual Size of this Structure is 272  4 + 8 + 256 = 268 + 4 (Padding) = 272

class ConsoleMMF
{
public:
	ConsoleMMF();
	~ConsoleMMF();

	bool m_bRunningState;
	HANDLE m_pFileMappingHandle;
	SharedData *m_pSharedMemData;

private:
	void DisplayConsole();
	void ConsoleCommands();
	void LinkMMFData();
};
