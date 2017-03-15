using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace WinForm_TDPS_2016_Test
{
	#region CV Results

	class TextureAnalysisResult
	{
		public Image<Gray, byte> Img;
	}

	/// <summary>
	/// Analysis result of the image's texture
	/// </summary>
	class LbpTextureAnalysisResult : TextureAnalysisResult
	{
		public LbpTextureAnalysisResult(byte[,,] argBytes)
		{
			LbpFactor = new byte[argBytes.GetLength(0), argBytes.GetLength(1)];
			for (int i = 0; i < argBytes.GetLength(0); i++)
			{
				for (int j = 0; j < argBytes.GetLength(1); j++)
				{
					LbpFactor[i, j] = argBytes[i, j, 0];
				}
			}
			base.Img = new Image<Gray, byte>(argBytes);
		}

		public readonly byte[,] LbpFactor;
	}

	/// <summary>
	/// Analysis result of the image's texture
	/// </summary>
	class CannyTextureAnalysisResult : TextureAnalysisResult
	{
		public CannyTextureAnalysisResult(Image<Gray,Byte> argImage)
		{
			base.Img = argImage;
			Data = new int[argImage.Width];
			for (int x = 0; x < argImage.Width; x++)
			{
				for (int y = 0; y < argImage.Height; y++)
				{
					if (argImage.Data[y,x,0] == 255)
					{
						Data[x]++;
					}
				}
			}
		}

		public CannyTextureAnalysisResult(Image<Gray, Byte> argImage, string argInfo)
		{
			base.Img = argImage;
			Data = new int[argImage.Width];
			for (int x = 0; x < argImage.Width; x++)
			{
				for (int y = 0; y < argImage.Height; y++)
				{
					if (argImage.Data[y, x, 0] == 255)
					{
						Data[x]++;
					}
				}
			}
			Info = argInfo;
		}

		public readonly int[] Data;

		public readonly string Info;
	}

	/// <summary>
	/// Detect basic elements result (such as circlr, line, rectangle and triangle)
	/// </summary>
	class DetectBasicEleementResult
	{
		public readonly Image TriangleImage;
		public readonly List<Triangle2DF> TriangleList;

		public readonly Image RectangleImage;
		public readonly List<RotatedRect> BoxList;

		public readonly Image CircleImage;
		public readonly CircleF[] Circles;

		public readonly Image LineImage;
		public readonly LineSegment2D[] Line;

		public readonly string Msg;

		public DetectBasicEleementResult(Image<Bgr, Byte> argImg, List<Triangle2DF> argTriangleLis, List<RotatedRect> argBoxList, CircleF[] argCircles, LineSegment2D[] argLines, string argMsg)
		{
			TriangleList = argTriangleLis;
			Image<Bgr, Byte> triangleImage = argImg.CopyBlank();
			foreach (Triangle2DF triangle in argTriangleLis)
				triangleImage.Draw(triangle, new Bgr(Color.DarkBlue), 2);
			TriangleImage = triangleImage.Bitmap;

			BoxList = argBoxList;
			Image<Bgr, Byte> rectangleImage = argImg.CopyBlank();
			foreach (RotatedRect box in argBoxList)
				rectangleImage.Draw(box, new Bgr(Color.DarkOrange), 2);
			RectangleImage = rectangleImage.Bitmap;

			if (argCircles != null)
			{
				Circles = argCircles;
				Image<Bgr, Byte> circleImage = argImg.CopyBlank();
				foreach (CircleF circle in argCircles)
					circleImage.Draw(circle, new Bgr(Color.Brown), 2);
				CircleImage = circleImage.Bitmap;
			}
			else
			{
				Circles = null;
				CircleImage = argImg.CopyBlank().Bitmap;
			}

			Line = argLines;
			Image<Bgr, Byte> lineImage = argImg.CopyBlank();
			foreach (LineSegment2D line in argLines)
				lineImage.Draw(line, new Bgr(Color.Green), 2);
			LineImage = lineImage.Bitmap;

			Msg = argMsg;
		}
	}

	#endregion

	/// <summary>
	/// Static class: All computer vision functions go here
	/// </summary>
	static class Cv
	{
		#region CV Arguements

		public enum DetectMode
		{
			NoCircle,
			IncludeCircle
		}

		#endregion

		#region Func
		private static int LbpComparer(Byte center, Byte target)
		{
			if (center >= target)
			{
				return 0;
			}
			else
			{
				return 1;
			}
		}
		#endregion

		#region Static CV Functions

		/// <summary>
		/// Detect basic elements (such as circlr, line, rectangle and triangle)
		/// </summary>
		/// <param name="argPath"></param>
		/// <param name="argtMode"></param>
		/// <returns></returns>
		public static DetectBasicEleementResult DetectBasicElement(string argPath, DetectMode argtMode)
		{
			StringBuilder msgBuilder = new StringBuilder("Performance: ");

			//Load the image from file and resize it for display
			Image<Bgr, byte> img = new Image<Bgr, byte>(argPath).Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);

			//Convert the image to grayscale and filter out the noise
			UMat uimage = new UMat();
			CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

			//use image pyr to remove noise
			UMat pyrDown = new UMat();
			CvInvoke.PyrDown(uimage, pyrDown);
			CvInvoke.PyrUp(pyrDown, uimage);

			//Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

			#region circle detection

			CircleF[] circles = null;
			Stopwatch watch = new Stopwatch();
			double cannyThreshold = 180.0;
			if (argtMode == DetectMode.IncludeCircle)
			{
				watch = Stopwatch.StartNew();
				double circleAccumulatorThreshold = 120;
				circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);

				watch.Stop();
				msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));
			}
			#endregion

			#region Canny and edge detection
			watch.Reset(); watch.Start();
			double cannyThresholdLinking = 120.0;
			UMat cannyEdges = new UMat();
			CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);

			LineSegment2D[] lines = CvInvoke.HoughLinesP(cannyEdges,
			   1, //Distance resolution in pixel-related units
			   Math.PI / 45.0, //Angle resolution measured in radians.
			   20, //threshold
			   30, //min Line width
			   10); //gap between lines

			watch.Stop();
			msgBuilder.Append(String.Format("Canny & Hough lines - {0} ms; ", watch.ElapsedMilliseconds));
			#endregion

			#region Find triangles and rectangles
			watch.Reset(); watch.Start();
			List<Triangle2DF> triangleList = new List<Triangle2DF>();
			List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle

			using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
			{
				CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
				int count = contours.Size;
				for (int i = 0; i < count; i++)
				{
					using (VectorOfPoint contour = contours[i])
					using (VectorOfPoint approxContour = new VectorOfPoint())
					{
						CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
						if (CvInvoke.ContourArea(approxContour, false) > 250) //only consider contours with area greater than 250
						{
							if (approxContour.Size == 3) //The contour has 3 vertices, it is a triangle
							{
								Point[] pts = approxContour.ToArray();
								triangleList.Add(new Triangle2DF(pts[0], pts[1], pts[2]));
							}
							else if (approxContour.Size == 4) //The contour has 4 vertices.
							{
								#region determine if all the angles in the contour are within [80, 100] degree
								bool isRectangle = true;
								Point[] pts = approxContour.ToArray();
								LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

								for (int j = 0; j < edges.Length; j++)
								{
									double angle = Math.Abs(edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
									if (angle < 80 || angle > 100)
									{
										isRectangle = false;
										break;
									}
								}
								#endregion

								if (isRectangle) boxList.Add(CvInvoke.MinAreaRect(approxContour));
							}
						}
					}
				}
			}

			watch.Stop();
			msgBuilder.Append(String.Format("Triangles & Rectangles - {0} ms; ", watch.ElapsedMilliseconds));
			#endregion

			return new DetectBasicEleementResult(img, triangleList, boxList, circles, lines, msgBuilder.ToString());
		}

		/// <summary>
		/// Analyze the texture of image
		/// </summary>
		/// <param name="argPath"></param>
		/// <returns></returns> 
		public static LbpTextureAnalysisResult LbpTextureAnalysis(string argPath)
		{
			Image<Gray, Byte> img = new Image<Gray, byte>(argPath);
			return LbpTextureAnalysis(img);
		}

		public static LbpTextureAnalysisResult LbpTextureAnalysis(Image<Gray,Byte> argImage)
		{
			byte[,,] rawImgData = argImage.Data;
			byte[,,] outputResult = new byte[argImage.Height - 2, argImage.Width - 2, 1];
			//LBP
			for (int yLoc = 1; yLoc < argImage.Height - 1; yLoc++)
			{
				for (int xLoc = 1; xLoc < argImage.Width - 1; xLoc++)
				{
					int[] temp = new int[8];
					temp[0] = LbpComparer(rawImgData[yLoc, xLoc, 0], rawImgData[yLoc - 1, xLoc - 1, 0]);
					temp[1] = LbpComparer(rawImgData[yLoc, xLoc, 0], rawImgData[yLoc - 1, xLoc, 0]);
					temp[2] = LbpComparer(rawImgData[yLoc, xLoc, 0], rawImgData[yLoc - 1, xLoc + 1, 0]);
					temp[3] = LbpComparer(rawImgData[yLoc, xLoc, 0], rawImgData[yLoc, xLoc + 1, 0]);
					temp[4] = LbpComparer(rawImgData[yLoc, xLoc, 0], rawImgData[yLoc + 1, xLoc + 1, 0]);
					temp[5] = LbpComparer(rawImgData[yLoc, xLoc, 0], rawImgData[yLoc + 1, xLoc, 0]);
					temp[6] = LbpComparer(rawImgData[yLoc, xLoc, 0], rawImgData[yLoc + 1, xLoc - 1, 0]);
					temp[7] = LbpComparer(rawImgData[yLoc, xLoc, 0], rawImgData[yLoc, xLoc - 1, 0]);

					int power = 1;
					outputResult[yLoc - 1, xLoc - 1, 0] = 0;
					for (int i = 0; i < temp.Length; i++)
					{
						if (power * temp[i] > 255)
						{
							throw new LogicErrorException();
						}
						outputResult[yLoc - 1, xLoc - 1, 0] += (byte)(power * temp[i]);
						power = power * 2;
					}
				}
			}
			return new LbpTextureAnalysisResult(outputResult);
		}

		public static CannyTextureAnalysisResult CannyTextureAnalysis(string argPath, double threshold1, double threshold2, int iteration = 1)
		{
			Image<Rgb, Byte> img = new Image<Rgb, Byte>(argPath);
			return CannyTextureAnalysis(img, threshold1, threshold2, iteration);
		}

		public static CannyTextureAnalysisResult CannyTextureAnalysis(Image<Gray, Byte> argImage, double threshold1, double threshold2, int iteration = 1)
		{
			Image<Gray, Byte> cannyImg = new Image<Gray, byte>(argImage.Size);
			for (int i = 0; i < iteration; i++)
			{
				if (i == 0)
				{
					cannyImg = argImage.Canny(threshold1, threshold2);
				}
				else
				{
					cannyImg = cannyImg.Canny(threshold1, threshold2);
				}
			}
			return new CannyTextureAnalysisResult(cannyImg);
		}

		public static CannyTextureAnalysisResult CannyTextureAnalysis(Image<Rgb,Byte> argImage, double threshold1, double threshold2,int iteration = 1)
		{
			Image<Gray, Byte> cannyImg = new Image<Gray, byte>(argImage.Size);
			for (int i = 0; i < iteration; i++)
			{
				if (i == 0)
				{
					cannyImg = argImage.Canny(threshold1, threshold2);
				}
				else
				{
					cannyImg = cannyImg.Canny(threshold1, threshold2);
				}
			}
			return new CannyTextureAnalysisResult(cannyImg);
		}

		/// <summary>
		/// Suggest value sets are threshold1=[200] threshold2=[200,300,400]
		/// </summary>
		/// <param name="argPath"></param>
		/// <param name="threshold1"></param>
		/// <param name="threshold2"></param>
		/// <param name="factorBetweenMinAndMax"></param>
		/// <returns></returns>
		public static CannyTextureAnalysisResult AutoCannyTextureAnalysis(string argPath, double[] threshold1, double[] threshold2, int factorBetweenMinAndMax = 3)
		{
			Image<Rgb, Byte> img = new Image<Rgb, Byte>(argPath);
			return AutoCannyTextureAnalysis(img, threshold1, threshold1, factorBetweenMinAndMax);
		}

		public static CannyTextureAnalysisResult AutoCannyTextureAnalysis(Image<Rgb,Byte> argImage, double[] threshold1, double[] threshold2, int factorBetweenMinAndMax = 3)
		{
			Image<Gray, Byte> cannyImg = new Image<Gray, byte>(argImage.Size);
			CannyTextureAnalysisResult bestCannyTextureAnalysisResult = null;
			double bestCannyTextureAnalysisResultValue = 0;
			foreach (var singleThresholdValue1 in threshold1)
			{
				foreach (var singleThresholdValue2 in threshold2)
				{
					cannyImg = argImage.Canny(singleThresholdValue1, singleThresholdValue2);
					CannyTextureAnalysisResult tempCannyTextureAnalysisResult = new CannyTextureAnalysisResult(cannyImg, string.Format("Threshold1/2 = {0:D} {1:D}", (int)singleThresholdValue1, (int)singleThresholdValue2));
					int sum=0, min= tempCannyTextureAnalysisResult.Data[0], max=min;
					foreach (var i in tempCannyTextureAnalysisResult.Data)
					{
						sum += i;
						if (i > max)
						{
							max = i;
						}
						if (i < min)
						{
							min = i;
						}
					}
					double ave = (double) sum / tempCannyTextureAnalysisResult.Data.Length;

					if (max / ave > factorBetweenMinAndMax && factorBetweenMinAndMax != 0)
					{
						return tempCannyTextureAnalysisResult;
					}
					if (max / ave > bestCannyTextureAnalysisResultValue)
					{
						bestCannyTextureAnalysisResultValue = max / ave;
						bestCannyTextureAnalysisResult = tempCannyTextureAnalysisResult;
					}
				}
			}

			if (bestCannyTextureAnalysisResult == null)
			{
				throw new LogicErrorException();
			}
			return bestCannyTextureAnalysisResult;
		}

		public static CannyTextureAnalysisResult AutoCannyTextureAnalysis(Image<Gray, Byte> argImage, double[] threshold1, double[] threshold2, int factorBetweenMinAndMax = 3)
		{
			Image<Gray, Byte> cannyImg = new Image<Gray, byte>(argImage.Size);
			CannyTextureAnalysisResult bestCannyTextureAnalysisResult = null;
			double bestCannyTextureAnalysisResultValue = 0;
			foreach (var singleThresholdValue1 in threshold1)
			{
				foreach (var singleThresholdValue2 in threshold2)
				{
					cannyImg = argImage.Canny(singleThresholdValue1, singleThresholdValue2);
					CannyTextureAnalysisResult tempCannyTextureAnalysisResult = new CannyTextureAnalysisResult(cannyImg, string.Format("Threshold1/2 = {0:D} {1:D}", (int)singleThresholdValue1, (int)singleThresholdValue2));
					int sum = 0, min = tempCannyTextureAnalysisResult.Data[0], max = min;
					foreach (var i in tempCannyTextureAnalysisResult.Data)
					{
						sum += i;
						if (i > max)
						{
							max = i;
						}
						if (i < min)
						{
							min = i;
						}
					}
					double ave = (double)sum / tempCannyTextureAnalysisResult.Data.Length;

					if (max / ave > factorBetweenMinAndMax && factorBetweenMinAndMax != 0)
					{
						return tempCannyTextureAnalysisResult;
					}
					if (max / ave > bestCannyTextureAnalysisResultValue)
					{
						bestCannyTextureAnalysisResultValue = max / ave;
						bestCannyTextureAnalysisResult = tempCannyTextureAnalysisResult;
					}
				}
			}

			if (bestCannyTextureAnalysisResult == null)
			{
				throw new LogicErrorException();
			}
			return bestCannyTextureAnalysisResult;
		}

		#endregion
	}
}
