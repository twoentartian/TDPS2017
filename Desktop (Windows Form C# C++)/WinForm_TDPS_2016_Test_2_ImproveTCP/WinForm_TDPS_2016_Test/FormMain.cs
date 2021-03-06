﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using System.Threading;
using System.Windows.Forms.VisualStyles;
using AForge.Controls;
using AForgeVideoSourceDevice;
using Emgu.CV.CvEnum;
using StateManagerSpace;
using TempFileManagerSpace;
using TcpIpFileManagerSpace;
using WinForm_TDPS_2016_TCPIP;

namespace WinForm_TDPS_2016_Test
{
	public partial class FormMain : Form
	{
		private readonly StateManager _stateManager;
		private readonly TempFileManager _tempFileManager;
		private readonly TcpIpFileManager _tcpIpFileManager;
		private TcpServer _tcpIpTcpServer;
		private UdpReceiver _udpReceiver;

		#region Singleton

		protected FormMain()
		{
			InitializeComponent();
			Control.CheckForIllegalCrossThreadCalls = false;
			_stateManager = StateManager.GetInstance();
			_tempFileManager = TempFileManager.GetInstance();
			_tcpIpFileManager = TcpIpFileManager.GetInstance();
		}

		public static FormMain GetInstance()
		{
			return _instance ?? (_instance = new FormMain());
		}

		private static FormMain _instance;

		#endregion

		#region Func

		public TextBox GetTextBox_Value1()
		{
			return textBoxValue1;
		}

		public TextBox GetTextBox_Value2()
		{
			return textBoxValue2;
		}

		public Button GetButton_BeginEnd()
		{
			return buttonBeginEnd;
		}

		public Button GetButton_Sample()
		{
			return buttonSample;
		}

		public VideoSourcePlayer GetVideoSourcePlayer()
		{
			return videoSourcePlayer;
		}

		public ComboBox GetComboBox_CaptureDevice()
		{
			return comboBoxCaptureDevice;
		}

		public ComboBox GetComboBox_CaptureResolution()
		{
			return comboBoxCaptureResolution;
		}

		public AForge.Controls.PictureBox GetPictureBox()
		{
			return pictureBox;
		}

		public System.Windows.Forms.PictureBox GetPictureBoxTcpIp()
		{
			return pictureBoxTcpIp;
		}

		#endregion

		#region Form

		private void FormMain_Load(object sender, EventArgs e)
		{
			DoubleBuffered = true;
			_stateManager.Init();
			_tempFileManager.Init();
			_tcpIpFileManager.Init();

			_tcpIpTcpServer = TcpServer.GetInstance();
			_udpReceiver = UdpReceiver.GetInstance();
			BroadcastService.GetInstance();
		}

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			_tempFileManager.ReleasePageFile();
			VideoSourceDevice.End();
		}

		private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			Application.Exit();
		}
		#endregion

		#region Button
		private void buttonBeginEnd_Click(object sender, EventArgs e)
		{
			if (_stateManager.Capture.GetState() == CaptureState.NowState.Start)
			{
				_stateManager.Capture.SetStop();
			}
			else if (_stateManager.Capture.GetState() == CaptureState.NowState.Stop)
			{
				_stateManager.Capture.SetStart();
			}
			else
			{
				//Never Reach
			}
		}


		private float _previousDiff = 0.0f;
		private void buttonSample_Click(object sender, EventArgs e)
		{
			Image<Rgb, Byte> rawImage;
			string tempPath = VideoSourceDevice.GetCurrentPicturePath();
			try
			{
				rawImage = new Image<Rgb, byte>(tempPath);
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				return;
			}

			double[] threshold1 = new double[]
			{
				200
			};
			double[] threshold2 = new double[]
			{
				200,300,400
			};
			CannyTextureAnalysisResult textureAnalysisResult = Cv.AutoCannyTextureAnalysis(rawImage, threshold1, threshold2, 0);
			float diff = textureAnalysisResult.Diff;

			/*
			ZedGraphForm tempGraphForm = new ZedGraphForm(textureAnalysisResult.Data);
			tempGraphForm.Show();
			*/
			Debug.ShowImg(textureAnalysisResult.Img);

			textBoxConsole.AppendText("Center: " + textureAnalysisResult.Center + Environment.NewLine);

			// if center is close to 0.5, it means the car direction is nearly towards the path.
			float threadhold = 0.05f;
			if (Math.Abs(diff) > threadhold)
			{
				if (diff > threadhold)
				{
					if (_previousDiff < -threadhold)
					{
						Arduino.GetInstance().Send(MotorDirection.Forward, 35, MotorDirection.Backward, 35, 200);
					}
					else
					{
						Arduino.GetInstance().Send(MotorDirection.Forward, 75, MotorDirection.Backward, 75, 200);
					}
				}
				else if (diff < -threadhold)
				{
					if (_previousDiff > threadhold)
					{
						Arduino.GetInstance().Send(MotorDirection.Backward, 35, MotorDirection.Forward, 35, 200);
					}
					else
					{
						Arduino.GetInstance().Send(MotorDirection.Backward, 75, MotorDirection.Forward, 75, 200);
					}
				}
			}
			else
			{
				Arduino.GetInstance().Send(MotorDirection.Forward, 75, MotorDirection.Forward, 75, 200);
			}
			_previousDiff = diff;
		}

		private void buttonDebug_Click(object sender, EventArgs e)
		{
			Debug.Debug1();
		}

		private void buttonDebug2_Click(object sender, EventArgs e)
		{
			Debug.Debug2();
		}

		private void buttonOpenArduinoPanel_Click(object sender, EventArgs e)
		{
			FormArduinoControlPanel arduinoForm = FormArduinoControlPanel.GetInstance();
			arduinoForm.Show();
		}

		private void buttonGround1Task1_Click(object sender, EventArgs e)
		{
			TDPS_Task.G1T1Start();
		}
		#endregion

		#region Label

		public Label LabelUDP => labelUdp;

		#endregion

		#region ComboBox
		private void comboBoxCaptureDevice_TextChanged(object sender, EventArgs e)
		{
			VideoSourceDevice.UpdateResolution();
		}
		#endregion


	}
}
