using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video.DirectShow;
using AForgeVideoSourceDevice;
using WinForm_TDPS_2016_Test;

namespace StateManagerSpace
{
	class StateManager
	{
		#region Singleton
		protected StateManager()
		{

		}
		private static StateManager _instance;
		public static StateManager GetInstance()
		{
			return _instance ?? (_instance = new StateManager());
		}
		#endregion

		public CaptureState Capture;
		public UdpState Udp;

		public void Init()
		{
			Capture = new CaptureState();
			Udp = new UdpState();
			Capture.Init();
			Udp.Init();
		}
	}

	class State
	{
		public virtual void Init()
		{
			
		}
	}

	class CaptureState : State
	{
		public CaptureState()
		{

		}

		public override void Init()
		{
			SetStop();
			VideoSourceDevice.Scan();
		}
		public enum NowState
		{
			Start, Stop
		}

		private NowState _state = NowState.Stop;

		public NowState GetState()
		{
			return _state;
		}

		public void SetStart()
		{
			_state = NowState.Start;
			// the behavior of starting
			FormMain.GetInstance().GetButton_BeginEnd().Text = "End";
			FormMain.GetInstance().GetButton_Sample().Enabled = true;
			VideoSourceDevice.Start();
		}

		public void SetStop()
		{
			_state = NowState.Stop;
			// the behavior of ending
			FormMain.GetInstance().GetButton_BeginEnd().Text = "Start";
			FormMain.GetInstance().GetButton_Sample().Enabled = false;
			VideoSourceDevice.End();
		}
	}

	class UdpState : State
	{
		public UdpState()
		{
			
		}

		public enum NowState
		{
			WaitForConnection, Connected, Close
		}

		private NowState _state = NowState.Close;

		public override void Init()
		{
			SetUdpClose();
			FormMain.GetInstance().LabelUDP.Text = "UDP: ";
		}

		public void SetUdpClose(string info = null)
		{
			_state = NowState.Close;
			if (String.IsNullOrWhiteSpace(info))
			{
				FormMain.GetInstance().LabelUDP.Text = "UDP: Close";
			}
			else
			{
				FormMain.GetInstance().LabelUDP.Text = "UDP: Close Info: " + info;
			}
			
		}

		public void SetUdpFindClient(string info = null)
		{
			_state = NowState.Connected;
			try
			{
				if (String.IsNullOrWhiteSpace(info))
				{
					FormMain.GetInstance().LabelUDP.Text = "UDP: Find client, waiting for data";
				}
				else
				{
					FormMain.GetInstance().LabelUDP.Text = "UDP: Find client, waiting for data Info: " + info;
				}
			}
			catch (Exception e)
			{

			}
		}

		public void SetUdpWaitForConnection(string info = null)
		{
			_state = NowState.WaitForConnection;
			if (String.IsNullOrWhiteSpace(info))
			{
				FormMain.GetInstance().LabelUDP.Text = "UDP: Wait for connection";
			}
			else
			{
				FormMain.GetInstance().LabelUDP.Text = "UDP: Wait for connection Info: " + info;
			}
		}
	}
}
