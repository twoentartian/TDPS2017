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
		public static int RemotePort = 15000;
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
					tempTcpManager.HostTcpClient.SendBufferSize = 10000000;
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
		private static int PackLength = 4096;

		public static TcpManager.ListenTaskDelegate TcpListenTaskDelegate = TcpListenTask;

		/// <summary>
		/// If TCP client receive data from server
		/// </summary>
		/// <param name="data"></param>
		private static void TcpListenTask(byte[] data)
		{
			throw new NotImplementedException ();
		}

		public static TimerCallback SendPictureTimerCallback = SendPictureTimerCallbackFunc;
		private static bool _sendPictureTimerCallbackFuncRunSign = false;
		private static void SendPictureTimerCallbackFunc(object state)
		{
			if (_sendPictureTimerCallbackFuncRunSign)
			{
				return;
			}
			_sendPictureTimerCallbackFuncRunSign = true;

			RaspberryCamera tempCamera = RaspberryCamera.GetInstance ();
			byte[] imgData = tempCamera.GetPictureFromStreaming ();

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
			long contentLen = imgData.LongLength;
			try
			{
				tempTcpManager.HostTcpClient.Client.Send(BitConverter.GetBytes(contentLen));
				for(long packNumber = 0; packNumber < imgData.LongLength/PackLength; packNumber++)
				{
					byte[] pack = new byte[PackLength];
					for(int i = 0; i < PackLength; i++)
					{
						pack[i] = imgData[packNumber*PackLength + i];
					}
					tempTcpManager.HostTcpClient.Client.Send(pack);
				}
				//Send last pack
				int lastPackLength = (int)(imgData.LongLength - (imgData.LongLength/PackLength) * PackLength);
				byte[] lastPack = new byte[lastPackLength];
				for (int i = 0; i < lastPackLength; i++)
				{
					lastPack[i] = imgData[imgData.LongLength - lastPackLength + i];
				}
				tempTcpManager.HostTcpClient.Client.Send(lastPack); 

			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				_sendPictureTimerCallbackFuncRunSign = false;
				tempStateManager.FindServer = false;
				return;
			}
			_sendPictureTimerCallbackFuncRunSign = false;
		}

		#endregion

	}
}
