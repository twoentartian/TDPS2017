using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace TDPS_Release_V1._0
{
	static  class Tdps
	{
		private static float _previousDiff;
		public static void T1G1()
		{
			Image<Rgb, Byte> rawImage;
			string tempPath = TcpIpFileManager.GetInstance().FilePath;
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
			FormMain.GetInstance().WriteToConsole("Diff = " + diff);
			FormMain.GetInstance().WriteToPicture1(textureAnalysisResult.Img);


			//threa
			float threshold = 0.05f;
			if (Math.Abs(diff) > threshold)
			{
				if (diff > threshold)
				{
					if (_previousDiff < -threshold)
					{
						Arduino.GetInstance().Send(MotorDirection.Forward, 35, MotorDirection.Backward, 35, 200);
					}
					else
					{
						Arduino.GetInstance().Send(MotorDirection.Forward, 75, MotorDirection.Backward, 75, 200);
					}
				}
				else if (diff < -threshold)
				{
					if (_previousDiff > threshold)
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

	}
}
