using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Timers;
using WinForm_TDPS_2016_Test;

using StateManagerSpace;
using Timer = System.Timers.Timer;
using TcpIpFileManagerSpace;
using TcpUdpManagerNamespace;
using TimerManagerNamespace;

/*
 * Broadcast:
 * local port - 15000 remote port - 15878
 * InterNetwork Image Service:
 * remote port - 15879
 * 
 */

namespace WinForm_TDPS_2016_TCPIP
{
	class TcpServer
	{
		#region Config

		private int _serverPort = 15879;

		public int ServerPort => _serverPort;

		#endregion

		#region Singleton

		private static TcpServer _instance;

		protected TcpServer()
		{
			Init();
		}

		public static TcpServer GetInstance()
		{
			if (_instance == null)
			{
				_instance = new TcpServer();
			}
			return _instance;
		}

		#endregion

		private void Init()
		{
			TcpManager tempTcpManager = TcpManager.GetInstance();
			tempTcpManager.InitTcpServer(_serverPort, ServerListen);
			tempTcpManager.HostTcpListener.Server.ReceiveBufferSize = 10000000;

			StateManager tempManager = StateManager.GetInstance();
			tempManager.Udp.SetUdpClose("Init");
		}

		static void ServerListen(object client)
		{
			TcpManager.TcpClientWithGuid tcpClientWithGuid = (TcpManager.TcpClientWithGuid) client;
			int lostCounter = 0;

			while (true)
			{
				byte[] commandBytes = new byte[8];
				try
				{
					tcpClientWithGuid.TcpClient.GetStream().Read(commandBytes, 0, commandBytes.Length);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					tcpClientWithGuid.Stop();
					return;
				}

				long commandType = BitConverter.ToInt64(commandBytes, 0);
				if (commandType == 1)
				{
					lostCounter = 0;
					// Receive picture
					try
					{
						byte[] bitLen = new byte[8];
						tcpClientWithGuid.TcpClient.GetStream().Read(bitLen, 0, bitLen.Length);
						//Get the length of the file
						long contentLen = BitConverter.ToInt64(bitLen, 0);
						if (contentLen > 1000000L)
						{
							tcpClientWithGuid.Stop();
							return;
						}
						int size = 0;
						MemoryStream ms = new MemoryStream();
						//Receive the file
						while (size < contentLen)
						{
							//Receive 256 bytes for every loop
							byte[] bits = new byte[4096];
							int r = tcpClientWithGuid.TcpClient.GetStream().Read(bits, 0, bits.Length);
							if (r <= 0) break;
							ms.Write(bits, 0, r);
							size += r;
						}
						System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
						TcpIpFileManager tempFileManager = TcpIpFileManager.GetInstance();
						tempFileManager.AddTempFile(img);
						AForgeVideoSourceDevice.VideoSourceDevice.FlashTcpIpImage();
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
						tcpClientWithGuid.Stop();
						return;
					}

				}
				else if (commandType == 0)
				{
					lostCounter++;
					if (lostCounter == 100)
					{
						tcpClientWithGuid.Stop();
					}
				}
				else
				{
					// TODO:Add 
				}

			}
		}
	}

	class UdpReceiver
	{
		#region Singleton

		private UdpReceiver()
		{
			Start();
		}

		private static UdpReceiver _instance;

		public static UdpReceiver GetInstance()
		{
			return _instance ?? (_instance = new UdpReceiver());
		}

		#endregion

		#region Config

		public static int receiveImgPort = 15001;
		public const string Separator = "#";

		#endregion

		private UdpClient _udpClient;
		public UdpClient MyUdpClient => _udpClient;
		private Thread _receiveThread;
		public Thread ReceiveThread => _receiveThread;

		public void Start()
		{
			UdpManager tempUdpManager = UdpManager.GetInstance();
			_udpClient = new UdpClient(new IPEndPoint(tempUdpManager.HostIpAddress, receiveImgPort));
			_udpClient.Client.ReceiveBufferSize = 10000000;
			_receiveThread = new Thread(Receive);
			_receiveThread.IsBackground = true;
			_receiveThread.Start();
		}

