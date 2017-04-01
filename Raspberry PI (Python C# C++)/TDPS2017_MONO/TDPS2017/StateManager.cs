using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimerManagerNamespace;

namespace Cs_Mono_RaspberryPi
{
	sealed class StateManager
	{
		#region Singleton

		public static StateManager GetInstance()
		{
			return _instance ?? (_instance = new StateManager());
		}

		private static StateManager _instance;

		private StateManager()
		{
			
		}

		#endregion

		#region NetworkState

		private bool _findServer = false;
		private Guid _timerGuid = Guid.Empty;
		public bool FindServer
		{
			get { return _findServer; }
			set
			{
				_findServer = value;
				TimerManager tempTimerManager = TimerManager.GetInstance();

				if (value)
				{
					Console.WriteLine("Find Server");
					_timerGuid = tempTimerManager.AddTimer(InterNetwork.SendPictureTimerCallback, null, 0, 1000/Program.fps);
				}
				else
				{
					Console.WriteLine("Lost Server");
					tempTimerManager.StopTimer(_timerGuid);
				}
			}
		}

		#endregion


	}
}
