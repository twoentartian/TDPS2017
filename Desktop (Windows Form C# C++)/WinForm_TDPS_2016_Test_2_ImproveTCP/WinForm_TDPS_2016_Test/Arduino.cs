using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinForm_TDPS_2016_TCPIP;

namespace WinForm_TDPS_2016_Test
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

		public delegate void ArduinoBecomesFree();

		public ArduinoBecomesFree FreeFunc;

		public class SendArguements
		{
			public SendArguements(MotorDirection argDirA, int argSpeedA, MotorDirection argDirB, int argSpeedB, int argTime)
			{
				DirA = argDirA;
				SpeedA = argSpeedA;
				DirB = argDirB;
				SpeedB = argSpeedB;
				Time = argTime;
			}

			public readonly MotorDirection DirA;
			public readonly int SpeedA;
			public readonly MotorDirection DirB;
			public readonly int SpeedB;
			public readonly int Time;
		}

		private SendArguements previousSendArguements;

		public void Send(SendArguements arg)
		{
			Thread sendThread = new Thread(SendTask);
			sendThread.IsBackground = true;
			previousSendArguements = arg;
			sendThread.Start(previousSendArguements);
		}

		public void Send(MotorDirection dirA, int speedA, MotorDirection dirB, int speedB, int time)
		{
			Send(new SendArguements(dirA, speedA, dirB, speedB, time));
		}

		private void SendTask(object arg)
		{
			SendArguements realArg = (SendArguements) arg;
			while (true)
			{
				if (!BusySign)
				{
					break;
				}
				else
				{
					Thread.Sleep(100);
				}
			}
			string data = "Motor" + BroadcastService.Separator + (int) realArg.DirA + BroadcastService.Separator + realArg.SpeedA +
			              BroadcastService.Separator + (int) realArg.DirB + BroadcastService.Separator + realArg.SpeedB +
			              BroadcastService.Separator + realArg.Time + BroadcastService.Separator + Environment.NewLine;
			BroadcastService broadcastService = BroadcastService.GetInstance();
			broadcastService.BroadcastToInterNetwork(data);

			BusySign = true;
		}

		private Thread releaseThread;
		private bool _busySign = false;
		public bool BusySign
		{
			get { return _busySign; }
			set
			{
				_busySign = value;
				if (value)
				{
					FormArduinoControlPanel.GetInstance().ArduinoBusyState.Text = "Arduino: Busy";
					releaseThread = new Thread(AutoFreeFunc) {IsBackground = true};
					releaseThread.Start();
				}
				else
				{
					FormArduinoControlPanel.GetInstance().ArduinoBusyState.Text = "Arduino: Free";
					if (releaseThread.IsAlive)
					{
						if (Thread.CurrentThread == releaseThread)
						{
							FreeFunc?.Invoke();
						}
						releaseThread.Abort();
					}
					FreeFunc?.Invoke();
				}
			}
		}

		private void AutoFreeFunc()
		{
			Thread.Sleep(2000);
			if (BusySign)
			{
				BusySign = false;
			}
		}

	}
}
