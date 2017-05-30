//	Serial buffer size:10 bytes
//	Flush buffer after every pack acquiring
//	Format:
//			|->	Motor		0x01 (1 byte)	A motor (2 byte)						B motor (2 bytes)						Time	(2 bytes)			0xFF (1 byte)
//						|->		Start Sign			0x00 0x01(Direction PWM)		0x00 0x01(Direction PWM)										End sign
//
//	Motor moves only when both of them get their packs.
#pragma once
#ifndef SERIAL_H
#define SERIAL_H

#include "Arduino.h"
#include "Ini.h"
#include "Error.h"

class CommandResult
{
public:
	enum ResultState
	{
		MotorResultState = 0
	};

	CommandResult(bool argIsData);

	~CommandResult();

	ResultState CurrentResult;

	bool isData;
};

class MotorResult : public CommandResult
{
public:
	MotorResult(Direction argMotorDirection1, byte argMotorSpeed1, Direction argMotorDirection2, byte argMotorSpeed2, int argTime);

	int motorSpeed1;
	int motorSpeed2;
	int time;
	Direction motorDirection1;
	Direction motorDirection2;
};

class SerialPort
{
	//Singleton
public:
	~SerialPort();
	SerialPort();
	static CommandResult CheckAvailable();
	static void Init();
private:
	static bool IsInit;
	static byte buffer[];
	static byte bufferLoc;
};
#endif // !SERIAL_H
