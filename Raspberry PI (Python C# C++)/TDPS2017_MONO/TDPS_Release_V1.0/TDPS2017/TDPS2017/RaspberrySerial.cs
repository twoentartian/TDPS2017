using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using TimerManagerNamespace;
using TcpUdpManagerNamespace;

namespace Cs_Mono_RaspberryPi
{
	class RaspberrySerial
	{
		public static Guid PortGuid;

		private static string[] seperators = { "#" };

		public static SerialDataReceivedEventHandler TargetDataReceivedEventHandler = TargetDataReceivedEventHandlerFunc;

		private static void TargetDataReceivedEventHandlerFunc(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
		{

		}

		private	static Thread receiveThread;
		public static void StartReceive(SerialPort argPort)
		{
			receiveThread = new Thread (ReceiveFunc);
			receiveThread.IsBackground = true;
			receiveThread.Start (argPort);
		}

		public static void ReceiveFunc(object arg)
		{
			SerialPort port = (SerialPort) arg;
			while (true)
			{
				string data = port.ReadLine ();
				//TODO: handle the data received from serial port.
				string[] items = data.Split (seperators, StringSplitOptions.RemoveEmptyEntries);
				if(items[0] == "PC")
				{
					string dataToPc = string.Empty;
					for (int i =1; i < items.Length; i++)
					{
						if (!items [i].Contains ("\r"))
						{
							dataToPc += items [i] + seperators [0];
						}
					}
					TcpManager.GetInstance ().TcpClientSend (dataToPc + Environment.NewLine);
				}
				else
				{
					throw new NotImplementedException();
				}
			}

		}
	}
}
