#include "Error.h"

void Error::Stop()
{
	pinMode(ERRORPIN, OUTPUT);
	bool sign = false;
	while (true)
	{
		digitalWrite(ERRORPIN, sign);
		sign = !sign;
		delay(1000);
	}
}
