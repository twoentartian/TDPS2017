using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpUdpManagerNamespace
{
	public sealed class UdpManager
	{
		#region Singleton

		private static UdpManager _instance;

		private UdpManager()
		{
			InitHost();
		}

		public static UdpManager GetInstance()
		{
			return _instance ?? (_instance = new UdpManager());
		}

		#endregion

		#region Property

		private string _hostName;
		//public string HostName => _hostName;

		private IPAddress _hostIpAddress;
		//public IPAddress HostIpAddress => _hostIpAddress;
		public IPAddress HostIpAddress
		{
			get
			{
				return _hostIpAddress;
			}
		}

		private IPEndPoint _hostIpEndPoint;
		//public IPEndPoint HostIpEndPoint => _hostIpEndPoint;
		public IPEndPoint HostIpEndPoint
		{
			get
			{
				return _hostIpEndPoint;
			}
		}

		private UdpClient _hostUdpClient;
		//public UdpClient HostUdpClient => _hostUdpClient;
		public UdpClient HostUdpClient
		{
			get
			{
				return _hostUdpClient;
			}
		}

		private ListenTaskDelegate _receiveDelegate;
		//public ListenTaskDelegate ReceiveDelegate => _receiveDelegate;
		public ListenTaskDelegate ReceiveDelegate
		{
			get
			{
				return _receiveDelegate;
			}
		}

		#endregion

		public void InitHost()
		{
			_hostName = null;
			_hostIpAddress = null;
			_hostIpEndPoint = null;
			_hostUdpClient = null;

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

		/// <summary>
		/// Init local UDP socket
		/// </summary>
		/// <param name="argPort"></param>
		/// <param name="argListenTask"></param>
		public void InitUdp(int argPort, ListenTaskDelegate argListenTask)
		{
			if (_hostName == null || _hostIpAddress == null)
			{
				InitHost();
			}

			_hostIpEndPoint = new IPEndPoint(IPAddress.Any, argPort);
			_hostUdpClient = new UdpClient(_hostIpEndPoint);
			_hostUdpClient.EnableBroadcast = true;
			byte[] tempData = Encoding.ASCII.GetBytes ("TSET");
			_hostUdpClient.Send (tempData, tempData.Length, new IPEndPoint (IPAddress.Broadcast, 15000));
			_receiveDelegate = argListenTask;

			//Add listen thread
			Thread listenThread = new Thread(Listen) {IsBackground = true};
			listenThread.Start();
		}

		/// <summary>
		/// Occurs when receive data from UDP socket.
		/// </summary>
		/// <param name="data"></param>
		public delegate void ListenTaskDelegate(byte[] data);

		/// <summary>
		/// Listen thread func
		/// </summary>
		private void Listen()
		{
			while (true)
			{
				IPEndPoint remotEndPoint = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = _hostUdpClient.Receive(ref remotEndPoint);

				//TODO: add code for listening
				try
				{
					_receiveDelegate(data);
				}
				catch (NullReferenceException e)
				{
					Console.WriteLine(e);
				}
			}
		}

		/// <summary>
		/// Send message through a UDP socket.
		/// </summary>
		/// <param name="remoteIpAddress"></param>
		/// <param name="port"></param>
		/// <param name="message"></param>
		public void Send(IPAddress remoteIpAddress, int port, string message)
		{
			if (_hostName == null || _hostIpAddress == null || _hostIpEndPoint == null || _hostUdpClient ==null)
			{
				throw new UdpManagerNotInitializeException("Please init UdpManager first");
			}
			byte[] data = Encoding.ASCII.GetBytes(message);
			_hostUdpClient.Send(data, data.Length, new IPEndPoint(remoteIpAddress, port));
		}

	}
	
}
