#include "Arduino.h"
#include "Serial.h"
#include "Motor.h"

CommandResult::CommandResult(bool argIsData)
{
	isData = argIsData;
}

CommandResult::~CommandResult()
{

}

MotorResult::MotorResult(Direction argMotorDirection1, byte argMotorSpeed1, Direction argMotorDirection2, byte argMotorSpeed2) : CommandResult(true)
{
	CurrentResult = ResultState::MotorResultState;
	motorDirection1 = argMotorDirection1;
	motorDirection2 = argMotorDirection2;
	motorSpeed1 = argMotorSpeed1;
	motorSpeed2 = argMotorSpeed2;
}

SerialPort::~SerialPort()
{

}

SerialPort::SerialPort()
{

}

CommandResult SerialPort::CheckAvailable()
{
	while (Serial.available())
	{
		int data = Serial.read();
		if (data == 0x00)
		{
			//Motor Data Pack
			if (buffer[0] == 0x01)
			{
				if (bufferLoc != 5)
				{
					//Error
					Error::Stop();
				}
				Direction dir1, dir2;
				if (buffer[1] == 0x01) { dir1 = Forward; }
				else { dir1 == Backward; }
				if (buffer[3] == 0x01) { dir2 = Forward; }
				else { dir2 == Backward; }

				MotorPair::motorA.Move(dir1, buffer[2]);
				MotorPair::motorB.Move(dir2, buffer[4]);

				MotorResult returnResult(dir1, buffer[2], dir2, buffer[4]);
#ifdef DEBUG_SERIAL
				Serial.print("Motor: ");
				Serial.print((int)dir1);
				Serial.print(" ");
				Serial.print((int)buffer[2]);
				Serial.print(" ");
				Serial.print((int)dir2);
				Serial.print(" ");
				Serial.println((int)buffer[4]);
#endif
				bufferLoc = 0;
				for (int i = 0; i < BUFFER_SIZE; i++)
				{
					buffer[i] = 0;
				}
				return MotorResult(returnResult);
			}
			//Other Data Pack
			else if (true)
			{
				return CommandResult(false);
			}
		}
		else
		{
			buffer[bufferLoc] = static_cast<byte>(data);
			bufferLoc++;
		}
	}
	return CommandResult(false);
}

void SerialPort::Init()
{
	if (!IsInit)
	{
		Serial.begin(BAUD_SERIAL);
		IsInit = true;
	}
}

bool SerialPort::IsInit = false;
byte SerialPort::buffer[BUFFER_SIZE];
byte SerialPort::bufferLoc = 0;
