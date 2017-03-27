using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TempFileManagerSpace
{
	class TempFileManager
	{
		#region Singleton

		protected TempFileManager()
		{

		}

		private static TempFileManager _instance;

		public static TempFileManager GetInstance()
		{
			return _instance ?? (_instance = new TempFileManager());
		}

		#endregion

		private const string _tempFolder = @"TEMP";

		private readonly string _currentPath = Environment.CurrentDirectory;

		public void Init()
		{
			string tempPath = _currentPath + Path.DirectorySeparatorChar + _tempFolder;
			if (!Directory.Exists(tempPath))
			{
				Directory.CreateDirectory(tempPath);
			}
			string[] tempFiles = Directory.GetFiles(tempPath);
			foreach (var tempFile in tempFiles)
			{
				File.Delete(tempFile);
			}
		}

		public void ReleasePageFile()
		{
			string tempPath = _currentPath + Path.DirectorySeparatorChar + _tempFolder;
			string[] tempFiles = Directory.GetFiles(tempPath);
			foreach (var tempFile in tempFiles)
			{
				File.Delete(tempFile);
			}
		}

		public string AddTempFile(Image argImage)
		{
			string tempPath = _currentPath + Path.DirectorySeparatorChar + _tempFolder;
			Random ra = new Random();
			string tempFilePath;
			while (true)
			{
				tempFilePath = tempPath + Path.DirectorySeparatorChar + ra.Next(0, 100000) + ".tmp";
				if (!File.Exists(tempFilePath))
				{
					break;
				}
			}
			FileStream fs = File.Create(tempFilePath);
			argImage.Save(fs, ImageFormat.Jpeg);
			fs.Close();
			return tempFilePath;
		}

		public string AddTempFile(string argString)
		{
			string tempPath = _currentPath + Path.DirectorySeparatorChar + _tempFolder;
			Random ra = new Random();
			string tempFilePath;
			while (true)
			{
				tempFilePath = tempPath + Path.DirectorySeparatorChar + ra.Next(0, 100000) + ".tmp";
				if (!File.Exists(tempFilePath))
				{
					break;
				}
			}
			FileStream fs = File.Create(tempFilePath);

			fs.Close();
			return tempFilePath;
		}

		public void DelTempFile(string argPath)
		{
			File.Delete(argPath);
		}

	}
}
