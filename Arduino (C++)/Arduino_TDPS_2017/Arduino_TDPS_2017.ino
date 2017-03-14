#include "Arduino.h"
#include "Serial.h"
#include "Motor.h"


void setup()
{
	SerialPort::Init();
	MotorPair::Init();
}

void loop()
{
	CommandResult cr =  SerialPort::CheckAvailable();
}
