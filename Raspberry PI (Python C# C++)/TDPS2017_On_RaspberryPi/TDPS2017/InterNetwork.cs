using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SerialPortNamespace;
using TcpUdpManagerNamespace;
using TimerManagerNamespace;

namespace Cs_Mono_RaspberryPi
{
	class InterNetwork
	{
		#region UDP

		public static UdpManager.ListenTaskDelegate UdpListenTaskDelegate = UdpListenTask;

		/// <summary>
		/// If UDP socket receive data from remote. (server and motor information)
		/// </summary>
		/// <param name="data"></param>
		private static void UdpListenTask(byte[] data)
		{
			string dataString = Encoding.ASCII.GetString(data);
			string[] itemStrings = dataString.Split(Program.SeparatorStrings, StringSplitOptions.RemoveEmptyEntries);

			if (itemStrings[0] == "Server")
			{
				StateManager tempStateManager = StateManager.GetInstance();
				if (!tempStateManager.FindServer)
				{
					TcpManager tempTcpManager = TcpManager.GetInstance();
					tempTcpManager.InitTcpClient(new IPEndPoint(IPAddress.Parse(itemStrings[1]), Convert.ToInt32(itemStrings[2])), TcpListenTaskDelegate);
					tempStateManager.FindServer = true;
				}
			}
			else if (itemStrings[0] == "Motor")
			{
				SerialManager tempSerialManager = SerialManager.GetInstance();
				SerialManager.SerialPortWithGuid port = tempSerialManager.GetPort(RaspberrySerial.PortGuid);

				byte[] serialBytes = new byte[8];
				serialBytes[0] = 0x01;
				for (int i = 1; i < 5; i++)
				{
					serialBytes[i] = Convert.ToByte(itemStrings[i]);
				}
				int time = Convert.ToInt32(itemStrings[5]);
				serialBytes[5] = (byte)(time / 256);
				serialBytes[6] = (byte)(time % 256);
				serialBytes[7] = 0xFF;
				port.Send(serialBytes);
			}
			else
			{
				throw new LogicErrorException("Not implementate");
			}
		}

		#endregion

		#region TCP

		public static TcpManager.ListenTaskDelegate TcpListenTaskDelegate = TcpListenTask;

		/// <summary>
		/// If TCP client receive data from server
		/// </summary>
		/// <param name="data"></param>
		private static void TcpListenTask(byte[] data)
		{

		}

		public static TimerCallback SendPictureTimerCallback = SendPictureTimerCallbackFunc;
		private static bool _sendPictureTimerCallbackFuncRunSign = false;
		//TODO: delete temp in future
		private static bool temp = false;
		private static void SendPictureTimerCallbackFunc(object state)
		{
			if (_sendPictureTimerCallbackFuncRunSign)
			{
				return;
			}
			_sendPictureTimerCallbackFuncRunSign = true;
			//TODO: add camera function to get the picture path
			string path;
			if (temp)
			{
				path = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "1.jpg";
			}
			else
			{
				path = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "2.jpg";
			}
			temp = !temp;


			StateManager tempStateManager = StateManager.GetInstance();
			TcpManager tempTcpManager = TcpManager.GetInstance();
			try
			{
				tempTcpManager.HostTcpClient.Client.Send(BitConverter.GetBytes(1L));
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				_sendPictureTimerCallbackFuncRunSign = false;
				tempStateManager.FindServer = false;
				return;
			}
			FileStream fs = new FileStream(path, FileMode.Open);
			long contentLen = fs.Length;
			try
			{
				tempTcpManager.HostTcpClient.Client.Send(BitConverter.GetBytes(contentLen));
				while (true)
				{
					byte[] bits = new byte[256];
					int r = fs.Read(bits, 0, bits.Length);
					if (r <= 0) break;
					tempTcpManager.HostTcpClient.Client.Send(bits, r, SocketFlags.None);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				_sendPictureTimerCallbackFuncRunSign = false;
				tempStateManager.FindServer = false;
				fs.Flush();
				fs.Dispose();
				return;
			}
			fs.Flush();
			fs.Dispose();
			_sendPictureTimerCallbackFuncRunSign = false;
		}

		#endregion

	}
}
