using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using WinForm_TDPS_2016_Test;

namespace AForgeVideoSourceDevice
{
	static class VideoSourceDevice
	{
		private enum NowState
		{
			Stop, LocalCamera, TcpIp
		}

		private static NowState State = NowState.Stop;

		private static string[] otherModes =
		{
			"TCP IP"
		};

		private static FilterInfoCollection _videoDevices;

		private static VideoCaptureDevice _videoSource;

		public static void Scan()
		{
			FormMain form = FormMain.GetInstance();
			try
			{
				//Find all the input devices
				_videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

				if (_videoDevices.Count == 0)
					throw new ApplicationException();

				form.GetComboBox_CaptureDevice().Items.AddRange(otherModes);
				foreach (FilterInfo device in _videoDevices)
				{
					form.GetComboBox_CaptureDevice().Items.Add(device.Name);
				}
				form.GetComboBox_CaptureDevice().SelectedIndex = 0;
			}
			catch (ApplicationException)
			{
				form.GetComboBox_CaptureDevice().Items.Add("No Capture Device");
				_videoDevices = null;
			}
		}

		public static void UpdateResolution()
		{
			FormMain form = FormMain.GetInstance();
			form.GetComboBox_CaptureResolution().Items.Clear();
			bool localCameraSign = true;
			foreach (var singleMode in otherModes)
			{
				if (form.GetComboBox_CaptureDevice().SelectedItem.ToString() == singleMode)
				{
					localCameraSign = false;
					form.GetComboBox_CaptureResolution().Enabled = false;
				}
			}

			if (localCameraSign)
			{
				form.GetComboBox_CaptureResolution().Enabled = true;
				_videoSource = new VideoCaptureDevice(_videoDevices[form.GetComboBox_CaptureDevice().SelectedIndex - 1].MonikerString);
				int preferData = 0;
				foreach (var tempCap in _videoSource.VideoCapabilities)
				{
					int preferDataTemp = tempCap.FrameSize.Width * tempCap.FrameSize.Height * tempCap.MaximumFrameRate;
					form.GetComboBox_CaptureResolution().Items.Add(string.Format("{0:D} FPS {1:D}x{2:D} {3:D} bits", tempCap.MaximumFrameRate, tempCap.FrameSize.Width, tempCap.FrameSize.Height, tempCap.BitCount));
					if (preferDataTemp > preferData)
					{
						preferData = preferDataTemp;
						form.GetComboBox_CaptureResolution().SelectedIndex = form.GetComboBox_CaptureResolution().Items.Count - 1;
					}
				}
			}
		}

		public static void Start()
		{
			FormMain form = FormMain.GetInstance();

			bool localCameraSign = true;
			foreach (var singleMode in otherModes)
			{
				if (form.GetComboBox_CaptureDevice().SelectedItem.ToString() == singleMode)
				{
					localCameraSign = false;
					State = NowState.TcpIp;
					form.GetPictureBoxTcpIp().Show();
					form.GetVideoSourcePlayer().Hide();
				}
			}
			if (localCameraSign)
			{
				form.GetPictureBoxTcpIp().Hide();
				form.GetVideoSourcePlayer().Show();
				State = NowState.LocalCamera;
				_videoSource = new VideoCaptureDevice(_videoDevices[form.GetComboBox_CaptureDevice().SelectedIndex - 1].MonikerString);
				_videoSource.VideoResolution = _videoSource.VideoCapabilities[form.GetComboBox_CaptureResolution().SelectedIndex];
				form.GetVideoSourcePlayer().VideoSource = _videoSource;
				form.GetVideoSourcePlayer().Start();
			}
		}

		public static void End()
		{
			if (State == NowState.Stop)
			{
				//Reach when when init
			}
			else if (State == NowState.LocalCamera)
			{
				FormMain form = FormMain.GetInstance();
				form.GetVideoSourcePlayer().SignalToStop();
				form.GetVideoSourcePlayer().WaitForStop();
				State = NowState.Stop;
			}
			else if (State == NowState.TcpIp)
			{
				State = NowState.Stop;
			}
			else
			{
				//Never reach
				throw new LogicErrorException();
			}
		}

		public static void FlashTcpIpImage()
		{
			if (State == NowState.TcpIp)
			{
				FormMain form = FormMain.GetInstance();
				FileStream fs = File.OpenRead(TcpIpFileManagerSpace.TcpIpFileManager.GetInstance().TcpIpFilePath);
				int filelength = 0;
				filelength = (int)fs.Length;
				Byte[] image = new Byte[filelength];
				fs.Read(image, 0, filelength);
				PictureBox tempBox = form.GetPictureBoxTcpIp();
				form.GetPictureBoxTcpIp().Image = PicFunc.Resize(Image.FromStream(fs), tempBox.Width, tempBox.Height);
				fs.Close();
			}
		}

	}
}
