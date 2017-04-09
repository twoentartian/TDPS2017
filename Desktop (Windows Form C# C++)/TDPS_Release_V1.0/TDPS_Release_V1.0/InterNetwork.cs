using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using TcpUdpManagerNamespace;
using TimerManagerNamespace;

namespace TDPS_Release_V1._0
{
	sealed class InterNetwork
	{
		#region Singleton

		private static InterNetwork _instance;

		private InterNetwork()
		{

		}

		public static InterNetwork GetInstance()
		{
			return _instance ?? (_instance = new InterNetwork());
		}

		#endregion

		#region Shell

		public Guid GetRemoteClient()
		{
			return TcpServer.GetInstance()._clientGuid;
		}

		public void Init()
		{
			BroadcastService.GetInstance().Init();
			TcpServer.GetInstance().Init();
		}

		public void AcquirePicture()
		{
			TcpManager.GetInstance().TcpServerSend(GetRemoteClient(), "#AcquirePicture#" + Environment.NewLine);
		}

		#endregion

		#region Kernal

		sealed class TcpServer
		{
			#region Singleton

			private static TcpServer _instance;

			private TcpServer()
			{

			}

			public static TcpServer GetInstance()
			{
				return _instance ?? (_instance = new TcpServer());
			}

			#endregion

			#region Config

			private int _serverPort = 15879;

			public int ServerPort => _serverPort;

			#endregion

			#region Property

			public Guid _clientGuid = Guid.Empty;
			private const string Separator = "#";
			private static readonly string[] Separators =
			{
				Separator
			};

			#endregion

			public void Init()
			{
				TcpManager tempTcpManager = TcpManager.GetInstance();
				tempTcpManager.InitTcpServer(_serverPort, Receive, TcpServerNewClient);
			}

			private void TcpServerNewClient(EndPoint argEndPoint)
			{
				IPEndPoint argIpEndPoint = (IPEndPoint)argEndPoint;
				_clientGuid = TcpManager.GetInstance().TcpServerGetGuid(argIpEndPoint.Address);
				StateManager.TcpState.IsClientConnected = true;
			}

			private void Receive(object argClient)
			{
				var client = (TcpManager.TcpClientWithGuid)argClient;
				NetworkStream ns = client.TcpStream;
				StreamReader sr = new StreamReader(ns);

				while (true)
				{
					string data;
					try
					{
						data = sr.ReadLine();
					}
					catch (IOException e)
					{
						client.Stop();
						StateManager.TcpState.IsClientConnected = false;
						break;
					}

					if (data == null)
					{
						client.Stop();
						StateManager.TcpState.IsClientConnected = false;
						break;
					}
					string[] items = data.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
					if (items[0] == "Picture")
					{
						long contentLen = Convert.ToInt64(items[1]);
						MemoryStream ms = new MemoryStream();
						long size = 0;
						while (size < contentLen)
						{
							//Receive 256 bytes for every loop
							byte[] bits = new byte[4096];
							int r = client.TcpClient.GetStream().Read(bits, 0, bits.Length);
							if (r <= 0) break;
							ms.Write(bits, 0, r);
							size += r;
						}
						System.Drawing.Image img;
						try
						{
							img = System.Drawing.Image.FromStream(ms);
						}
						catch (Exception e)
						{
							InterNetwork.GetInstance().AcquirePicture();
							continue;
						}

						TcpIpFileManager.GetInstance().AddTempFile(img);
					}
					else if (items[0] == "Motor")
					{
						if (items[1] == "Finished")
						{
							StateManager.ArduinoState.IsBusy = false;
						}
						else if (items[1] == "GetCommand")
						{
							
						}
						else
						{
							throw new NotImplementedException();
						}
					}
					else
					{
						throw new NotImplementedException();
					}


				}
			}

		}

		sealed class BroadcastService
		{
			#region Singleton

			private static BroadcastService _instance;

			private BroadcastService()
			{

			}

			public static BroadcastService GetInstance()
			{
				return _instance ?? (_instance = new BroadcastService());
			}

			#endregion

			#region Config

			private int localPort = 15000;
			private int BroadcastPort = 15878;


			#endregion

			#region Property

			private string _contentTcp;
			private Guid _timerGuid;
			public const string Separator = "#";
			public static readonly string[] Separators =
			{
				Separator
			};

			#endregion

			public void Init()
			{
				//Set the local UDP port
				UdpManager tempUdpManager = UdpManager.GetInstance();
				tempUdpManager.InitUdp(localPort, UdpReceiveCommand);

				//Generate the broadcast info
				_contentTcp = ("Server" + Separator + tempUdpManager.HostIpAddress + Separator + TcpServer.GetInstance().ServerPort + Separator + Environment.NewLine);

				//Set the timer
				TimerManager tempTimerManager = TimerManager.GetInstance();
				_timerGuid = tempTimerManager.AddTimer(BroadcastTimerOnElapsed, null, 0, 1000);
			}

			private static string udpReceiveBuffer;

			private static void UdpReceiveCommand(byte[] dataBytes)
			{
				//TODO: add function when UDP receive message.
				throw new NotImplementedException();
			}

			private void BroadcastTimerOnElapsed(object arg)
			{
				UdpManager tempUdpManager = UdpManager.GetInstance();
				tempUdpManager.Send(IPAddress.Broadcast, BroadcastPort, _contentTcp);
			}

			public void BroadcastToInterNetwork(string data)
			{
				UdpManager tempUdpManager = UdpManager.GetInstance();
				tempUdpManager.Send(IPAddress.Broadcast, BroadcastPort, data);
			}

			public void StopBroadcast()
			{
				TimerManager tempTimerManager = TimerManager.GetInstance();
				tempTimerManager.StopTimer(_timerGuid);
			}


		}

		#endregion

	}
}
