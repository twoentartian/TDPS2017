#pragma once
#ifndef MOTOR_H
#define MOTOR_H

#include "Arduino.h"
#include "Ini.h"
class Motor
{
public:
	enum Direction
	{
		Forward = 0, Backward = 1
	};

	Motor(int argIn1, int argIn2, int argPwm);

	~Motor();

	//Speed: 1~255
	void Move(Direction argDir, int argSpeed);

	static void Stop();

private:
	int In1;
	int In2;
	int Pwm;
};


#endif // !MOTOR_H
