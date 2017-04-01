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
using RaspberryCam;

namespace Cs_Mono_RaspberryPi
{
	partial class Program
	{


		#region Ini

		public static readonly string[] SeparatorStrings = new[] { "#" };

		public static readonly int LocalUdpPort = 15878;

		public static readonly int BaudRate = 115200;

		public static readonly PictureSize pictureSize = new PictureSize (1280, 720);

		public static readonly int fps = 10;

		#endregion



		static void Main(string[] args)
		{
			#region Camera
			RaspberryCamera tempCamera = RaspberryCamera.GetInstance ();
			tempCamera.StartStreaming (Program.pictureSize, Program.fps);
			Console.WriteLine("Init Camera --- success");
			#endregion

			#region Init UDP manager

			Console.Write("Init UDP manager");
			UdpManager tempUdpManager = UdpManager.GetInstance();
			tempUdpManager.InitUdp(LocalUdpPort, InterNetwork.UdpListenTaskDelegate);
			Console.WriteLine(" --- success");

			#endregion

			#region Init Serial Manager

			Console.Write("Init SERIAL manager ");
			SerialManager tempSerialManager = SerialManager.GetInstance();
			while (true)
			{
				string[] ports = SerialManager.GetAllVaildPorts();
				bool signFind = false;
				foreach(string i in ports)
				{
					if(i.Contains("USB"))
					{
						RaspberrySerial.PortGuid = tempSerialManager.Add(i, BaudRate, Parity.None, RaspberrySerial.TargetDataReceivedEventHandler).Guid;
						Console.Write("Select" + i);
						signFind = true;
						break;
					}
				}
				if (!signFind)
				{
					Console.WriteLine("The system is waiting for a serial port device");
					Thread.Sleep(1000);
				}
				else
				{
					break;
				}
			}
			Console.WriteLine(" --- success");

			#endregion
			Console.ReadLine();
			tempCamera.StopStreaming ();
			StateManager.GetInstance ().FindServer = false;
		}
	}
}
