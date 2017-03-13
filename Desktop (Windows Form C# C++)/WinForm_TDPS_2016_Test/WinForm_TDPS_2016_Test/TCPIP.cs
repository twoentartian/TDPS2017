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
using TcpIpFileManagerSpace;
using Timer = System.Timers.Timer;

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

		private IPAddress[] localAddresses;
		private IPAddress localIpV4Address;
		private string localName;
		private Socket serverSocket;
		private Socket clientSocket;

		public Socket ClientSocket => clientSocket;

		private byte[] result = new byte[1024];

		private void Init()
		{
			localName = Dns.GetHostName();
			localAddresses = Dns.GetHostAddresses(localName);
			bool multiResult = false;
			foreach (var singleAddress in localAddresses)
			{
				if (singleAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					if (!multiResult)
					{
						multiResult = true;
						localIpV4Address = singleAddress;
					}
					else
					{
						throw new MultiIpV4AddressException();
					}
				}
			}
			serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			serverSocket.Bind(new IPEndPoint(localIpV4Address, _serverPort));  //绑定IP地址：端口  
			serverSocket.Listen(10);    //设定最多10个排队连接请求  
			StateManager tempManager = StateManager.GetInstance();
			tempManager.Udp.SetUdpClose("Init");
		}

		public void StartListen()
		{
			StateManager tempManager = StateManager.GetInstance();
			tempManager.Udp.SetUdpWaitForConnection($"Listenning {serverSocket.LocalEndPoint.ToString()}");
			Thread listenThread = new Thread(ListenClientConnect);
			listenThread.IsBackground = true;
			listenThread.Start();
		}

		/// <summary>  
		/// Wait and listen for client.
		/// </summary>  
		private void ListenClientConnect()
		{
			while (true)
			{
				StateManager tempManager = StateManager.GetInstance();
				clientSocket = serverSocket.Accept();
				tempManager.Udp.SetUdpFindClient();
				Thread receiveThread = new Thread(ReceiveMessage);
				receiveThread.IsBackground = true;
				receiveThread.Start(clientSocket);
			}
		}

		/// <summary>  
		/// Receive message
		/// </summary>  
		/// <param name="clientSocket"></param>  
		private void ReceiveMessage(object clientSocket)
		{
			Socket myClientSocket = (Socket)clientSocket;

			byte[] commandBytes = new byte[8];
			myClientSocket.Receive(commandBytes, commandBytes.Length, SocketFlags.None);
			long commandType = BitConverter.ToInt64(commandBytes, 0);

			foreach (var singleCommand in Command.CommandList)
			{
				if (singleCommand.GetId() == commandType)
				{
					singleCommand.Execute(myClientSocket);
					break;
				}
			}
			
			myClientSocket.Close();
			//Close current connection and then listen

			StateManager tempManager = StateManager.GetInstance();
			tempManager.Udp.SetUdpWaitForConnection($"Listenning {serverSocket.LocalEndPoint.ToString()}");
		}
	}

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
					byte[] bits = new byte[256];
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

		private IPAddress[] _localAddresses;
		private IPAddress _localIpV4Address;
		private IPEndPoint _localIpEndPoint;
		private IPEndPoint _remoteIpEndPoint;
		private UdpClient _localUdpClient;

		private byte[] _contentBytes;
		private Timer _broadcastTimer;

		private string _separator = "#";

		private void Init()
		{
			//Set the timer
			_broadcastTimer = new Timer();
			_broadcastTimer.Interval = 1000;
			_broadcastTimer.Elapsed += BroadcastTimerOnElapsed;

			//Set the local UDP port
			string localName = Dns.GetHostName();
			_localAddresses = Dns.GetHostAddresses(localName);
			bool multiResult = false;
			foreach (var singleAddress in _localAddresses)
			{
				if (singleAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					if (!multiResult)
					{
						multiResult = true;
						_localIpV4Address = singleAddress;
					}
					else
					{
						throw new MultiIpV4AddressException();
					}
				}
			}

			//Generate the broadcast info
			string Content = ("Server" + _separator + _localIpV4Address + _separator + Server.GetInstance().ServerPort + _separator + Environment.NewLine).ToString();
			_contentBytes = Encoding.ASCII.GetBytes(Content);
			_remoteIpEndPoint = new IPEndPoint(IPAddress.Broadcast, BroadcastPort);
			//Local broadcast
			byte[] addr = _localIpV4Address.GetAddressBytes();
			addr[3] = 255;
			_remoteIpEndPoint.Address = new IPAddress(addr);

			//Set the local UDP Client
			_localIpEndPoint = new IPEndPoint(_localIpV4Address, localPort);
			_localUdpClient = new UdpClient(_localIpEndPoint);
		}

		private void BroadcastTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			_localUdpClient.Send(_contentBytes, _contentBytes.Length, _remoteIpEndPoint);
		}

		public void StartBroadcast()
		{
			_broadcastTimer.Start();
		}

		public void StopBroadcast()
		{
			_broadcastTimer.Stop();
		}
	}
}
