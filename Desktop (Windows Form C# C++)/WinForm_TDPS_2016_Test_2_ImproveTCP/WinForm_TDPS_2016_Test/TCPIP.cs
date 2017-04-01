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
	class Server
	{
		#region Config

		private int _serverPort = 15879;

		public int ServerPort => _serverPort;

		#endregion

		#region Singleton

		private static Server _instance;

		protected Server()
		{
			Init();
		}

		public static Server GetInstance()
		{
			if (_instance == null)
			{
				_instance = new Server();
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

	/*
		class Command
		{
			protected static List<Command> _commandList = new List<Command>(5);
	
			public static List<Command> CommandList
			{
				get
				{
					if (_commandList.Count == 0)
					{
						InitList();
					}
					return _commandList;
				}
			}
	
			protected Command()
			{
				
			}
	
			protected static void InitList()
			{
				_commandList.Add(new GetPictureCommand());
				_commandList.Add(new EchoCommand());
			}
	
			public virtual void Execute(Socket clientSocket)
			{
				
			}
	
			public virtual long GetId()
			{
				return 0;
			}
		}
	
		class GetPictureCommand : Command
		{
			public override long GetId()
			{
				return 1;
			}
	
			public override void Execute(Socket clientSocket)
			{
				try
				{
					byte[] bitLen = new byte[8];
					clientSocket.Receive(bitLen, bitLen.Length, SocketFlags.None);
					//Get the length of the file
					long contentLen = BitConverter.ToInt64(bitLen, 0);
					int size = 0;
					MemoryStream ms = new MemoryStream();
					//Receive the file
					while (size < contentLen)
					{
						//Receive 256 bytes for every loop
						byte[] bits = new byte[4096];
						int r = clientSocket.Receive(bits, bits.Length, SocketFlags.None);
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
					Console.WriteLine(ex.Message);
					clientSocket.Shutdown(SocketShutdown.Both);
					clientSocket.Close();
					return;
				}
	
	
			}
		}
	
		class EchoCommand : Command
		{
			public override long GetId()
			{
				return 2;
			}
	
			public override void Execute(Socket clientSocket)
			{
				Server localServer = Server.GetInstance();
				localServer.ClientSocket.Send(Encoding.ASCII.GetBytes("Echo from server" + Environment.NewLine));
			}
		}
		*/

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

		private string content;
		private Guid TimerGuid;
		public const string Separator = "#";

		private void Init()
		{
			//Set the local UDP port
			UdpManager tempUdpManager = UdpManager.GetInstance();
			tempUdpManager.InitUdp(localPort, UdpReceive);

			//Generate the broadcast info
			content = ("Server" + Separator + tempUdpManager.HostIpAddress + Separator + Server.GetInstance().ServerPort + Separator + Environment.NewLine);

			//Set the timer
			TimerManager tempTimerManager = TimerManager.GetInstance();
			TimerGuid = tempTimerManager.AddTimer(BroadcastTimerOnElapsed, null, 0, 1000);
		}

		private static void UdpReceive(byte[] dataBytes)
		{
			//TODO: add function when UDP receive message.
			throw new NotImplementedException();
		}

		private void BroadcastTimerOnElapsed(object arg)
		{
			UdpManager tempUdpManager = UdpManager.GetInstance();
			tempUdpManager.Send(IPAddress.Broadcast, BroadcastPort, content);
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
