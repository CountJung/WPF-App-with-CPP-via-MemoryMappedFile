#include "ConsoleMMF.h"

void ThreadConsole()
{
	ConsoleMMF console;
	while (console.m_bRunningState)
		this_thread::sleep_for(chrono::milliseconds(1));
}

int main(int argc, char** argv)
{
	//cout << "Hello CPP?" << endl;
	thread consoleThread = thread(ThreadConsole);
	consoleThread.join();
	return 0;
}

ConsoleMMF::ConsoleMMF()
{
	m_bRunningState = true;
	m_pFileMappingHandle = NULL;
	m_pSharedMemData = NULL;
	LinkMMFData();
	ConsoleCommands();
}

ConsoleMMF::~ConsoleMMF()
{
}

void ConsoleMMF::DisplayConsole()
{
	cout << "=================================================" << endl;
	cout << "==== Console Memory Mapped File Context Test ====" << endl;
	cout << "==== Licence is MIT  ============================" << endl;
	cout << "==== Commands shown as below ====================" << endl;
	cout << "==== exit = exit console application ============" << endl;
	cout << "==== int = Change WPF TextBlock Color ===========" << endl;
	cout << "==== double = Change WPF TextBlock Data =========" << endl;
	cout << "==== text = Change WPF TextBlock ================" << endl;
	cout << "=================================================" << endl;
}

void ConsoleMMF::ConsoleCommands()
{
	DisplayConsole();
	string str;
	while (m_bRunningState)
	{
		cout << "Command :";
		cin >> str;
		cout << "Input : " << str << endl;
		if (str == "exit")
			m_bRunningState = false;
		else if (str == "int")
		{
			cout << "Textblock background color change to" << endl;
			cout << "0:Teal, 1:Orange, 2:Pink, Default:Beige" << endl;
			cout << "Input int : ";
			cin >> str;
			cout << "int Data = " << str << endl;
			m_pSharedMemData->integerData = stoi(str);
		}
		else if (str == "double")
		{
			cout << "Textblock double data" << endl;
			cout << "Input double : ";
			cin >> str;
			cout << "double Data = " << str << endl;
			m_pSharedMemData->doubleData = stod(str);
		}
		else if (str == "text")
		{
			cout << "Textblock string data" << endl;
			cout << "Input text : ";
			cin >> str;
			cout << "text Data = " << str << endl;
			strcpy_s(m_pSharedMemData->stringData, str.c_str());
		}
		else
			DisplayConsole();
	}
}

void ConsoleMMF::LinkMMFData()
{
	if ((m_pFileMappingHandle = CreateFileMapping(INVALID_HANDLE_VALUE, 0, PAGE_READWRITE, 0, /*268*/sizeof(SharedData), L"WPFWithCPPMMF")) == NULL)
	{
		cout << "Map Handle Fail" << endl;
		return;
	}
	if ((m_pSharedMemData = (SharedData*)MapViewOfFile(m_pFileMappingHandle, FILE_MAP_ALL_ACCESS, 0, 0, /*268*/sizeof(SharedData))) == NULL)
	{
		cout << "ShareMemData Fail" << endl;
		return;
	}
}
