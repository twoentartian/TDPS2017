using System;
using System.Threading;
using RaspberryCam;
using TimerManagerNamespace;

namespace Cs_Mono_RaspberryPi
{
	public sealed class RaspberryCamera
	{
		private static RaspberryCamera _instance;

		private static Guid FlashTimerGuid;

		public static byte[] imgData;

		public static RaspberryCamera GetInstance()
		{
			return _instance??(_instance = new RaspberryCamera());
		}

		private RaspberryCamera ()
		{
			camDriver = new CamDriver ("/dev/video0");
		}

		private CamDriver camDriver;

		public void StartStreaming(int width, int height, int fps)
		{
			camDriver.StartVideoStreaming (new PictureSize (width, height), fps);
			TimerManager tempTimerManager = TimerManager.GetInstance ();
			FlashTimerGuid = tempTimerManager.AddTimer (FlashThreadFunc, null, 0, 1000 / fps);
		}

		public void StartStreaming(PictureSize size, int fps)
		{
			camDriver.StartVideoStreaming (size, fps);
			TimerManager tempTimerManager = TimerManager.GetInstance ();
			FlashTimerGuid = tempTimerManager.AddTimer (FlashThreadFunc, null, 0, 1000 / fps);
		}

		private bool FlashThreadFuncSign = false;
		private bool FirstSign = true;
		private void FlashThreadFunc(object arg)
		{
			if (FlashThreadFuncSign)
				return;
			FlashThreadFuncSign = true;
			imgData = GetPictureFromStreaming ();
			if (FirstSign)
			{
				Console.WriteLine ("Picture is ready");
				FirstSign = false;
			}
			FlashThreadFuncSign = false;
		}

		public void StopStreaming()
		{
			TimerManager.GetInstance ().StopTimer (FlashTimerGuid);
			Thread.Sleep (100);
			camDriver.StopVideoStreaming ();
		}

		public byte[] GetPictureFromStreaming()
		{
			while (true)
			{
				if (camDriver.IsVideoStreamOpenned)
				{
					return camDriver.GetVideoFrame ();
				}
				else
				{
					Console.WriteLine ("Wait for camera get ready!");
					Thread.Sleep (1000);
				}
			}
		}

		public void SavePicture(int width, int height,string path)
		{
			camDriver.SavePicture (new PictureSize (width, height), path);
		}

		public void SavePicture(PictureSize size,string path)
		{
			camDriver.SavePicture (size, path);
		}
	}
}

