using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CS_UWP_HKReporter_TDPS2017_V2
{
	class TcpIpManager
	{
		#region Singleton

		private static TcpIpManager _instance;

		protected TcpIpManager()
		{
			Init();
		}

		public static TcpIpManager GetInstance()
		{
			if (_instance == null)
			{
				_instance = new TcpIpManager();
			}
			return _instance;
		}

		#endregion

		private IPAddress[] _localAddresses;
		private IPAddress _localIpV4Address;
		private string _localName;
		private int _localUdpPort = 15878;
		private Socket _localUdpSocket;
		private IPEndPoint _remoteIpEndPoint;

		private readonly string[] _separator = new string[]
		{
			"#"
		};

		private NowState _state = NowState.NotFindServer;

		public NowState State
		{
			get { return _state; }
			set
			{
				_state = value;
				if (value == NowState.FindServer)
				{
					StartTimerUdpService();
				}
				else if (value == NowState.NotFindServer)
				{
					StopTimerUdpService();
				}
				else
				{
					throw new LogicErrorException();
				}
			}
		}

		public enum NowState
		{
			NotFindServer, FindServer
		}

		private async void Init()
		{
			_state = NowState.NotFindServer;

			_localName = Dns.GetHostName();
			_localAddresses = await Dns.GetHostAddressesAsync(_localName);
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
			_localUdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			_localUdpSocket.Bind(new IPEndPoint(_localIpV4Address, _localUdpPort));
			_localUdpSocket.EnableBroadcast = true;
			_localUdpSocket.ReceiveBufferSize = 256;

			Console _console = Console.GetInstance();
			_console.Display("Open port: " + _localUdpSocket.LocalEndPoint.ToString());
		}

		public async Task ListenAsync()
		{
			while (true)
			{
				byte[] dataSend = Encoding.ASCII.GetBytes("TEST");
				EndPoint ep = new IPEndPoint(IPAddress.Broadcast, 0);
				_localUdpSocket.SendTo(dataSend, ep);
				await Task.Delay(100);
				byte[] buffer = new byte[256];
				EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
				Flush();
				_localUdpSocket.ReceiveFrom(buffer, ref remoteEndPoint);
				string data = Encoding.ASCII.GetString(buffer);
				string[] parts = data.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

				if (parts[0] == "Server")
				{
					_remoteIpEndPoint = new IPEndPoint(IPAddress.Parse(parts[1]), Convert.ToInt32(parts[2]));
					break;
				}
				else if (parts[0] == "Motor")
				{

				}
				else
				{
					throw new LogicErrorException();
				}
			}

			//Set State Find Sever
			State = NowState.FindServer;
			Console console = Console.GetInstance();
			console.Display("Find server: " + _remoteIpEndPoint);
		}

		public void SendPictureAsync(string path)
		{
			long contentLen = 0;

			Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				sock.Connect(_remoteIpEndPoint);
			}
			catch (Exception)
			{
				//Set State Not Find Server
				State = NowState.NotFindServer;
				Console console = Console.GetInstance();
				console.Display("Lost Server");
				return;
			}

			//Send Picture Command
			sock.Send(BitConverter.GetBytes(1L));

			FileStream fs = new FileStream(path,FileMode.Open);
			contentLen = fs.Length;
			sock.Send(BitConverter.GetBytes(contentLen));
			//循环发送文件内容
			while (true)
			{
				byte[] bits = new byte[256];
				int r = fs.Read(bits, 0, bits.Length);
				if (r <= 0) break; //如果从流中读取完毕,则break;
				sock.Send(bits, r, SocketFlags.None);
			}
			sock.Shutdown(SocketShutdown.Both);
			//由于读取操作会是文件指针产生偏移,最后读取结束之后,要将指针置为0;
			fs.Flush();
			fs.Dispose();
		}

		private void Flush()
		{
			while (_localUdpSocket.Available != 0)
			{
				byte[] buffer = new byte[256];
				EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
				_localUdpSocket.ReceiveFrom(buffer, ref remoteEndPoint);
			}
		}

		private Timer _udpTimer;

		public void StartTimerUdpService()
		{
			TimerCallback udpTimerCallback = UdpTimerCallback;
			_udpTimer = new Timer(udpTimerCallback, null, 0, 100);
		}

		public void StopTimerUdpService()
		{
			_udpTimer.Dispose();
		}

		private void UdpTimerCallback(object state)
		{
			byte[] dataSend = Encoding.ASCII.GetBytes("TEST");
			EndPoint ep = new IPEndPoint(IPAddress.Broadcast, 0);
			_localUdpSocket.SendTo(dataSend, ep);
			byte[] buffer = new byte[256];
			EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
			Flush();
			_localUdpSocket.ReceiveFrom(buffer, ref remoteEndPoint);
			string data = Encoding.ASCII.GetString(buffer);
			string[] parts = data.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
			 if (parts[0] == "Motor")
			{
				byte dirA, dirB, speedA, speedB, time1, time2;
				try
				{
					dirA = Convert.ToByte(parts[1]);
					speedA = Convert.ToByte(parts[2]);
					dirB = Convert.ToByte(parts[3]);
					speedB = Convert.ToByte(parts[4]);
					int time = Convert.ToInt32(parts[5]);
					time1 = (byte)(time / 0xff);
					time2 = (byte)(time % 0xff);
				}
				catch (Exception e)
				{
					Debug.WriteLine("Format error");
					return;
				}
				SerialRaspberry st = SerialRaspberry.GetInstance();
				byte[] tempBytes = new byte[]
				{
					0x01, dirA, speedA, dirB, speedB, time1, time2, 0xFF
				};
				st.Write(tempBytes);
			}
			else if (parts[0] == "Server")
			{
				
			}
			else
			{
				throw new LogicErrorException();
			}
		}
	}
}
