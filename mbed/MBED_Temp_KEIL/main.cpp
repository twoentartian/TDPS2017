#include "mbed.h"
#include "PwmOut.h"


Serial PC(USBTX, USBRX);
Serial MPU6050_Serial(p9, p10);

DigitalOut LEDS[4] =
{
	LED1,LED2,LED3,LED4
};

void Error()
{

	while (true)
	{
		for (int i = 0; i < 4; i ++)
		{
			LEDS[0] = 0;
			LEDS[1] = 0;
			LEDS[2] = 0;
			LEDS[3] = 0;
			LEDS[i] = 1;
			wait(1);
		}
	}
}

#pragma region Motor

Timeout timeoutStop;

DigitalOut _A1(p29);
DigitalOut _A2(p30);
PwmOut _Apwm(p23);
DigitalOut _B1(p27);
DigitalOut _B2(p26);
PwmOut _Bpwm(p22);
DigitalOut _stby(p28);

enum Direction
{
	Forward, Backward
};


void TB6612_Init(PinName A1, PinName A2, PinName Apwm, PinName B1, PinName B2, PinName Bpwm, PinName stby)
{
	
}

void TB6612_Stop()
{
	_A1 = 0;
	_A2 = 0;
	_Apwm = 0;
	_B1 = 0;
	_B2 = 0;
	_Bpwm = 0;
	_stby = 0;
	LEDS[0] = 0;
}

void TB6612_Move(Direction DirLeft, unsigned char SpeedLeft, Direction DirRight, unsigned char SpeedRight, int time)
{
	_stby = 1;
	//Motor Left
	if (DirLeft == Forward)
	{
		_A1 = 1;
		_A2 = 0;
	}
	else
	{
		_A1 = 0;
		_A2 = 1;
	}
	_Apwm = static_cast<float>(SpeedLeft) / 256;
	

	//Motor Right
	if (DirRight == Forward)
	{
		_B1 = 1;
		_B2 = 0;
	}	 
	else
	{	 
		_B1 = 0;
		_B2 = 1;
	}
	_Bpwm = static_cast<float>(SpeedRight) / 256;
	LEDS[0] = 1;

	timeoutStop.attach(TB6612_Stop, static_cast<float>(time) / 1000);
}


#pragma endregion


#pragma region Data
class Data
{
public:
	static float xAcc;
	static float yAcc;
	static float zAcc;

	static float xAngleSpeed;
	static float yAngleSpeed;
	static float zAngleSpeed;

	static float xAngle;
	static float yAngle;
	static float zAngle;

	static float Temp;
};
float Data::xAcc = 0;
float Data::yAcc = 0;
float Data::zAcc = 0;
float Data::xAngleSpeed = 0;
float Data::yAngleSpeed = 0;
float Data::zAngleSpeed = 0;
float Data::xAngle = 0;
float Data::yAngle = 0;
float Data::zAngle = 0;
float Data::Temp = 0;
#pragma endregion

#pragma region MPU6050

static void FlashData()
{
	while (MPU6050_Serial.readable())
	{
		if (MPU6050_Serial.getc() == 0x55)
		{
			unsigned char buffer[9];
			char DataPackSign = MPU6050_Serial.getc();
			unsigned char sum = 0x55 + DataPackSign;
			switch (DataPackSign)
			{
			case 0x51:				//Acc
				for (int i = 0; i < 9; i++)
				{
					buffer[i] = MPU6050_Serial.getc();
					if (i == 8)
					{
						if (sum != buffer[8])
						{
							return;
						}
						Data::xAcc =static_cast<float>((buffer[1] << 8) | buffer[0]) / 32768 * 16 * 9.8;
						Data::yAcc =static_cast<float>((buffer[3] << 8) | buffer[2]) / 32768 * 16 * 9.8;
						Data::zAcc =static_cast<float>((buffer[5] << 8) | buffer[4]) / 32768 * 16 * 9.8;
						Data::Temp = static_cast<float>((buffer[7] << 8) | buffer[6]) / 340 + 36.53;
					}
					else
					{
						sum += buffer[i];
					}
				}
				break;
			case 0x52:				//AngleSpeed
				for (int i = 0; i < 9; i++)
				{
					buffer[i] = MPU6050_Serial.getc();
					if (i == 8)
					{
						if (sum != buffer[8])
						{
							return;
						}
						Data::xAngleSpeed =static_cast<float>((buffer[1] << 8) | buffer[0]) / 32768 * 2000;
						Data::yAngleSpeed =static_cast<float>((buffer[3] << 8) | buffer[2]) / 32768 * 2000;
						Data::zAngleSpeed =static_cast<float>((buffer[5] << 8) | buffer[4]) / 32768 * 2000;
						Data::Temp = static_cast<float>((buffer[7] << 8) | buffer[6]) / 340 + 36.53;
					}
					else
					{
						sum += buffer[i];
					}
				}
				break;
			case 0x53:				//Angle
				for (int i = 0; i < 9; i++)
				{
					buffer[i] = MPU6050_Serial.getc();
					if (i == 8)
					{
						if (sum != buffer[8])
						{
							return;
						}
						Data::xAngle = static_cast<float>((buffer[1] << 8) | buffer[0]) / 32768 * 180;
						Data::yAngle = static_cast<float>((buffer[3] << 8) | buffer[2]) / 32768 * 180;
						Data::zAngle = static_cast<float>((buffer[5] << 8) | buffer[4]) / 32768 * 180;
						Data::Temp = static_cast<float>((buffer[7] << 8) | buffer[6]) / 340 + 36.53;
					}
					else
					{
						sum += buffer[i];
					}
				}

				break;
			default:
				
				break;
			}
		}
	}
}

#pragma endregion

void Init()
{
	MPU6050_Serial.baud(9600);
	MPU6050_Serial.attach(&FlashData);

	PC.baud(115200);

}

int main()
{
	Init();
	while (true)
	{
		TB6612_Move(Forward, 255, Forward, 255, 1000);
		wait(5);
		TB6612_Move(Backward, 255, Backward, 255, 1000);
		wait(5);
	}
}
