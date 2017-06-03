#include "mbed.h"
#include "PID.h"

Serial MicroUSB(USBTX, USBRX);
Serial PC(p28, p27);
Serial MPU6050_Serial(p9, p10);
Serial HC12(p13, p14);

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

#pragma region Sonic Wave
DigitalOut trig(p11);
InterruptIn echo(p12);
Timer timerSonic;
float dis = 0;
bool freshSign = false;
bool sonicMeasureRunSign = false;

void rise(void)
{
	timerSonic.reset();
    timerSonic.start();    
} 

void fall(void)
{
	float time = timerSonic.read();
    timerSonic.stop();
    timerSonic.reset();
	if (time < 0.1)
	{
		dis = time * 34000.0f / 2;
		freshSign = true;
	}
}

void SonicWave_Init()
{
	echo.rise(&rise);
	echo.fall(&fall);
}

float SonicWave_GetDistance()
{
	sonicMeasureRunSign = true;
	while (true)
	{
		float dis1, dis2;
		while (true)
		{
			trig=1;
			wait_us(15);
			trig=0;
			wait_ms(50);
			if (freshSign)
			{
				dis1 = dis;
				freshSign = false;
				break;
			}
		}
		while (true)
		{
			trig=1;
			wait_us(15);
			trig=0;
			wait_ms(50);
			if (freshSign)
			{
				dis2 = dis;
				freshSign = false;
				break;
			}
		}
		if (abs(dis1 - dis2) < 5)
		{
			sonicMeasureRunSign = false;
			return (dis1 + dis2) / 2;
		}
	}
}
#pragma endregion

#pragma region MPU6050

static void MPU6050_FlashData()
{
	if (sonicMeasureRunSign)
	{
		while (MPU6050_Serial.readable())
		{
			MPU6050_Serial.getc();
		}
	}
	else
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
							Data::xAcc = static_cast<float>((buffer[1] << 8) | buffer[0]) / 32768 * 16 * 9.8;
							Data::yAcc = static_cast<float>((buffer[3] << 8) | buffer[2]) / 32768 * 16 * 9.8;
							Data::zAcc = static_cast<float>((buffer[5] << 8) | buffer[4]) / 32768 * 16 * 9.8;
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
							Data::xAngleSpeed = static_cast<float>((buffer[1] << 8) | buffer[0]) / 32768 * 2000;
							Data::yAngleSpeed = static_cast<float>((buffer[3] << 8) | buffer[2]) / 32768 * 2000;
							Data::zAngleSpeed = static_cast<float>((buffer[5] << 8) | buffer[4]) / 32768 * 2000;
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
}

#pragma endregion

#pragma region Turns an Angle (PID)

#define RATE_TURN 0.1f									//Wait time

PID controller_turn(6.0f, 2.0f, 0.008f, RATE_TURN);

