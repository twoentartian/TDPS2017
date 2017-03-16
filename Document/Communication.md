# TDPS2017
Team Design Project (for Meepo Uestc, Uog)

##Arduino - Raspberry pi serial port command interpretation
* Start sign - 1 byte - the byte indicating the function of the following data pack.
* Data pack - 1~n byte -  notice: no 0x00 in data pack.
* End sign - 1 byte -  it should always be 0x00.

Example: 0x01(start motor sign) 0x01 0xff 0x01 0xff(data pack: dir pwm dir pwm) 0x00(end sign)  
Totally 6 bytes.

In Arduino client, you must write a class inheriting from CommandResult class when interpreting the received pack. Which is as follow:  

	class CommandResult  
	{  
	public:  
		enum ResultState  
		{  
			MotorResultState = 0  
		};  
		CommandResult(bool argIsData);  
		~CommandResult();  
		ResultState CurrentResult;  
		bool isData;  
	};  

	class MotorResult : public CommandResult  
	{  
	public:  
		MotorResult(Direction argMotorDirection1, byte argMotorSpeed1, Direction argMotorDirection2, byte argMotorSpeed2);  
		int motorSpeed1;  
		int motorSpeed2;  
		Direction motorDirection1;  
		Direction motorDirection2;  
	};  

##Raspberry pi 