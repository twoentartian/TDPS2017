using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinForm_TDPS_2016_TCPIP;

namespace WinForm_TDPS_2016_Test
{
	public enum MotorDirection
	{
		Forward = 0,
		Backward = 1
	};

	public partial class FormArduinoControlPanel : Form
	{
		#region Singleton

		protected static FormArduinoControlPanel Instance;

		public static FormArduinoControlPanel GetInstance()
		{
			return Instance ?? (Instance = new FormArduinoControlPanel());
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

		public Label ArduinoBusyState => labelArduino;

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
			Instance.Hide();
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


	}
}
