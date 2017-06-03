using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimerManagerNamespace;

namespace TDPS_Release_V1._0
{
	 static class StateManager
	{
		public static class TcpState
		{
			private static bool _isClientConnected = false;

			public static bool IsClientConnected
			{
				get { return _isClientConnected; }

				set
				{
					_isClientConnected = value;
					FormMain.GetInstance().ChangeSampleButtonState(value);
					FormMain.GetInstance().ChangeAutoSampleButtonState(value);
					FormMain.GetInstance().WriteToConsole(value ? "Client connect." : "Client lost.");
					if (AutoSampleState.IsOn)
					{
						AutoSampleState.IsOn = false;
					}

				}
			}
		}

		public static class ArduinoState
		{
			private static bool _isBusy = false;

			public static bool IsBusy
			{
				get { return _isBusy; }

				set
				{
					_isBusy = value;
					if (value)
					{
						FormArduinoControlPanel.GetInstance().WriteToBusyLabel("Busy");
						FormArduinoControlPanel.GetInstance().ChangeButtonEnableState(false);
					}
					else
					{
						FormArduinoControlPanel.GetInstance().WriteToBusyLabel("Free");
						FormArduinoControlPanel.GetInstance().ChangeButtonEnableState(true);
					}
					
				}
			}

			private static bool _isFanOpen = false;

			public static bool IsFanOpen
			{
				get { return _isFanOpen; }

				set
				{
					_isFanOpen = value;
					if (value)
					{

					}
					else
					{

					}

				}
			}
		}

		public static class AutoSampleState
		{
			private static bool _isOn = false;

			private static Guid TimerGuid = Guid.Empty;

			public static bool IsOn
			{
				get { return _isOn; }

				set
				{
					_isOn = value;
					TimerManager tempTimerManager = TimerManager.GetInstance();
					if (value)
					{
						TimerGuid = tempTimerManager.AddTimer(Tdps.AutoSampleTimerCallback, null, 0, 1000);
						FormMain.GetInstance().ChangeAutoSampleButtonText("Auto Sample: on");
					}
					else
					{
						tempTimerManager.StopTimer(TimerGuid);
						FormMain.GetInstance().ChangeAutoSampleButtonText("Auto Sample: off");
					}

				}
			}
		}
	}
}
