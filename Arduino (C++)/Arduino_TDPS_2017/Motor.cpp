#include "Arduino.h"
#include "Motor.h"

Motor::Motor(int argIn1, int argIn2, int argPwm)
{
	this->In1 = argIn1;
	this->In2 = argIn2;
	this->Pwm = argPwm;
}

Motor::~Motor()
{

}

void Motor::Move(Direction argDir, int argSpeed)
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
	digitalWrite(STBY, LOW);
}
