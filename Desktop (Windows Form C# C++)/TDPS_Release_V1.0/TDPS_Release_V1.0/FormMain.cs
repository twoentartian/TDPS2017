using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using TcpUdpManagerNamespace;

namespace TDPS_Release_V1._0
{
	public partial class FormMain : Form
	{
		#region Singleton

		public static FormMain GetInstance()
		{
			return _instance ?? (_instance = new FormMain());
		}

		private static FormMain _instance;

		private FormMain()
		{
			InitializeComponent();
		}

		#endregion


		#region Textbox

		public delegate void WriteToConsoleHandler(string info);
		public void WriteToConsole(string info)
		{
			if (textBoxConsole.InvokeRequired == true)
			{
				WriteToConsoleHandler set = new WriteToConsoleHandler(WriteToConsole);//委托的方法参数应和SetCalResult一致
				textBoxConsole.Invoke(set, new object[] { info }); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				textBoxConsole.AppendText(info + Environment.NewLine);
			}
		}

		#endregion


		#region Form

		private void FormMain_Load(object sender, EventArgs e)
		{
			InterNetwork.GetInstance().Init();
			StateManager.TcpState.IsClientConnected = false;
		}

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			TcpManager tempTcpManager = TcpManager.GetInstance();
			tempTcpManager.TcpServerStopAllClient();
		}

		#endregion

		#region Button

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Environment.Exit(0);
		}

		private void arduinoControlPanelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FormArduinoControlPanel.GetInstance().Show();
		}

		private void buttonSample_Click(object sender, EventArgs e)
		{
			Thread sampleThread = new Thread(SampleThreadFunc) {IsBackground = true};
			sampleThread.Start();
		}

		public void SampleThreadFunc()
		{
			if (StateManager.TcpState.IsClientConnected)
			{
				InterNetwork.GetInstance().AcquirePicture();
			}
			else
			{
				WriteToConsole("No client connected !");
			}
			while (true)
			{
				if (TcpIpFileManager.GetInstance().IsFileFresh)
				{
					break;
				}
				else
				{
					Thread.Sleep(100);
				}
			}
			//When the file is ready
			Image<Rgb, Byte> rawImage = new Image<Rgb, byte>((Bitmap)TcpIpFileManager.GetInstance().NowImage);
			WriteToPictureRaw(rawImage);



		}

		private void buttonAutoSample_Click(object sender, EventArgs e)
		{
			StateManager.AutoSampleState.IsOn = !StateManager.AutoSampleState.IsOn;
		}


		public delegate void ChangeButtonEnableStateHandler(bool state);
		public void ChangeSampleButtonState(bool state)
		{
			if (buttonSample.InvokeRequired == true)
			{
				ChangeButtonEnableStateHandler set = new ChangeButtonEnableStateHandler(ChangeSampleButtonState);//委托的方法参数应和SetCalResult一致
				buttonSample.Invoke(set, new object[] { state }); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				buttonSample.Enabled = state;
			}
		}

		public void ChangeAutoSampleButtonState(bool state)
		{
			if (buttonAutoSample.InvokeRequired == true)
			{
				ChangeButtonEnableStateHandler set = new ChangeButtonEnableStateHandler(ChangeAutoSampleButtonState);//委托的方法参数应和SetCalResult一致
				buttonAutoSample.Invoke(set, new object[] { state }); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				buttonAutoSample.Enabled = state;
			}
		}

		public delegate void ChangeButtonTextHandler(string text);
		public void ChangeAutoSampleButtonText(string text)
		{
			if (buttonAutoSample.InvokeRequired == true)
			{
				ChangeButtonTextHandler set = new ChangeButtonTextHandler(ChangeAutoSampleButtonText);//委托的方法参数应和SetCalResult一致
				buttonAutoSample.Invoke(set, new object[] { text }); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				buttonAutoSample.Text = text;
			}
		}
		#endregion

		#region Picturebox

		public Size GetPictureRawSize()
		{
			return pictureBoxRaw.Size;
		}

		public Size GetPicture1Size()
		{
			return pictureBoxOutput1.Size;
		}

		public Size GetPicture2Size()
		{
			return pictureBoxOutput2.Size;
		}

		//RGB
		public delegate void WriteToPictureBoxRgbHandler(Image<Rgb, Byte> image);
		public void WriteToPictureRaw(Image<Rgb, Byte> image)
		{
			if (pictureBoxRaw.InvokeRequired == true)
			{
				WriteToPictureBoxRgbHandler set = new WriteToPictureBoxRgbHandler(WriteToPictureRaw);//委托的方法参数应和SetCalResult一致
				pictureBoxRaw.Invoke(set, new object[] { image }); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				pictureBoxRaw.Image = image.Resize(pictureBoxRaw.Width, pictureBoxRaw.Height, Inter.Linear).Bitmap;
			}
		}

		public void WriteToPicture1(Image<Rgb, Byte> image) 
		{
			if (textBoxConsole.InvokeRequired == true)
			{
				WriteToPictureBoxRgbHandler set = new WriteToPictureBoxRgbHandler(WriteToPicture1);//委托的方法参数应和SetCalResult一致
				textBoxConsole.Invoke(set, new object[] { image }); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				pictureBoxOutput1.Image = image.Resize(pictureBoxOutput1.Width, pictureBoxOutput1.Height, Inter.Linear).Bitmap;
			}
		}

		public void WriteToPicture2(Image<Rgb, Byte> image)
		{
			if (textBoxConsole.InvokeRequired == true)
			{
				WriteToPictureBoxRgbHandler set = new WriteToPictureBoxRgbHandler(WriteToPicture2);//委托的方法参数应和SetCalResult一致
				textBoxConsole.Invoke(set, new object[] { image }); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				pictureBoxOutput2.Image = image.Resize(pictureBoxOutput2.Width, pictureBoxOutput2.Height, Inter.Linear).Bitmap;
			}
		}

		//Gray
		public delegate void WriteToPictureBoxGrayHandler(Image<Gray, Byte> image);
		public void WriteToPictureRaw(Image<Gray, Byte> image)
		{
			if (pictureBoxRaw.InvokeRequired == true)
			{
				WriteToPictureBoxGrayHandler set = new WriteToPictureBoxGrayHandler(WriteToPictureRaw);//委托的方法参数应和SetCalResult一致
				pictureBoxRaw.Invoke(set, new object[] { image }); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				pictureBoxRaw.Image = image.Resize(pictureBoxRaw.Width, pictureBoxRaw.Height, Inter.Linear).Bitmap;
			}
		}

		public void WriteToPicture1(Image<Gray, Byte> image)
		{
			if (textBoxConsole.InvokeRequired == true)
			{
				WriteToPictureBoxGrayHandler set = new WriteToPictureBoxGrayHandler(WriteToPicture1);//委托的方法参数应和SetCalResult一致
				textBoxConsole.Invoke(set, new object[] { image }); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				pictureBoxOutput1.Image = image.Resize(pictureBoxOutput1.Width, pictureBoxOutput1.Height, Inter.Linear).Bitmap;
			}
		}

		public void WriteToPicture2(Image<Gray, Byte> image)
		{
			if (textBoxConsole.InvokeRequired == true)
			{
				WriteToPictureBoxGrayHandler set = new WriteToPictureBoxGrayHandler(WriteToPicture2);//委托的方法参数应和SetCalResult一致
				textBoxConsole.Invoke(set, new object[] { image }); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				pictureBoxOutput2.Image = image.Resize(pictureBoxOutput2.Width, pictureBoxOutput2.Height, Inter.Linear).Bitmap;
			}
		}

		//For System.Drawing.Image
		public delegate void WriteToPictureBoxSystemDrawingHandler(Image image);
		public void WriteToPictureRaw(Image image)
		{
			if (pictureBoxRaw.InvokeRequired == true)
			{
				WriteToPictureBoxSystemDrawingHandler set = new WriteToPictureBoxSystemDrawingHandler(WriteToPictureRaw);//委托的方法参数应和SetCalResult一致
				pictureBoxRaw.Invoke(set, new object[] { image }); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				pictureBoxRaw.Image = Cv.ResizeImage(image, pictureBoxRaw.Width, pictureBoxRaw.Height);
			}
		}

		public void WriteToPicture1(Image image)
		{
			if (pictureBoxOutput1.InvokeRequired == true)
			{
				WriteToPictureBoxSystemDrawingHandler set = new WriteToPictureBoxSystemDrawingHandler(WriteToPictureRaw);//委托的方法参数应和SetCalResult一致
				pictureBoxOutput1.Invoke(set, new object[] { image }); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				pictureBoxOutput1.Image = Cv.ResizeImage(image,pictureBoxOutput1.Width,pictureBoxOutput2.Height);
			}
		}

		public void WriteToPicture2(Image image)
		{
			if (pictureBoxOutput2.InvokeRequired == true)
			{
				WriteToPictureBoxSystemDrawingHandler set = new WriteToPictureBoxSystemDrawingHandler(WriteToPictureRaw);//委托的方法参数应和SetCalResult一致
				pictureBoxOutput2.Invoke(set, new object[] { image }); //此方法第二参数用于传入方法,代替形参result
			}
			else
			{
				pictureBoxOutput2.Image = Cv.ResizeImage(image, pictureBoxOutput2.Width, pictureBoxOutput2.Height);
			}
		}



		#endregion


		#region TDPS_TASK

		private void Menu_Ground1_Task1_Click(object sender, EventArgs e)
		{
			Thread g1T1Thread = new Thread(G1T1ThreadFunc) {IsBackground = true};
			g1T1Thread.Start();
		}

		private void G1T1ThreadFunc()
		{
			WriteToConsole("Ground 1 Task 1 begins to execute !");
			while (true)
			{
				SampleThreadFunc();
				Tdps.T1G1();
				TcpIpFileManager.GetInstance().IsFileFresh = false;
				while (StateManager.ArduinoState.IsBusy)
				{
					Thread.Sleep(100);
				}
				if (Tdps.IsT1G1End())
				{
					break;
				}
			}
			WriteToConsole("Ground 1 Task 1 finished !");
		}

		private void Menu_Ground1_Task2_Click(object sender, EventArgs e)
		{
			Thread g1T2Thread = new Thread(G1T2ThreadFunc) { IsBackground = true };
			g1T2Thread.Start();
		}

		private void G1T2ThreadFunc()
		{
			WriteToConsole("Ground 1 Task 2 begins to execute !");
			while (true)
			{
				SampleThreadFunc();

				if (Tdps.IsT1G2End())
				{
					break;
				}
			}
			WriteToConsole("Ground 1 Task 2 finished !");
		}

		private void Menu_Ground2_Task1_Click(object sender, EventArgs e)
		{
			Thread g2T1Thread = new Thread(G2T1ThreadFunc) { IsBackground = true };
			g2T1Thread.Start();
		}

		private void G2T1ThreadFunc()
		{
			WriteToConsole("Ground 2 Task 1 begins to execute !");
			while (true)
			{
				SampleThreadFunc();

				if (Tdps.IsT2G1End())
				{
					break;
				}
			}
			WriteToConsole("Ground 2 Task 1 finished !");
		}

		private void Menu_Ground2_Task2_Click(object sender, EventArgs e)
		{
			Thread g2T2Thread = new Thread(G2T2ThreadFunc) { IsBackground = true };
			g2T2Thread.Start();
		}

		private void G2T2ThreadFunc()
		{
			WriteToConsole("Ground 2 Task 2 begins to execute !");
			while (true)
			{
				SampleThreadFunc();

				if (Tdps.IsT2G2End())
				{
					break;
				}
			}
			WriteToConsole("Ground 2 Task 2 finished !");

		}

		#endregion

		private void debug1ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//Image<Gray,byte> debugImage = new Image<Gray, byte>("1.jpg");
			//ImageViewerManager.Instance().ShowPicture(debugImage);
			ColorDetectResult result = Cv.DetectColor(new Image<Rgb, byte>("4.jpg"));

		}
	}
}
