using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TcpUdpManagerNamespace;

namespace TDPS_Release_V1._0
{
	sealed class Arduino
	{
		#region Singleton

		private Arduino()
		{

		}

		private static Arduino _instance;

		public static Arduino GetInstance()
		{
			return _instance ?? (_instance = new Arduino());
		}

		#endregion

		#region Property

		private const string Separator = "#";
		private static readonly string[] Separators =
		{
			Separator
		};

		#endregion

		public void Send(string data)
		{
			if (StateManager.TcpState.IsClientConnected == true)
			{
				TcpManager tempTcpManager = TcpManager.GetInstance();
				tempTcpManager.TcpServerSend(InterNetwork.GetInstance().GetRemoteClient(), data + Environment.NewLine);
			}
			else
			{
				FormArduinoControlPanel.GetInstance().WriteToConsole("Error: No Client Connected !");
			}
		}

		public void Send(MotorDirection dirA, int speedA, MotorDirection dirB, int speedB, int time)
		{
			if (StateManager.TcpState.IsClientConnected == true)
			{
				string data = "Motor" + Separator + (int) dirA + Separator + speedA + Separator + (int) dirB + Separator + speedB + Separator + time + Separator + Environment.NewLine;
				TcpManager tempTcpManager = TcpManager.GetInstance();
				tempTcpManager.TcpServerSend(InterNetwork.GetInstance().GetRemoteClient(), data);
				StateManager.ArduinoState.IsBusy = true;
			}
			else
			{
				FormArduinoControlPanel.GetInstance().WriteToConsole("Error: No Client Connected !");
			}
		}

		public void GoStraight(short loop)
		{
			byte[] dataBytes = new byte[4];
			dataBytes[0] = 0x06;
			dataBytes[1] = (byte)(loop / 256);
			dataBytes[2] = (byte)(loop % 256);
			dataBytes[3] = 0xff;
			string dataStringCore = string.Empty;
			foreach (var b in dataBytes)
			{
				dataStringCore = dataStringCore + Convert.ToString(b) + "#";
			}
			string dataString = "TOMC#" + dataStringCore;
			Arduino.GetInstance().Send(dataString);
			StateManager.ArduinoState.IsBusy = true;
		}

		public void GetZAngle()
		{
			byte[] dataBytes = new byte[2];
			dataBytes[0] = 0x04;
			dataBytes[1] = 0xff;
			string dataStringCore = string.Empty;
			foreach (var b in dataBytes)
			{
				dataStringCore = dataStringCore + Convert.ToString(b) + "#";
			}
			string dataString = "TOMC#" + dataStringCore;
			Arduino.GetInstance().Send(dataString);
		}

		public void GetDistance()
		{
			byte[] dataBytes = new byte[2];
			dataBytes[0] = 0x05;
			dataBytes[1] = 0xff;
			string dataStringCore = string.Empty;
			foreach (var b in dataBytes)
			{
				dataStringCore = dataStringCore + Convert.ToString(b) + "#";
			}
			string dataString = "TOMC#" + dataStringCore;
			Arduino.GetInstance().Send(dataString);
		}

		public void PidTurn(int angle)
		{
			if (angle > 180 || angle < -180)
			{
				throw new ArgumentException();
			}
			short value = (short)((angle + 180) * 32768 / 180);

			byte[] dataBytes = new byte[4];
			dataBytes[0] = 0x03;
			dataBytes[1] = (byte)(value / 256);
			dataBytes[2] = (byte)(value % 256);
			dataBytes[3] = 0xff;
			string dataStringCore = string.Empty;
			foreach (var b in dataBytes)
			{
				dataStringCore = dataStringCore + Convert.ToString(b) + "#";
			}
			string dataString = "TOMC#" + dataStringCore;
			Arduino.GetInstance().Send(dataString);
			StateManager.ArduinoState.IsBusy = true;
		}

		public void FanControl(bool sign)
		{
			byte[] dataBytes = new byte[3];
			dataBytes[0] = 0x07;
			dataBytes[2] = 0xff;
			if (sign)
			{
				dataBytes[1] = 0x01;
			}
			else
			{
				dataBytes[1] = 0x00;
			}

			string dataStringCore = string.Empty;
			foreach (var b in dataBytes)
			{
				dataStringCore = dataStringCore + Convert.ToString(b) + "#";
			}
			string dataString = "TOMC#" + dataStringCore;
			Arduino.GetInstance().Send(dataString);
		}


		public void Hc12Send(string argString)
		{
			byte[] stringBytes = Encoding.ASCII.GetBytes(argString);
			foreach (var d in stringBytes)
			{
				byte[] dataBytes = new byte[3];
				dataBytes[0] = 0x08;
				dataBytes[1] = d;
				dataBytes[2] = 0xff;
				string dataStringCore = string.Empty;
				foreach (var b in dataBytes)
				{
					dataStringCore = dataStringCore + Convert.ToString(b) + "#";
				}
				string dataString = "TOMC#" + dataStringCore;
				Arduino.GetInstance().Send(dataString);
			}
		}

		private float _sonicDistance;
		private bool _sonicDistanceUseSign = true;
		public float SonicDistance
		{
			get
			{
				while (true)
				{
					if (_sonicDistanceUseSign)
					{
						Thread.Sleep(100);
					}
					else
					{
						break;
					}
				}
				_sonicDistanceUseSign = true;
				return _sonicDistance;
			}
			set
			{
				_sonicDistance = value;
				_sonicDistanceUseSign = false;
			}
		}
	}
}
