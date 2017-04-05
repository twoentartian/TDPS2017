using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AForgeVideoSourceDevice;
using Emgu.CV;
using Emgu.CV.Structure;
using TimerManagerNamespace;

namespace WinForm_TDPS_2016_Test
{
	class TDPS_Task
	{
		#region Ground1Task1_FindPath

		public static void G1T1Start()
		{
			 
		}



		private static float _previousDiff = 0.0f;
		private static void G1T1Loop()
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
			float center = textureAnalysisResult.Center;
			float diff = textureAnalysisResult.Diff;

			// if center is close to 0.5, it means the car direction is nearly towards the path.
			float threadhold = 0.02f;
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

		#endregion
	}
}
