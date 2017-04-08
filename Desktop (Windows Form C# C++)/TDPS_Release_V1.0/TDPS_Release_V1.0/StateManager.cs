using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


		}
	}
}
