#pragma once
#include <iostream>
#include <thread>
using namespace std;

class ConsoleMMF
{
public:
	ConsoleMMF();
	~ConsoleMMF();

	bool m_bRunningState;

private:
	void DisplayConsole();
	void ConsoleCommands();
};
