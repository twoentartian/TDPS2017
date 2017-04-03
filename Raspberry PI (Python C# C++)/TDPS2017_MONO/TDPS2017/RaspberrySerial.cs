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
					UdpManager tempUdpManager = UdpManager.GetInstance ();
					for (int i = 1; i < items.Length; i ++)
					{
						tempUdpManager.Send (IPAddress.Broadcast, InterNetwork.RemotePort, items [i]);
						tempUdpManager.Send (IPAddress.Broadcast, InterNetwork.RemotePort, seperators[0]);
					}
					tempUdpManager.Send (IPAddress.Broadcast, InterNetwork.RemotePort, Environment.NewLine);
				}
				else
				{
					throw new NotImplementedException();
				}
			}

		}
	}
}
