using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;

namespace TDPS_Release_V1._0
{
	class TcpIpFileManager
	{
		#region Singleton

		protected TcpIpFileManager()
		{

		}

		private static TcpIpFileManager _instance;

		public static TcpIpFileManager GetInstance()
		{
			return _instance ?? (_instance = new TcpIpFileManager());
		}

		#endregion

		private const string _tempFolder = @"TCPIP";

		private readonly string _currentPath = Environment.CurrentDirectory;

		private readonly string _tcpIpFolderPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + _tempFolder;

		private readonly string _tcpIpFilePath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + _tempFolder + Path.DirectorySeparatorChar + "InterNetwork.jpg";

		public string FilePath => _tcpIpFilePath;

		#region Porperty

		public bool IsFileFresh = false;

		public delegate void NewFileComes(string path);

		public NewFileComes NewFileComesEvent;

		#endregion


		public void Init()
		{
			if (!Directory.Exists(_tcpIpFolderPath))
			{
				Directory.CreateDirectory(_tcpIpFolderPath);
			}
			string[] tempFiles = Directory.GetFiles(_tcpIpFolderPath);
			foreach (var tempFile in tempFiles)
			{
				File.Delete(tempFile);
			}
			IsFileFresh = false;
		}

		public void AddTempFile(Image argImage)
		{
			FileStream fs = File.Create(_tcpIpFilePath);
			argImage.Save(fs, ImageFormat.Jpeg);
			fs.Close();
			NewFileComesEvent?.Invoke(_tcpIpFilePath);
			IsFileFresh = true;
		}
	}
}
