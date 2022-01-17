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
	cout << "==== Text = Change WPF TextBlock ================" << endl;
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
		else
			DisplayConsole();
	}
}
