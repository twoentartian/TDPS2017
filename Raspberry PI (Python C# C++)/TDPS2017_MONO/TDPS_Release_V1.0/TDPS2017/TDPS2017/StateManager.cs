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
		public bool FindServer
		{
			get { return _findServer; }
			set
			{
				_findServer = value;
				if (value)
				{
					Console.WriteLine("Find Server");
				}
				else
				{
					Console.WriteLine("Lost Server");
				}
			}
		}

		#endregion


	}
}
