using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace TDPS_Release_V1._0
{
	static  class Tdps
	{
		private static float _previousDiff;
		public static void T1G1()
		{
			Image<Rgb, Byte> rawImage = new Image<Rgb, byte>((Bitmap)TcpIpFileManager.GetInstance().NowImage);
			FormMain.GetInstance().WriteToPictureRaw(rawImage);

			double[] threshold1 = new double[]
			{
				200
			};
			double[] threshold2 = new double[]
			{
				200,300,400
			};
			CannyTextureAnalysisResult textureAnalysisResult = Cv.AutoCannyTextureAnalysis(rawImage, threshold1, threshold2, 0);

			DetectLineResult lineResult = Cv.DetectLine(textureAnalysisResult.Img);
			Image<Gray, Byte> outputImage = textureAnalysisResult.Img;
			foreach (LineSegment2D line in lineResult.Line)
			{
				outputImage.Draw(line, new Gray(0), 2);
			}
			CannyTextureAnalysisResult reduceLinesResult = new CannyTextureAnalysisResult(outputImage, "Reduce the lines");


			float diff = reduceLinesResult.Diff;
			FormMain.GetInstance().WriteToConsole("Diff = " + diff);
			FormMain.GetInstance().WriteToPicture1(reduceLinesResult.Img);

			//threshold
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

		public static TimerCallback AutoSampleTimerCallback = AutoSampleTimerCallbackFunc;

		private static bool AutoSampleTimerCallbackFuncSign = false;
		private static void AutoSampleTimerCallbackFunc(object state)
		{
			if (AutoSampleTimerCallbackFuncSign)
			{
				return;
			}
			if (StateManager.ArduinoState.IsBusy == true)
			{
				return;
			}
			AutoSampleTimerCallbackFuncSign = true;
			FormMain.GetInstance().SampleThreadFunc(null);
			AutoSampleTimerCallbackFuncSign = false;

		}
	}
}
