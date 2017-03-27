using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SerialPortNamespace;
using TcpUdpManagerNamespace;
using TimerManagerNamespace;

namespace Cs_Mono_RaspberryPi
{
	partial class Program
	{


		#region Ini

		public static readonly string[] SeparatorStrings = new[] { "#" };

		public static readonly int LocalUdpPort = 15878;

		public static readonly int BaudRate = 115200;

		#endregion



		static void Main(string[] args)
		{
			#region Init UDP manager

			Console.Write("Init UDP manager");
			UdpManager tempUdpManager = UdpManager.GetInstance();
			tempUdpManager.InitUdp(LocalUdpPort, InterNetwork.UdpListenTaskDelegate);
			Console.WriteLine(" --- success");

			#endregion

			#region Init Serial Manager

			Console.Write("Init SERIAL manager");
			SerialManager tempSerialManager = SerialManager.GetInstance();
			while (true)
			{
				string[] ports = SerialManager.GetAllVaildPorts();
				foreach(string i in ports)
				{
					Console.WriteLine(i);
				}


				if (ports.Length == 0)
				{
					Console.WriteLine("The system is waiting for a serial port device");
					Thread.Sleep(1000);
				}
				else
				{
					RaspberrySerial.PortGuid = tempSerialManager.Add(ports[0], BaudRate, Parity.None, RaspberrySerial.TargetDataReceivedEventHandler).Guid;
					break;
				}
			}
			Console.WriteLine(" --- success");

			#endregion






			Console.ReadLine();
		}
	}
}
