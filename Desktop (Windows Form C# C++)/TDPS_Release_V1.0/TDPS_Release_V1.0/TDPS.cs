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
		private static int countAcc = 0;
		public static void T1G1()
		{
			Image<Rgb, Byte> rawImage = new Image<Rgb, byte>((Bitmap)TcpIpFileManager.GetInstance().NowImage);

			double[] threshold1 = new double[]
			{
				200
			};
			double[] threshold2 = new double[]
			{
				100,200,300
			};

			CannyTextureAnalysisResult textureAnalysisResult = Cv.AutoCannyTextureAnalysis(rawImage, threshold1, threshold2, 0);

			//[obsolote] Detect lines and then remove them from final image.
			//DetectLineResult lineResult = Cv.DetectLine(textureAnalysisResult.Img);
			//Image<Gray, Byte> outputImage = textureAnalysisResult.Img;
			//foreach (LineSegment2D line in lineResult.Line)
			//{
			//	outputImage.Draw(line, new Gray(0), 2);
			//}
			//CannyTextureAnalysisResult reduceLinesResult = new CannyTextureAnalysisResult(rawImage, "Reduce the lines");

			float diff = textureAnalysisResult.Diff;
			LinearRegressionResult lrResult = Cv.CalculateLinearRegression(textureAnalysisResult.Img);
			FormMain.GetInstance().WriteToConsole("Diff= " + diff + "   Slope= " + lrResult.Slope + "   Count= " + lrResult.Count);
			FormMain.GetInstance().WriteToPicture1(textureAnalysisResult.Img);

			//[obsolote] Generate nine patch graph.
			//NinePatchResult patchResult = Cv.CalculateNinePatch(textureAnalysisResult.Img);
			//int height = FormMain.GetInstance().GetPicture2Size().Height;
			//int width = FormMain.GetInstance().GetPicture2Size().Width;
			//byte[,,] patchImageData = new byte[height, width, 1];
			//for (var index0 = 0; index0 < 3; index0++)
			//{
			//	for (var index1 = 0; index1 < 3; index1++)
			//	{
			//		if (patchResult.Patch[index0,index1])
			//		{
			//			for (var y = height/3*index1; y < height/3*(index1+1); y++)
			//			{
			//				for (var x = width/3*index0; x < width/3*(index0+1); x++)
			//				{
			//					patchImageData[y, x, 0] = 255;
			//				}
			//			}
			//		}
			//	}
			//}
			//Image<Gray, Byte> patchImage = new Image<Gray, Byte>(patchImageData);
			//FormMain.GetInstance().WriteToPicture2(patchImage);

			

			//threshold
			float threshold = 0.05f;
			if (lrResult.Count < 6000)
			{
				Arduino.GetInstance().Send(MotorDirection.Backward, 80, MotorDirection.Backward, 80, 300);
				return;
			}
			if (Math.Abs(diff) > threshold)
			{
				countAcc = 0;
				if (diff > threshold)
				{
					if (_previousDiff < -threshold)
					{
						Arduino.GetInstance().Send(MotorDirection.Forward, 35, MotorDirection.Backward, 35, 300);
					}
					else
					{
						Arduino.GetInstance().Send(MotorDirection.Forward, 80, MotorDirection.Backward, 100, 300);
					}
				}
				else if (diff < -threshold)
				{
					if (_previousDiff > threshold)
					{
						Arduino.GetInstance().Send(MotorDirection.Backward, 35, MotorDirection.Forward, 35, 300);
					}
					else
					{
						Arduino.GetInstance().Send(MotorDirection.Backward, 80, MotorDirection.Forward, 100, 300);
					}
				}
			}
			else
			{
				if (lrResult.Slope < 0.1)
				{
					countAcc++;
					if (countAcc > 3)
					{
						Arduino.GetInstance().Send(MotorDirection.Forward, 75, MotorDirection.Forward, 75, 3000);
					}
					else
					{
						Arduino.GetInstance().Send(MotorDirection.Forward, 75, MotorDirection.Forward, 75, 1000);
					}
					
				}
				else
				{
					countAcc = 0;
					Arduino.GetInstance().Send(MotorDirection.Forward, 75, MotorDirection.Forward, 75, 200);
				}
				
			}
			_previousDiff = diff;
		}

		public static bool IsT1G1End()
		{


			return false;
		}

		public static void T1G2()
		{
			
		}

		public static bool IsT1G2End()
		{


			return true;
		}

		public static void T2G1()
		{
			
		}

		public static bool IsT2G1End()
		{


			return true;
		}

		public static void T2G2()
		{
			
		}

		public static bool IsT2G2End()
		{


			return true;
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
			FormMain.GetInstance().SampleThreadFunc();
			AutoSampleTimerCallbackFuncSign = false;

		}
	}
}
