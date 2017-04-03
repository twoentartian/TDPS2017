using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForgeVideoSourceDevice;
using Emgu.CV;
using Emgu.CV.Structure;

namespace WinForm_TDPS_2016_Test
{
	class TDPS_Task
	{
		#region Singleton

		private TDPS_Task()
		{
			
		}

		private static TDPS_Task _instance;

		public static TDPS_Task GetInstance()
		{
			return _instance ?? (_instance = new TDPS_Task());
		}

		#endregion


		#region Ground1Task1_FindPath

		public void G1T1Start()
		{
			Arduino.GetInstance().FreeFunc = G1T1Loop;
			G1T1Loop();
		}

		private void G1T1Loop()
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
			int allValues = 0;
			int allPointValues = 0;
			for (int point = 0; point < textureAnalysisResult.Data.Length; point++)
			{
				allPointValues += textureAnalysisResult.Data[point] * point;
				allValues += textureAnalysisResult.Data[point];
			}
			float center = ((float)allPointValues) / allValues / textureAnalysisResult.Data.Length;
			float diff = center - 0.5f;

			// if center is close to 0.5, it means the car direction is nearly towards the path.
			if (Math.Abs(diff) > 0.05)
			{
				MotorDirection dirA = MotorDirection.Forward;
				int speedA = (int) (500 * diff);
				if (speedA > 254)
				{
					speedA = 254;
				}
				if (speedA < 0)
				{
					dirA = MotorDirection.Backward;
					speedA = -speedA;
				}


				MotorDirection dirB = MotorDirection.Forward;
				int speedB = -(int) (500 * diff);
				if (speedB > 254)
				{
					speedB = 254;
				}
				if (speedB < 0)
				{
					dirB = MotorDirection.Backward;
					speedB = -speedB;
				}

				Arduino.GetInstance().Send(dirA, speedA, dirB, speedB, 200);
			}
			else
			{
				Arduino.GetInstance().Send(MotorDirection.Forward, 75, MotorDirection.Forward, 75, 200);
			}
		}

		#endregion
	}
}
