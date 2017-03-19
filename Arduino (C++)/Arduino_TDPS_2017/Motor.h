#pragma once
#ifndef MOTOR_H
#define MOTOR_H

#include "Arduino.h"
#include "Ini.h"

class Motor
{
public:
	Motor(int argIn1, int argIn2, byte argPwm);

	~Motor();

	//Speed: 1~254
	void Move(Direction argDir, byte argSpeed);

	void Stop();

	static void AllStop();

private:
	int In1;
	int In2;
	byte Pwm;
};

class MotorPair
{
public:
	static void Init();

	static Motor motorA;
	static Motor motorB;
};

#endif // !MOTOR_H
