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
		public static bool FinalSign = false;
		public static int Counter = 0;
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
			FormMain.GetInstance().WriteToConsole("Diff= " + diff + "   Slope= " + lrResult.Slope + "   Count= " + lrResult.Count + " CounterFinal= " + Counter);
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
			if (lrResult.Count < 2000)
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
					if (countAcc > 2)
					{
						if (FinalSign)
						{
							Counter++;
							Arduino.GetInstance().Send(MotorDirection.Forward, 75, MotorDirection.Forward, 75, 1000);
						}
						else
						{
							Counter = Counter+3;
							Arduino.GetInstance().Send(MotorDirection.Forward, 75, MotorDirection.Forward, 75, 3000);
						}
					}
					else
					{
						Counter++;
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
			if (!FinalSign)
			{
				return false;
			}
			ColorDetectResult result =  Cv.DetectColor(new Image<Rgb, byte>(TcpIpFileManager.GetInstance().FilePath));
			if (result.Result>180)
			{
				while (true)
				{
					FormMain.GetInstance().SampleThreadFunc();
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
					TcpIpFileManager.GetInstance().IsFileFresh = false;
					float threshold = 0.05f;
					if (Math.Abs(textureAnalysisResult.Diff - 0.15) < threshold)
					{
						return true;
					}
					else
					{
						if (textureAnalysisResult.Diff > threshold)
						{
							Arduino.GetInstance().Send(MotorDirection.Forward, 80, MotorDirection.Backward, 80, 300);
						}
						else if (textureAnalysisResult.Diff < -threshold)
						{
							Arduino.GetInstance().Send(MotorDirection.Backward, 80, MotorDirection.Forward, 80, 300);
						}
					}
					while (StateManager.ArduinoState.IsBusy)
					{
						Thread.Sleep(100);
					}


				}
				
			}
			else
			{
				return false;
			}


		}

		public static void T1G2()
		{
			Arduino.GetInstance().GoStraight(110);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			FormMain.GetInstance().SampleThreadFunc();
			TcpIpFileManager.GetInstance().IsFileFresh = false;
			FormMain.GetInstance().SampleThreadFunc();
			ColorDetectResult result =  Cv.DetectColor(new Image<Rgb, byte>(TcpIpFileManager.GetInstance().FilePath));
			TcpIpFileManager.GetInstance().IsFileFresh = false;
			Arduino.GetInstance().GoStraight(7);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			//if (result.Result <= 130 &&  result.Result >= 60)							//Yellow Green
			//{
			//	FormMain.GetInstance().WriteToConsole("YELLOW");
			//	Arduino.GetInstance().PidTurn(90);
			//	while (StateManager.ArduinoState.IsBusy)
			//	{
			//		Thread.Sleep(100);
			//	}
			//	Arduino.GetInstance().GoStraight(180);
			//	while (true)
			//	{
			//		Arduino.GetInstance().GoStraight(3);
			//		while (StateManager.ArduinoState.IsBusy)
			//		{
			//			Thread.Sleep(100);
			//		}
			//		Arduino.GetInstance().GetDistance();
			//		float dis = Arduino.GetInstance().SonicDistance;
			//		if (dis > 40 && dis < 80)
			//		{
			//			break;
			//		}
			//	}
			//	while (StateManager.ArduinoState.IsBusy)
			//	{
			//		Thread.Sleep(100);
			//	}
			//	Arduino.GetInstance().PidTurn(-90);
			//	while (StateManager.ArduinoState.IsBusy)
			//	{
			//		Thread.Sleep(100);
			//	}

			//}
			//else if (result.Result <= 240)															//Blue
			//{
			//	FormMain.GetInstance().WriteToConsole("BLUE");
			//	Arduino.GetInstance().PidTurn(135);
			//	while (StateManager.ArduinoState.IsBusy)
			//	{
			//		Thread.Sleep(100);
			//	}
			//	Arduino.GetInstance().GoStraight(75);
			//	while (StateManager.ArduinoState.IsBusy)
			//	{
			//		Thread.Sleep(100);
			//	}
			//	Arduino.GetInstance().PidTurn(-55);
			//	while (StateManager.ArduinoState.IsBusy)
			//	{
			//		Thread.Sleep(100);
			//	}

			//	//Finish
			//	Arduino.GetInstance().GoStraight(160);
			//	while (true)
			//	{
			//		Arduino.GetInstance().GoStraight(3);
			//		while (StateManager.ArduinoState.IsBusy)
			//		{
			//			Thread.Sleep(100);
			//		}
			//		Arduino.GetInstance().GetDistance();
			//		float dis = Arduino.GetInstance().SonicDistance;
			//		if (dis > 40 && dis < 80)
			//		{
			//			break;
			//		}
			//	}
			//	Arduino.GetInstance().PidTurn(-80);


			//}
			if (result.Result <= 320)															//Violet
			{
				FormMain.GetInstance().WriteToConsole("VIOLET");
				Arduino.GetInstance().PidTurn(45);
				while (StateManager.ArduinoState.IsBusy)
				{
					Thread.Sleep(100);
				}
				Arduino.GetInstance().GoStraight(75);
				while (StateManager.ArduinoState.IsBusy)
				{
					Thread.Sleep(100);
				}
				Arduino.GetInstance().PidTurn(45);
				while (StateManager.ArduinoState.IsBusy)
				{
					Thread.Sleep(100);
				}

				//Finish
				Arduino.GetInstance().GoStraight(160);
				while (true)
				{
					Arduino.GetInstance().GoStraight(3);
					while (StateManager.ArduinoState.IsBusy)
					{
						Thread.Sleep(100);
					}
					Arduino.GetInstance().GetDistance();
					float dis = Arduino.GetInstance().SonicDistance;
					if (dis > 40 && dis < 80)
					{
						break;
					}
				}
				Arduino.GetInstance().PidTurn(-90);

			}
			else
			{
				throw new LogicErrorException();
			}

		}

		public static bool IsT1G2End()
		{


			return true;
		}

		private static double T2G1_Angle = 0.0;

		public static void T2G1()
		{
			Arduino.GetInstance().PidTurn(-90);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			Arduino.GetInstance().GoStraight(110);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			Arduino.GetInstance().PidTurn(45);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			//Image<Rgb, Byte> rawImage = new Image<Rgb, byte>((Bitmap)TcpIpFileManager.GetInstance().NowImage);

			//double[] threshold1 = new double[]
			//{
			//	200
			//};
			//double[] threshold2 = new double[]
			//{
			//	100,200,300
			//};
			//Image<Gray, byte> rawGrayImage = new Image<Gray, byte>(rawImage.Size);
			//CvInvoke.CvtColor(rawImage, rawGrayImage, ColorConversion.Rgb2Gray);
			//DetectLineResult lines =  Cv.DetectLine(rawGrayImage);

			//Image<Rgb, Byte> lineImage = rawImage.CopyBlank();
			//float slopeCounter = 0;
			//int counter = 0;
			//foreach (LineSegment2D line in lines.Line)
			//{
			//	if (Math.Abs((float)(line.P1.Y - line.P2.Y) / (line.P1.X - line.P2.X)) < 0.5)
			//	{
			//		counter++;
			//		slopeCounter += -(float)(line.P1.Y - line.P2.Y) / (line.P1.X - line.P2.X);
			//		lineImage.Draw(line, new Rgb(Color.Green), 2);
			//	}

			//}
			//float slope = slopeCounter / counter;
			//T2G1_Angle = Math.Atan(slope);
			//FormMain.GetInstance().WriteToPicture1(lineImage);
			//FormMain.GetInstance().WriteToConsole("ANGLE= " + T2G1_Angle);
			////Normal -0.096

			//if (Math.Abs(T2G1_Angle - 0.096) < 0.001)
			//{

			//}
			//else
			//{
			//	if (T2G1_Angle - 0.096 > 0.001)
			//	{
			//		Arduino.GetInstance().Send(MotorDirection.Backward, 80, MotorDirection.Forward, 100, 200);
			//	}
			//	if (T2G1_Angle - 0.096 < -0.001)
			//	{
			//		Arduino.GetInstance().Send(MotorDirection.Forward, 80, MotorDirection.Backward, 100, 200);
			//	}
			//}
		}

		public static bool IsT2G1End()
		{
			//if (Math.Abs(T2G1_Angle - 0.096) < 0.001)
			//{
			//	return true;
			//}
			//else
			//{
			//	return false;
			//}
			return true;
		}

		public static void T2G2()
		{
			Arduino.GetInstance().GoStraight(80);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			while (true)
			{
				Arduino.GetInstance().GoStraight(3);
				while (StateManager.ArduinoState.IsBusy)
				{
					Thread.Sleep(100);
				}
				Arduino.GetInstance().GetDistance();
				float dis = Arduino.GetInstance().SonicDistance;
				if (dis > 40 && dis < 80)
				{
					break;
				}
			}
			Arduino.GetInstance().PidTurn(90);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			//After turning
			Arduino.GetInstance().GoStraight(60);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			Arduino.GetInstance().PidTurn(-90);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			//After turning
			//Stone Road
			Arduino.GetInstance().GoStraight(200);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			while (true)
			{
				Arduino.GetInstance().GoStraight(3);
				while (StateManager.ArduinoState.IsBusy)
				{
					Thread.Sleep(100);
				}
				Arduino.GetInstance().GetDistance();
				float dis = Arduino.GetInstance().SonicDistance;
				if (dis > 20 && dis < 60)
				{
					break;
				}
			}

			//Turning at the edge
			Arduino.GetInstance().PidTurn(-90);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			while (true)
			{
				Arduino.GetInstance().GoStraight(2);
				while (StateManager.ArduinoState.IsBusy)
				{
					Thread.Sleep(100);
				}
				Arduino.GetInstance().GetDistance();
				float dis = Arduino.GetInstance().SonicDistance;
				if (dis > 40 && dis < 80)
				{
					break;
				}
			}

			//Reaching the fish food box
			Arduino.GetInstance().Send(MotorDirection.Backward, 150, MotorDirection.Backward, 150, 1250);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}

			Arduino.GetInstance().PidTurn(-90);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			Arduino.GetInstance().Send(MotorDirection.Backward, 150, MotorDirection.Backward, 150, 500);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}

			Arduino.GetInstance().FanControl(true);
			Thread.Sleep(15000);
			Arduino.GetInstance().FanControl(false);
		}

		public static bool IsT2G2End()
		{


			return true;
		}

		public static void T3G2()
		{
			Arduino.GetInstance().GoStraight(240);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			while (true)
			{
				Arduino.GetInstance().GoStraight(3);
				while (StateManager.ArduinoState.IsBusy)
				{
					Thread.Sleep(100);
				}
				Arduino.GetInstance().GetDistance();
				float dis = Arduino.GetInstance().SonicDistance;
				if (dis > 40 && dis < 80)
				{
					break;
				}
			}
			Arduino.GetInstance().PidTurn(-90);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			Arduino.GetInstance().GoStraight(80);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			Arduino.GetInstance().PidTurn(90);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}
			Arduino.GetInstance().GoStraight(50);
			while (StateManager.ArduinoState.IsBusy)
			{
				Thread.Sleep(100);
			}

			for (int i = 0; i < 5; i++)
			{
				var currentTime = DateTime.Now;
				Arduino.GetInstance().Hc12Send(currentTime.Year + "-");
				Thread.Sleep(100);
				Arduino.GetInstance().Hc12Send(currentTime.Month + "-");
				Thread.Sleep(100);
				Arduino.GetInstance().Hc12Send(currentTime.Day + "-");
				Thread.Sleep(100);
				Arduino.GetInstance().Hc12Send(currentTime.Hour + "-");
				Thread.Sleep(100);
				Arduino.GetInstance().Hc12Send(currentTime.Minute + "-");
				Thread.Sleep(100);
				Arduino.GetInstance().Hc12Send(currentTime.Second + "\n");
				Thread.Sleep(100);
				Arduino.GetInstance().Hc12Send("Meepo");
				Thread.Sleep(100);
				Arduino.GetInstance().Hc12Send("No 03\n");

				Thread.Sleep(300);
			}
			

		}

		public static bool IsT3G2End()
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