		private void Receive()
		{
			int totalPackNumber = 0;
			int packCount = 0;
			MemoryStream ms = new MemoryStream();
			bool errorSign = false;
			while (true)
			{
				IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = _udpClient.Receive(ref remote);

				if (data.Length == 10)
				{
					string dataString = Encoding.ASCII.GetString(data);
					string[] separators = new[] {Separator};
					string[] items = dataString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
					if (items[0] == "Picture")
					{
						totalPackNumber = Convert.ToInt32(items[1]);
						packCount = 0;
						ms.Flush();
						ms.Position = 0;
						errorSign = false;
					}
					else
					{
						throw new LogicErrorException();
					}
				}
				else
				{
					if (errorSign)
					{
						continue;
					}
					try
					{
						ms.Write(data, 0, data.Length);
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						errorSign = true;
						continue;
					}

					packCount++;
					if (packCount == totalPackNumber)
					{
						try
						{
							System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
							TcpIpFileManager tempFileManager = TcpIpFileManager.GetInstance();
							tempFileManager.AddTempFile(img);
						}
						catch (Exception e)
						{
							Console.WriteLine(e);
							errorSign = true;
							continue;
						}
						AForgeVideoSourceDevice.VideoSourceDevice.FlashTcpIpImage();
						totalPackNumber = 0;
					}


				}


			}
		}

		public void Stop()
		{
			_receiveThread.Abort();
		}

	}

	class BroadcastService
	{
		#region Singleton

		private static BroadcastService _instance;

		protected BroadcastService()
		{
			Init();
		}

		public static BroadcastService GetInstance()
		{
			if (_instance == null)
			{
				_instance = new BroadcastService();
			}
			return _instance;
		}

		#endregion

		#region Config

		private int localPort = 15000;
		private int BroadcastPort = 15878;


		#endregion

		private string contentTcp;
		private string contentUdp;
		private Guid TimerGuid;
		public const string Separator = "#";

		public static readonly string[] Separators =
		{
			Separator
		};

		private void Init()
		{
			//Set the local UDP port
			UdpManager tempUdpManager = UdpManager.GetInstance();
			tempUdpManager.InitUdp(localPort, UdpReceiveCommand);

			//Generate the broadcast info
			contentTcp = ("Server" + Separator + tempUdpManager.HostIpAddress + Separator + TcpServer.GetInstance().ServerPort + Separator + Environment.NewLine);
			contentUdp = ("UdpIMG" + Separator + tempUdpManager.HostIpAddress + Separator + UdpReceiver.receiveImgPort + Separator + Environment.NewLine);

			//Set the timer
			TimerManager tempTimerManager = TimerManager.GetInstance();
			TimerGuid = tempTimerManager.AddTimer(BroadcastTimerOnElapsed, null, 0, 1000);
		}

		private static string udpReceiveBuffer;

		private static void UdpReceiveCommand(byte[] dataBytes)
		{
			//TODO: add function when UDP receive message.
			string data = Encoding.ASCII.GetString(dataBytes);
			udpReceiveBuffer += data;
			if (!udpReceiveBuffer.Contains("\n"))
			{
				return;
			}
			string[] items = udpReceiveBuffer.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
			if (items[0] == "Motor")
			{
				if (items[1] == "Finished")
				{
					Arduino.GetInstance().BusySign = false;
				}
				else if(items[1] == "GetCommand")
				{

				}
			}
			else
			{
				
			}

			udpReceiveBuffer = string.Empty;
		}

		private void BroadcastTimerOnElapsed(object arg)
		{
			UdpManager tempUdpManager = UdpManager.GetInstance();
			tempUdpManager.Send(IPAddress.Broadcast, BroadcastPort, contentTcp);
			tempUdpManager.Send(IPAddress.Broadcast, BroadcastPort, contentUdp);
		}

		public void BroadcastToInterNetwork(string data)
		{
			UdpManager tempUdpManager = UdpManager.GetInstance();
			tempUdpManager.Send(IPAddress.Broadcast, BroadcastPort, data);
		}

		public void StopBroadcast()
		{
			TimerManager tempTimerManager = TimerManager.GetInstance();
			tempTimerManager.StopTimer(TimerGuid);
		}



	}
}
