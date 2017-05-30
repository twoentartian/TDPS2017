#include "mbed.h"
#include "PID.h"

Serial MicroUSB(USBTX, USBRX);
Serial PC(p28, p27);
Serial MPU6050_Serial(p9, p10);

DigitalOut LEDS[4] =
{
	LED1,LED2,LED3,LED4
};

#pragma region Error

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

#pragma endregion

#pragma region Motor

Timeout timeoutStop;

DigitalOut _A1(p29);
DigitalOut _A2(p30);
PwmOut _Apwm(p23);
DigitalOut _B1(p25);
DigitalOut _B2(p24);
PwmOut _Bpwm(p22);
DigitalOut _stby(p26);

bool MOTOR_RunSign = false;

enum Direction
{
	Forward, Backward
};

void MOTOR_Stop()
{
	MOTOR_RunSign = false;
	_A1 = 0;
	_A2 = 0;
	_Apwm = 0;
	_B1 = 0;
	_B2 = 0;
	_Bpwm = 0;
	_stby = 0;
	LEDS[0] = 0;
}

void MOTOR_Move(Direction DirLeft, unsigned char SpeedLeft, Direction DirRight, unsigned char SpeedRight, unsigned int time)
{
	MOTOR_RunSign = true;
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

	timeoutStop.attach(MOTOR_Stop, static_cast<float>(time) / 1000);
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
	static int zAngleTurns;
	static float zAnglePrevious;

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
int Data::zAngleTurns = 0;
float Data::zAnglePrevious = 0;
float Data::Temp = 0;

#pragma endregion

#pragma region MPU6050

static void MPU6050_FlashData()
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
						float zAngleNow = static_cast<float>((buffer[5] << 8) | buffer[4]) / 32768 * 180;
						if (Data::zAnglePrevious > 270 && zAngleNow < 90)
						{
							Data::zAngleTurns++;
						}
						if (Data::zAnglePrevious < 90 && zAngleNow > 270)
						{
							Data::zAngleTurns--;
						}
						Data::zAngle = Data::zAngleTurns * 360 + zAngleNow;
						Data::Temp = static_cast<float>((buffer[7] << 8) | buffer[6]) / 340 + 36.53;
						Data::zAnglePrevious = zAngleNow;
						//MicroUSB.printf("ZANGLE = %f\n", Data::zAngle);
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

#pragma region Turns an Angle (PID)

#define RATE 0.1									//Wait time
#define DEFAULT_P	0.6
#define DEFAULT_I	2.0
#define DEFAULT_D	0.008

PID controller(DEFAULT_P, DEFAULT_I, DEFAULT_D, RATE);

float PidP = DEFAULT_P;
float PidI = DEFAULT_I;
float PidD = DEFAULT_D;

void PID_Turns(int angle)
{
	
	if (angle > 0)
	{
		controller.setInputLimits(Data::zAngle - 90, Data::zAngle + 360);
	}
	else
	{
		controller.setInputLimits(Data::zAngle - 360, Data::zAngle + 90);
	}
	
	controller.setOutputLimits(-1.0, 1.0);
	controller.setBias(0.0);
	controller.setMode(AUTO_MODE);

	float targetAngle = Data::zAngle + angle;
	controller.setSetPoint(targetAngle);

	while (true)
	{
		controller.setProcessValue(Data::zAngle);
		float result = controller.compute();
		Direction dir1, dir2;
		if (result > 0)
		{
			dir1 = Backward;
			dir2 = Forward;
		}
		else
		{
			dir1 = Forward;
			dir2 = Backward;
		}
		MicroUSB.printf("Angle now: %f result=%f\n", Data::zAngle, result);
		MOTOR_Move(dir1, static_cast<unsigned char>(255 * abs(result)), dir2, static_cast<unsigned char>(255 * abs(result)), RATE*1000);
		wait(RATE);
		if (abs(Data::zAngle - targetAngle) < 1)
		{
			break;
		}
	}





	while (Data::zAngle > 360)
	{
		Data::zAngle = Data::zAngle - 360;
	}
	while (Data::zAngle < 0)
	{
		Data::zAngle = Data::zAngle + 360;
	}

}


#pragma endregion

#pragma region Serial

//	Serial buffer size:10 bytes
//	Flush buffer after every pack acquiring
//	Format:
//			|->	Motor		0x01 (1 byte)	A motor (2 byte)						B motor (2 bytes)						Time	(2 bytes)			0xFF (1 byte)
//						|->		Start Sign			0x00 0x01(Direction PWM)		0x00 0x01(Direction PWM)										End Sign
//
//			|->	MPU			0x02					Type Def (1 byte)																										0xFF (1 byte)
//						|->		Start Sign			0x00(Acc)  0x01(AngleSpeed)  0x02(Angle)  0x03(Temperature)								End Sign
//
//			|->	Motor		0x03					Turn an angle (2 byte)																								0xFF (1 byte)
//						|->		Start Sign			-: counter clock wise		+: clock wise																		End Sign
//
//			|->	MPU Z Angle SendBack	0x04																															0xFF (1 byte)
//						|->								Start Sign																													End SIgn
//
//
//
//	Motor moves only when both of them get their packs.
//

#define BUFFER_SIZE	10
unsigned char buffer[BUFFER_SIZE];
unsigned char bufferLoc;

void SERIAL_CheckData()
{
	while (PC.readable())
	{
		LEDS[0] = !LEDS[0];
		int data = PC.getc();
		if (data == 0xFF)
		{
			//Motor Data Pack
			if (buffer[0] == 0x01)
			{
				if (bufferLoc != 7)
				{
					Error();
				}
				Direction dir1 = Forward, dir2 = Forward;
				if (buffer[1] == 0x00) { dir1 = Forward; }
				else if (buffer[1] == 0x01)	{dir1 = Backward;	}
				else	{Error();	}
				if (buffer[3] == 0x00) { dir2 = Forward; }
				else if (buffer[3] == 0x01)	{dir2 = Backward;	}
				else	{Error();	}

				unsigned int time = buffer[5] << 8 ^ buffer[6];
				PC.printf("PC#Motor#GetCommand#\n");
				MOTOR_Move(dir1, buffer[2], dir2, buffer[4], time);
				while (MOTOR_RunSign)
				{
					
				}
				PC.printf("PC#Motor#Finished#\n");

				//Clear Buffer
				bufferLoc = 0;
				for (int i = 0; i < BUFFER_SIZE; i++)
				{
					buffer[i] = 0;
				}
				return;
			}

			//MPU6050 Data Pack
			else if (buffer[0] == 0x02)
			{
				if (bufferLoc != 2)
				{
					Error();
				}
				if (buffer[1] == 0x00)					//ACC
				{
					PC.printf("#ACC#%f#%f#%f#\n", Data::xAcc, Data::yAcc, Data::zAcc);
				}
				else if (buffer[1] == 0x01)				//AngleSpeed
				{
					PC.printf("#ANGLESPEED#%f#%f#%f#\n", Data::xAngleSpeed, Data::yAngleSpeed, Data::zAngleSpeed);
				}
				else if (buffer[1] == 0x02)				//Angle
				{
					PC.printf("#ANGLE#%f#%f#%f#\n", Data::xAngle, Data::yAngle, Data::zAngle);
				}
				else if (buffer[1] == 0x03)				//Temp
				{
					PC.printf("#TEMP#%f#\n", Data::Temp);
				}
				else
				{
					Error();
				}
				//Clear Buffer
				bufferLoc = 0;
				for (int i = 0; i < BUFFER_SIZE; i++)
				{
					buffer[i] = 0;
				}
				return;
			}
			
			//Motor Turns An Angle
			else if (buffer[0] == 0x03)
			{
				if (bufferLoc!= 3)
				{
					Error();
				}
				int angle = static_cast<int>((static_cast<float>((buffer[1] << 8) | buffer[2])) / 65536 * 360) -180;
				MicroUSB.printf("Angle Now= %f  Target Angle= %i\n", Data::zAngle, angle);
				PC.printf("PC#Motor#GetCommand#\n");
				PID_Turns(angle);
				PC.printf("PC#Motor#Finished#\n");

				//Clear Buffer
				bufferLoc = 0;
				for (int i = 0; i < BUFFER_SIZE; i++)
				{
					buffer[i] = 0;
				}
				return;
			}

			//MPU6050 Z Angle Sendback
			else if (buffer[0] == 0x04)
			{
				PC.printf("PC#MPU#Angle#Z#%f.1\n", Data::zAngle);
				
				//Clear Buffer
				bufferLoc = 0;
				for (int i = 0; i < BUFFER_SIZE; i++)
				{
					buffer[i] = 0;
				}
				return;
			}

			//Other Data Pack
			else
			{
				return;
			}
		}
		else
		{
			buffer[bufferLoc] = static_cast<char>(data);
			bufferLoc++;
		}
	}
}

#pragma endregion




void Init()
{
	MPU6050_Serial.baud(9600);
	MPU6050_Serial.attach(&MPU6050_FlashData);

	PC.baud(115200);

}

int main()
{
	Init();
	while (true)
	{
		SERIAL_CheckData();
	}
}
