#include "Arduino.h"
#include "Ini.h"
#include "Motor.h"

Motor motor1(AIN1, AIN2, PWMA);
Motor motor2(BIN1, BIN2, PWMB);

void setup()
{
	pinMode(STBY, OUTPUT);

	pinMode(PWMA, OUTPUT);
	pinMode(AIN1, OUTPUT);
	pinMode(AIN2, OUTPUT);
	
	pinMode(PWMB, OUTPUT);
	pinMode(BIN1, OUTPUT);
	pinMode(BIN2, OUTPUT);
}

void loop()
{
	motor1.Move(Motor::Forward, 128);
	motor2.Move(Motor::Forward, 128);
	delay(250);
	Motor::Stop();
	delay(250);

	motor1.Move(Motor::Backward, 128);
	motor2.Move(Motor::Backward, 128);
	delay(250);
	Motor::Stop();
	delay(250);

	motor1.Move(Motor::Forward, 255);
	motor2.Move(Motor::Forward, 255);
	delay(250);
	Motor::Stop();
	delay(250);

	motor1.Move(Motor::Backward, 255);
	motor2.Move(Motor::Backward, 255);
	delay(250);
	Motor::Stop();
	delay(250);
}
