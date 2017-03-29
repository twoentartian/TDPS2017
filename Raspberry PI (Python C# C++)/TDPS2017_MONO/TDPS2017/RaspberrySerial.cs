using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs_Mono_RaspberryPi
{
	class RaspberrySerial
	{
		public static Guid PortGuid;

		public static SerialDataReceivedEventHandler TargetDataReceivedEventHandler = TargetDataReceivedEventHandlerFunc;

		private static void TargetDataReceivedEventHandlerFunc(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
		{
			SerialPort port = (SerialPort) sender;
			string data = port.ReadExisting();
			//TODO: handle the data received from serial port.
		}
	}
}
