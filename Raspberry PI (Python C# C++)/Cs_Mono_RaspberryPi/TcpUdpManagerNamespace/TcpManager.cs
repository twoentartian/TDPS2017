using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpUdpManagerNamespace
{
	public sealed class TcpManager
	{
		#region Singleton

		private static TcpManager _instance;

		private TcpManager()
		{
			InitHost();
		}

		public static TcpManager GetInstance()
		{
			return _instance ?? (_instance = new TcpManager());
		}

		#endregion

		private void Print(string info)
		{
			Console.WriteLine("UDP Manager: " + info);
		}

		#region Property

		public delegate void ListenTaskDelegate(byte[] argBytes);

		private string _hostName;
		public string HostName => _hostName;

		private IPAddress _hostIpAddress;
		public IPAddress HostIpAddress => _hostIpAddress;

		private IPEndPoint _hostClientEndPoint;
		public IPEndPoint HostClientEndPoint => _hostClientEndPoint;

		private IPEndPoint _hostServerEndPoint;
		public IPEndPoint HostServerEndPoint => _hostServerEndPoint;

		private TcpListener _hostTcpServer;
		public TcpListener HostTcpListener => _hostTcpServer;

		private TcpClient _hostTcpClient;
		public TcpClient HostTcpClient => _hostTcpClient;

		private ListenTaskDelegate _tcpServerReceiveDelegate;
		public ListenTaskDelegate TcpServerReceiveDelegate => _tcpServerReceiveDelegate;

		private ListenTaskDelegate _tcpClientReceiveDelegate;
		public ListenTaskDelegate TcpClientReceiveDelegate => _tcpClientReceiveDelegate;

		#endregion

		/// <summary>
		/// Acquire local network information.
		/// </summary>
		public void InitHost()
		{
			_hostName = null;
			_hostIpAddress = null;

			_hostName = Dns.GetHostName();
			IPAddress[] allAddresses = Dns.GetHostAddresses(_hostName);
			IEnumerable<IPAddress> ipV4AddressList = from singleAddresses in allAddresses where (singleAddresses.AddressFamily == AddressFamily.InterNetwork) select singleAddresses;
			IPAddress[] ipV4AddressArray = ipV4AddressList.ToArray();
			if (ipV4AddressArray.Length == 0)
			{
				throw new NoVaildIpV4AddressException("No Vaild InterNetwork V4 Address");
			}
			else if (ipV4AddressArray.Length == 1)
			{
				_hostIpAddress = ipV4AddressArray[0];
			}
			else
			{
				throw new MultiIpV4AddressException("More Than One Vaild InterNetwork V4 Address");
			}
		}

		#region TcpServer

		private const int MaxClient = 10;
		private readonly TcpClientWithGuid[] _tcpClientArray = new TcpClientWithGuid[MaxClient];

		private class TcpClientWithGuid
		{
			public Thread ReceiveThread = null;
			public TcpClient TcpClient = null;
			public NetworkStream TcpStream = null;
			public Guid Guid = System.Guid.Empty;
			public bool SignOccupied = false;

			/// <summary>
			/// Stop the Tcp client and kill the receive thread.
			/// </summary>
			public void Stop()
			{
				TcpClient.Close();
				TcpClient = null;
				Guid = Guid.Empty;
				SignOccupied = false;
				if (ReceiveThread != null)
				{
					if (ReceiveThread.IsAlive)
					{
						ReceiveThread.Abort();
					}
				}
			}
		}

		/// <summary>
		/// Init TCP server, start listen task on argPort at the same time.
		/// </summary>
		/// <param name="argPort"></param>
		/// <param name="argListenDelegate"></param>
		public void InitTcpServer(int argPort, ListenTaskDelegate argListenDelegate)
		{
			if (_hostName == null || _hostIpAddress == null)
			{
				InitHost();
			}
			_hostServerEndPoint = null;
			_hostTcpServer = null;
			_hostServerEndPoint = new IPEndPoint(_hostIpAddress, argPort);
			_hostTcpServer = new TcpListener(_hostServerEndPoint);
			_tcpServerReceiveDelegate = argListenDelegate;

			_hostTcpServer.Start(MaxClient);
			TcpServerStartListenTask();
		}

		/// <summary>
		/// Start listen service on server
		/// </summary>
		private void TcpServerStartListenTask()
		{
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
				bool enoughClientSign = false;
				for (int i = 0; i < _tcpClientArray.Length; i++)
				{
					if (_tcpClientArray[i] != null)
					{
						if (_tcpClientArray[i].SignOccupied != false)
						{
							continue;
						}
					}
					enoughClientSign = true;
					TcpClient tempTcpClient = _hostTcpServer.AcceptTcpClient();
					_tcpClientArray[i] = new TcpClientWithGuid();
					try
					{
						_tcpClientArray[i].Guid = Guid.NewGuid();
						_tcpClientArray[i].TcpClient = tempTcpClient;
						_tcpClientArray[i].ReceiveThread = new Thread(TcpServerReceiveMessage) { IsBackground = true };
						_tcpClientArray[i].ReceiveThread.Start(_tcpClientArray[i]);
						_tcpClientArray[i].SignOccupied = true;
						_tcpClientArray[i].TcpStream = tempTcpClient.GetStream();
					}
					catch (Exception)
					{
						throw;
					}
					break;
				}
				if (!enoughClientSign)
				{
					throw new NotEnoughTcpServerCapacityException("The capacity of the server is set to be " + MaxClient);
				}
				TcpServerRebuildClientList();
			}
		}

		/// <summary>
		/// Mark unoccupied client as null and run GC.
		/// </summary>
		public void TcpServerRebuildClientList()
		{
			for (int i = 0; i < _tcpClientArray.Length; i++)
			{
				if (_tcpClientArray[i] == null)
				{
					continue;
				}
				if (_tcpClientArray[i].SignOccupied == false)
				{
					_tcpClientArray[i] = null;
				}
			}
			GC.Collect();
		}

		/// <summary>  
		/// Receive message
		/// </summary>  
		/// <param name="argClientWithGuid"></param>  
		private void TcpServerReceiveMessage(object argClientWithGuid)
		{
			TcpClientWithGuid tcpClientWithGuid = (TcpClientWithGuid)argClientWithGuid;
			StreamReader sr = new StreamReader(tcpClientWithGuid.TcpStream);
			while (true)
			{
				string result = sr.ReadLine();
				if (result == null)
				{
					sr.Dispose();
					tcpClientWithGuid.Stop();
				}
				else
				{
					//TODO: Write the message execute code
					_tcpServerReceiveDelegate(Encoding.ASCII.GetBytes(result));
				}
			}
		}

		/// <summary>
		/// Get the remote TCP client by port number
		/// </summary>
		/// <param name="argPort"></param>
		/// <returns></returns>
		private TcpClientWithGuid GetRemoteTcpClient(int argPort)
		{
			foreach (TcpClientWithGuid client in _tcpClientArray)
			{
				if (client == null)
				{
					continue;
				}
				if (client.SignOccupied == false)
				{
					continue;
				}
				IPEndPoint tempIpEndPoint = (IPEndPoint)client.TcpClient.Client.RemoteEndPoint;
				if (tempIpEndPoint.Port == argPort)
				{
					return client;
				}
			}
			throw new ArgumentException("Cannot find vaild client with port number: " + argPort);
		}

		/// <summary>
		/// Get the remote TCP client by IP address
		/// </summary>
		/// <param name="argAddress"></param>
		/// <returns></returns>
		private TcpClientWithGuid GetRemoteTcpClient(IPAddress argAddress)
		{
			foreach (TcpClientWithGuid client in _tcpClientArray)
			{
				if (client == null)
				{
					continue;
				}
				if (client.SignOccupied == false)
				{
					continue;
				}
				IPEndPoint tempIpEndPoint = (IPEndPoint)client.TcpClient.Client.RemoteEndPoint;
				if (tempIpEndPoint.Address.Equals(argAddress))
				{
					return client;
				}
			}
			throw new ArgumentException("Cannot find vaild client with IP address: " + argAddress);
		}

		/// <summary>
		/// Get the remote TCP client by GUID (recommanded)
		/// </summary>
		/// <param name="argGuid"></param>
		/// <returns></returns>
		private TcpClientWithGuid GetRemoteTcpClient(Guid argGuid)
		{
			foreach (TcpClientWithGuid client in _tcpClientArray)
			{
				if (client == null)
				{
					continue;
				}
				if (client.SignOccupied == false)
				{
					continue;
				}
				if (client.Guid == argGuid)
				{
					return client;
				}
			}
			throw new ArgumentException("Cannot find vaild client with GUID: " + argGuid);
		}

		/// <summary>
		/// Close a socket by remote port number
		/// </summary>
		/// <param name="argPort"></param>
		public void TcpServerStopClient(int argPort)
		{
			GetRemoteTcpClient(argPort).Stop();
		}

		/// <summary>
		/// Close a socket by remote IP address
		/// </summary>
		/// <param name="argAddress"></param>
		public void TcpServerStopClient(IPAddress argAddress)
		{
			GetRemoteTcpClient(argAddress).Stop();
		}

		/// <summary>
		/// Close a socket by GUID (recommanded)
		/// </summary>
		/// <param name="argGuid"></param>
		public void TcpServerStopClient(Guid argGuid)
		{
			GetRemoteTcpClient(argGuid).Stop();
		}

		/// <summary>
		/// Use TCP server to send message to a remote client by port number
		/// </summary>
		/// <param name="argPort"></param>
		/// <param name="message"></param>
		public void TcpServerSend(int argPort, string message)
		{
			TcpClientWithGuid client = GetRemoteTcpClient(argPort);
			byte[] messageBytes = Encoding.ASCII.GetBytes(message);
			client.TcpStream.Write(messageBytes, 0, messageBytes.Length);
		}

		/// <summary>
		/// Use TCP server to send message to a remote client by IP address
		/// </summary>
		/// <param name="argAddress"></param>
		/// <param name="message"></param>
		public void TcpServerSend(IPAddress argAddress, string message)
		{
			TcpClientWithGuid client = GetRemoteTcpClient(argAddress);
			byte[] messageBytes = Encoding.ASCII.GetBytes(message);
			client.TcpStream.Write(messageBytes, 0, messageBytes.Length);
		}

		/// <summary>
		/// Use TCP server to send message to a remote client by GUID (recommanded)
		/// </summary>
		/// <param name="argGuid"></param>
		/// <param name="message"></param>
		public void TcpServerSend(Guid argGuid, string message)
		{
			TcpClientWithGuid client = GetRemoteTcpClient(argGuid);
			byte[] messageBytes = Encoding.ASCII.GetBytes(message);
			client.TcpStream.Write(messageBytes, 0, messageBytes.Length);
		}

		/// <summary>
		/// Use TCP server to send message to a remote client by port number
		/// </summary>
		/// <param name="argPort"></param>
		/// <param name="message"></param>
		public void TcpServerSend(int argPort, byte[] message)
		{
			TcpClientWithGuid client = GetRemoteTcpClient(argPort);
			client.TcpStream.Write(message, 0, message.Length);
		}

		/// <summary>
		/// Use TCP server to send message to a remote client by IP address
		/// </summary>
		/// <param name="argAddress"></param>
		/// <param name="message"></param>
		public void TcpServerSend(IPAddress argAddress, byte[] message)
		{
			TcpClientWithGuid client = GetRemoteTcpClient(argAddress);
			client.TcpStream.Write(message, 0, message.Length);
		}

		/// <summary>
		/// Use TCP server to send message to a remote client by GUID (recommanded)
		/// </summary>
		/// <param name="argGuid"></param>
		/// <param name="message"></param>
		public void TcpServerSend(Guid argGuid, byte[] message)
		{
			TcpClientWithGuid client = GetRemoteTcpClient(argGuid);
			client.TcpStream.Write(message, 0, message.Length);
		}

		/// <summary>
		/// Close all remote client sockets
		/// </summary>
		public void TcpServerStopAllClient()
		{
			foreach (TcpClientWithGuid client in _tcpClientArray)
			{
				if (client == null)
				{
					continue;
				}
				if (client.SignOccupied)
				{
					client.Stop();
				}
			}
		}

		/// <summary>
		/// Get the GUID by port.
		/// </summary>
		/// <param name="argPort"></param>
		/// <returns></returns>
		public Guid TcpServerGetGuid(int argPort)
		{
			return GetRemoteTcpClient(argPort).Guid;
		}

		/// <summary>
		/// Get the GUID by IP address
		/// </summary>
		/// <param name="argAddress"></param>
		/// <returns></returns>
		public Guid TcpServerGetGuid(IPAddress argAddress)
		{
			return GetRemoteTcpClient(argAddress).Guid;
		}

		#endregion

		#region TcpClient

		/// <summary>
		/// Init TCP client and connect to a server, occupy a port at the same time.
		/// </summary>
		/// <param name="argRemoteIpEndPoint"></param>
		/// <param name="argListenDelegate"></param>
		/// <param name="argPort"></param>
		public void InitTcpClient(IPEndPoint argRemoteIpEndPoint, ListenTaskDelegate argListenDelegate, int argPort)
		{
			if (_hostName == null || _hostIpAddress == null)
			{
				InitHost();
			}
			_hostClientEndPoint = null;
			_hostClientEndPoint = new IPEndPoint(_hostIpAddress, argPort);
			_hostTcpClient = null;
			_hostTcpClient = new TcpClient(_hostClientEndPoint);

			_hostTcpClient.Connect(argRemoteIpEndPoint);

			_tcpClientReceiveDelegate = argListenDelegate;
			TcpClientStartListenTask();
		}

		/// <summary>
		/// Init TCP client, this method use a random port.
		/// </summary>
		/// <param name="argRemoteIpEndPoint"></param>
		/// <param name="argListenDelegate"></param>
		public void InitTcpClient(IPEndPoint argRemoteIpEndPoint, ListenTaskDelegate argListenDelegate)
		{
			Random randomGenerator = new Random();
			while (true)
			{
				int port = randomGenerator.Next(10000, 65535);
				try
				{
					InitTcpClient(argRemoteIpEndPoint, argListenDelegate, port);
					break;
				}
				catch (SocketException e)
				{
					if (e.SocketErrorCode == SocketError.ConnectionRefused)
					{
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Start client listen service.
		/// </summary>
		private void TcpClientStartListenTask()
		{
			Thread receiveThread = new Thread(TcpClientReceiveMessage) {IsBackground = true};
			receiveThread.Start();
		}

		/// <summary>
		/// The thread method for TcpClientStartListenTask
		/// </summary>
		private void TcpClientReceiveMessage()
		{
			NetworkStream ns = new NetworkStream(_hostTcpClient.Client);
			StreamReader sr = new StreamReader(ns);
			while (true)
			{
				string message;
				try
				{
					message = sr.ReadLine();
				}
				catch (Exception)
				{
					break;
				}
				if (message == null)
				{
					break;
				}

				//TODO: Add delegate
				_tcpClientReceiveDelegate(Encoding.ASCII.GetBytes(message));
			}
			ns.Dispose();
		}

		/// <summary>
		/// Send a message to remote TCP server.
		/// </summary>
		/// <param name="message"></param>
		public void TcpClientSend(string message)
		{
			byte[] messageByte = Encoding.ASCII.GetBytes(message);
			_hostTcpClient.Client.Send(messageByte);
		}

		/// <summary>
		/// Once a client is closed, you must use InitTcpClient() to build a new one.
		/// </summary>
		public void TcpClientClose()
		{
			if (_hostTcpClient.Connected)
			{
				_hostTcpClient.Close();
			}
		}

		#endregion

	}
}
