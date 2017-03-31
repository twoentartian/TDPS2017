using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace TcpIpFileManagerSpace
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

		public string TcpIpFilePath => _tcpIpFilePath;

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
		}

		public void AddTempFile(Image argImage)
		{
			FileStream fs = File.Create(_tcpIpFilePath);
			argImage.Save(fs, ImageFormat.Jpeg);
			fs.Close();
		}
	}
}
