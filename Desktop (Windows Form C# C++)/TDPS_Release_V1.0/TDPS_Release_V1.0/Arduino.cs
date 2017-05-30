using System;
using System.Collections.Generic;
using System.Linq;
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
				StateManager.ArduinoState.IsBusy = true;
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

	}
}