void PID_Turns(int angle, int bias)
{
	
	if (angle > 0)
	{
		controller_turn.setInputLimits(Data::zAngle - 90, Data::zAngle + 360);
	}
	else
	{
		controller_turn.setInputLimits(Data::zAngle - 360, Data::zAngle + 90);
	}
	
	controller_turn.setOutputLimits(-1.0, 1.0);
	controller_turn.setBias(0.0);
	controller_turn.setMode(AUTO_MODE);

	float targetAngle = Data::zAngle + angle;
	controller_turn.setSetPoint(targetAngle);

	while (true)
	{
		controller_turn.setProcessValue(Data::zAngle);
		float result = controller_turn.compute();
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
		MOTOR_Move(dir1, static_cast<unsigned char>(bias + 150 * abs(result)), dir2, static_cast<unsigned char>(bias + 150 * abs(result)), RATE_TURN*1000);
		wait(RATE_TURN);
		if (abs(Data::zAngle - targetAngle) < 0.5)
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

#define RATE_STRAIGHT	0.1f

PID controller_straight(1.5f, 0.0f, 0.0f, RATE_STRAIGHT);

void PID_GoStraight(int loop)
{

	controller_straight.setInputLimits(Data::zAngle - 180, Data::zAngle + 180);
	controller_straight.setOutputLimits(-1.0, 1.0);
	controller_straight.setBias(0.0);
	controller_straight.setMode(AUTO_MODE);

	controller_straight.setSetPoint(Data::zAngle);

	while (loop--)
	{
		controller_straight.setProcessValue(Data::zAngle);
		float result = controller_straight.compute();
		Direction dir1=Forward, dir2=Forward;
		MicroUSB.printf("Angle now: %f result=%f\n", Data::zAngle, result);
		MOTOR_Move(dir1, static_cast<unsigned char>(150 - 100 * result), dir2, static_cast<unsigned char>(150 + 100 * result), RATE_STRAIGHT * 1000);
		wait(RATE_STRAIGHT);
	}
}
#pragma endregion

#pragma region Fan
DigitalOut fanPin(p20);
DigitalIn noUsePin(p21);
void FAN_Open()
{
	fanPin = 1;
	MicroUSB.printf("FAN OPEN\n");
}

void FAN_Close()
{
	fanPin = 0;
	MicroUSB.printf("FAN CLOSE\n");
}

#pragma endregion

#pragma region HC12

void HC12_Init()
{
	HC12.baud(9600);
	HC12.format(8, SerialBase::None, 1);
}

void HC12_Send(char data)
{
	LEDS[0] = !LEDS[0];
	HC12.printf("%c", data);
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
//			|->	Sonic Dis Measurement	0x05																															0xFF (1 byte)
//						|->								Start Sign																													End SIgn
//
//			|->	Motor		0x06					Go straight for a distance	with given loop(1 byte)													0xFF (1 byte)
//						|->		Start Sign			0x03																															End Sign
//
//			|->	Fan			0x07					DataSign (1 byte) Zero fpr close, none zero for open												0xFF (1 byte)
//						|->		Start Sign																																			End Sign
//
//			|->	HC12		0x08					Data (4 byte)																											0xFF (1 byte)
//						|->		Start Sign			Send this data packet to HC 12																				End Sign
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
				int angle = static_cast<int>((static_cast<float>((buffer[1] << 8) | buffer[2])) / 65536 * 360 - 180);
				MicroUSB.printf("Angle Now= %f  Target Angle= %i\n", Data::zAngle, angle);
				PC.printf("PC#Motor#GetCommand#\n");
				PID_Turns(angle, 0);
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

			//Get Sonic Wave Distance Measure Result
			else if (buffer[0] == 0x05)
			{
				float dis = SonicWave_GetDistance();
				PC.printf("PC#DISTANCE#%f\n", dis);

				//Clear Buffer
				bufferLoc = 0;
				for (int i = 0; i < BUFFER_SIZE; i++)
				{
					buffer[i] = 0;
				}
				return;
			}

			//Go straight for a distance with given loop.
			else if (buffer[0] == 0x06)
			{
				int loop = static_cast<int>((buffer[1] << 8) | buffer[2]);

				PC.printf("PC#Motor#GetCommand#\n");
				PID_GoStraight(loop);
				PC.printf("PC#Motor#Finished#\n");

				//Clear Buffer
				bufferLoc = 0;
				for (int i = 0; i < BUFFER_SIZE; i++)
				{
					buffer[i] = 0;
				}
				return;
			}

			//Fan control
			else if (buffer[0] == 0x07)
			{
				if (buffer[1] == 0)
				{
					FAN_Close();
				}
				else
				{
					FAN_Open();
				}

				//Clear Buffer
				bufferLoc = 0;
				for (int i = 0; i < BUFFER_SIZE; i++)
				{
					buffer[i] = 0;
				}
				return;
			}

			//HC12
			else if (buffer[0] == 0x08)
			{
				HC12_Send(buffer[1]);
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

	HC12_Init();

	PC.baud(115200);

	SonicWave_Init();
}

int main()
{
	Init();
	while (true)
	{
		SERIAL_CheckData();
	}
}
