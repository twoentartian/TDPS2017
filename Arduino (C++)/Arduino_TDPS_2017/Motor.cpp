#include "Arduino.h"
#include "Motor.h"

Motor::Motor(int argIn1, int argIn2, byte argPwm)
{
	pinMode(STBY, OUTPUT);
	this->In1 = argIn1;
	this->In2 = argIn2;
	this->Pwm = argPwm;

	pinMode(this->In1, OUTPUT);
	pinMode(this->In2, OUTPUT);
	pinMode(this->Pwm, OUTPUT);
}

Motor::~Motor()
{

}

void Motor::Move(Direction argDir, byte argSpeed)
{
	digitalWrite(STBY, HIGH); //disable standby
	if (argDir == Direction::Forward)
	{
		digitalWrite(In1, HIGH);
		digitalWrite(In2, LOW);
	}
	else if (argDir == Direction::Backward)
	{
		digitalWrite(In1, LOW);
		digitalWrite(In2, HIGH);
	}
	else
	{
		//Never reach
	}

	analogWrite(Pwm, argSpeed);
}

void Motor::Stop()
{
	digitalWrite(In1, LOW);
	digitalWrite(In2, LOW);
	analogWrite(Pwm, 0);
}


void Motor::AllStop()
{
	digitalWrite(STBY, LOW);
}

void MotorPair::Init()
{
	motorA = Motor(AIN1, AIN2, PWMA);
	motorB = Motor(BIN1, BIN2, PWMB);
}

Motor MotorPair::motorA(AIN1, AIN2, PWMA);
Motor MotorPair::motorB(BIN1, BIN2, PWMB);
