using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TDPS_Release_V1._0
{
	public enum MotorDirection
	{
		Forward = 0,
		Backward = 1
	};

	public sealed partial class FormArduinoControlPanel : Form
	{
		#region Singleton

		private static FormArduinoControlPanel _instance;

		public static FormArduinoControlPanel GetInstance()
		{
			return _instance ?? (_instance = new FormArduinoControlPanel());
		}

		private FormArduinoControlPanel()
		{
			InitializeComponent();
			Init();
		}

		#endregion

		private readonly int MaxSpeed = 150;
		private readonly int MinSpeed = 1;

		#region Label

		public delegate void WriteToBusyLabelHandler(string info);
		public void WriteToBusyLabel(string info)
		{
			if (labelArduinoBusy.InvokeRequired == true)
			{
				WriteToBusyLabelHandler set = new WriteToBusyLabelHandler(WriteToBusyLabel);//委托的方法参数应和SetCalResult一致
				labelArduinoBusy.Invoke(set, new object[] {info}); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				labelArduinoBusy.Text = "Arduino: " + info;
			}
		}

		#endregion

		#region Button

		public delegate void ChangeButtonEnableStateHandler(bool state);
		public void ChangeButtonEnableState(bool state)
		{
			if (buttonSend.InvokeRequired == true)
			{
				ChangeButtonEnableStateHandler set = new ChangeButtonEnableStateHandler(ChangeButtonEnableState);//委托的方法参数应和SetCalResult一致
				buttonSend.Invoke(set, new object[] { state }); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				buttonSend.Enabled = state;
			}
		}

		#endregion

		#region Textbox

		public delegate void WriteToConsoleHandler(string info);
		public void WriteToConsole(string info)
		{
			if (textBoxArduinoConsole.InvokeRequired == true)
			{
				WriteToConsoleHandler set = new WriteToConsoleHandler(WriteToConsole);//委托的方法参数应和SetCalResult一致
				textBoxArduinoConsole.Invoke(set, new object[] { info }); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				textBoxArduinoConsole.AppendText(info + Environment.NewLine);
			}
		}

		#endregion

		public void Init()
		{
			labelSpeed.Text = String.Format("Speed ({0:D}~{1:D})", MinSpeed, MaxSpeed);
			comboBoxMotorA.Items.Add(MotorDirection.Forward);
			comboBoxMotorA.Items.Add(MotorDirection.Backward);
			comboBoxMotorB.Items.Add(MotorDirection.Forward);
			comboBoxMotorB.Items.Add(MotorDirection.Backward);

			comboBoxMotorA.SelectedIndex = 0;
			comboBoxMotorB.SelectedIndex = 0;
			textBoxSpeedMotorA.Text = MaxSpeed.ToString();
			textBoxSpeedMotorB.Text = MaxSpeed.ToString();
			textBoxMotorTime.Text = 1000.ToString();
		}

		private void FormArduinoControlPanel_FormClosing(object sender, FormClosingEventArgs e)
		{
			_instance.Hide();
			e.Cancel = true;
		}

		private void buttonSend_Click(object sender, EventArgs e)
		{
			int speedA, speedB, time;
			try
			{
				speedA = Convert.ToInt32(textBoxSpeedMotorA.Text);
				speedB = Convert.ToInt32(textBoxSpeedMotorB.Text);
				time = Convert.ToInt32(textBoxMotorTime.Text);
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
				return;
			}
			if (speedA > MaxSpeed || speedA < MinSpeed)
			{
				MessageBox.Show("Please check your input - motor A!");
				return;
			}
			if (speedB > MaxSpeed || speedB < MinSpeed)
			{
				MessageBox.Show("Please check your input - motor B!");
				return;
			}
			if (time < 0)
			{
				MessageBox.Show("Please check your input - time!");
				return;
			}
			MotorDirection dirA = (MotorDirection) comboBoxMotorA.SelectedIndex;
			MotorDirection dirB = (MotorDirection) comboBoxMotorB.SelectedIndex;
			Arduino.GetInstance().Send(dirA, speedA, dirB, speedB, time);
		}

		private void buttonAngle_Click(object sender, EventArgs e)
		{
			Int16 angle;
			try
			{
				angle = Convert.ToInt16(textBoxAngle.Text);
			}
			catch (FormatException)
			{
				WriteToConsole("Invaild Format !");
				return;
			}
			Arduino.GetInstance().PidTurn(angle);
		}

		private void buttonGetZAngle_Click(object sender, EventArgs e)
		{
			Arduino.GetInstance().GetZAngle();
		}

		private void buttonGetDis_Click(object sender, EventArgs e)
		{
			Arduino.GetInstance().GetDistance();
		}

		private void buttonGoStraight_Click(object sender, EventArgs e)
		{
			short loop;
			try
			{
				loop = Convert.ToInt16(textBoxGoStraightLoop.Text);
			}
			catch (FormatException)
			{
				WriteToConsole("Invaild Format !");
				return;
			}

			Arduino.GetInstance().GoStraight(loop);
			StateManager.ArduinoState.IsBusy = true;
		}

		private void buttonFan_Click(object sender, EventArgs e)
		{
			if (StateManager.ArduinoState.IsFanOpen)
			{
				Arduino.GetInstance().FanControl(false);
				buttonFan.Text = "Fan: OFF";
			}
			else
			{
				Arduino.GetInstance().FanControl(true);
				buttonFan.Text = "Fan: ON";
			}
			StateManager.ArduinoState.IsFanOpen = !StateManager.ArduinoState.IsFanOpen;
		}

		private void buttonHc12Test_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < 5; i++)
			{
				var currentTime = DateTime.Now;
				Arduino.GetInstance().Hc12Send(currentTime.Year + "-");
				Thread.Sleep(100);
				Arduino.GetInstance().Hc12Send(currentTime.Month + "-");
				Thread.Sleep(100);
				Arduino.GetInstance().Hc12Send(currentTime.Day + "-");
				Thread.Sleep(100);
				Arduino.GetInstance().Hc12Send(currentTime.Hour + "-");
				Thread.Sleep(100);
				Arduino.GetInstance().Hc12Send(currentTime.Minute + "-");
				Thread.Sleep(100);
				Arduino.GetInstance().Hc12Send(currentTime.Second + "\n");
				Thread.Sleep(100);
				Arduino.GetInstance().Hc12Send("Meepo");
				Thread.Sleep(100);
				Arduino.GetInstance().Hc12Send("No 03\n");

				Thread.Sleep(300);
			}
		}
	}
}
