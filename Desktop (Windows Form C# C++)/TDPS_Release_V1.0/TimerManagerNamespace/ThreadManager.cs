using System;
using System.Collections.Generic;
using System.Threading;

namespace TimerManagerNamespace
{
	public sealed class TimerManager
	{
		#region Singleton

		private static TimerManager _instance;

		private TimerManager()
		{
			
		}

		public static TimerManager GetInstance()
		{
			return _instance ?? (_instance = new TimerManager());
		}

		#endregion

		private class TimerWithGuid
		{
			public Guid TimerGuid = Guid.Empty;
			public bool TimerOccupied = false;
			public Timer TimerInstance;
		}

		private readonly List<TimerWithGuid> _timerList = new List<TimerWithGuid>();

		/// <summary>
		/// Add a thread to the program.
		/// </summary>
		/// <param name="argTimerCallback"></param>
		/// <param name="argState"></param>
		/// <param name="dueTime"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public Guid AddTimer(TimerCallback argTimerCallback, object argState, int dueTime, int period)
		{
			TimerWithGuid freeTimer = null;
			foreach (var tempTimer in _timerList)
			{
				if (tempTimer.TimerOccupied == false)
				{
					freeTimer = tempTimer;
					break;
				}
			}
			if (freeTimer == null)
			{
				freeTimer = new TimerWithGuid();
				_timerList.Add(freeTimer);
			}

			freeTimer.TimerInstance = new Timer(argTimerCallback, argState, dueTime, period);
			freeTimer.TimerOccupied = true;
			freeTimer.TimerGuid = Guid.NewGuid();
			return freeTimer.TimerGuid;
		}

		/// <summary>
		/// Stop a thread by GUID
		/// </summary>
		/// <param name="argGuid"></param>
		public void StopTimer(Guid argGuid)
		{
			foreach (var tempTimer in _timerList)
			{
				if (tempTimer.TimerGuid == argGuid && tempTimer.TimerOccupied)
				{
					tempTimer.TimerInstance.Dispose();
					tempTimer.TimerGuid = Guid.Empty;
					tempTimer.TimerOccupied = false;
					return;
				}
			}
			throw new NoGuidFoundException("Could not find such GUID");
		}

		/// <summary>
		/// Stop all the thread managed by ThreadManager
		/// </summary>
		public void StopAllTimer()
		{
			foreach (var tempTimer in _timerList)
			{
				if (tempTimer.TimerOccupied == true)
				{
					tempTimer.TimerInstance.Dispose();
					tempTimer.TimerGuid = Guid.Empty;
					tempTimer.TimerOccupied = false;
				}
			}
		}

	}
}
