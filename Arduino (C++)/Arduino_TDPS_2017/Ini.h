#pragma once
#ifndef INI_H
#define INI_H

#define DEBUG_SERIAL
#define BAUD_SERIAL		115200

//motor A connected between A01 and A02
//motor B connected between B01 and B02

#define STBY		6		//Stand by control

//Motor A
#define PWMA		9		//Speed control
#define AIN1		8		//Direction
#define AIN2		7		//Direction

//Motor B
#define PWMB		10		//Speed control
#define BIN1		11		//Direction
#define BIN2		12		//Direction


//Serial Port
#define RX			0
#define TX			1


//Error Repoter
#define ERRORPIN		13

//Buffer Size
#define BUFFER_SIZE	10

enum Direction
{
	Forward = 1, Backward = 2
};

#endif // !INI_H
