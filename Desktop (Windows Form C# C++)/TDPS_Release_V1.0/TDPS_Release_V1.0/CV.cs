using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Markup;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace TDPS_Release_V1._0
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
		public CannyTextureAnalysisResult(Image<Gray, Byte> argImage, string argInfo)
		{
			Img = argImage;
			Data = new float[argImage.Width];
			for (int x = 0; x < argImage.Width; x++)
			{
				for (int y = 0; y < argImage.Height; y++)
				{
					if (argImage.Data[y, x, 0] == 255)
					{
						Data[x] += (float) y / argImage.Height;
					}
				}
			}
			Info = argInfo;
			float allValues = 0;
			float allPointValues = 0;
			for (int point = 0; point < Data.Length; point++)
			{
				allPointValues += Data[point] * point;
				allValues += Data[point];
			}
			Center = ((float) allPointValues) / allValues / Data.Length;
			Diff = Center - 0.5f;
		}

		public readonly float[] Data;

		public readonly string Info;

		public readonly float Center;

		public readonly float Diff;
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

	/// <summary>
	/// Detect line results.
	/// </summary>
	class DetectLineResult
	{
		public readonly LineSegment2D[] Line;

		public DetectLineResult(LineSegment2D[] argLines)
		{
			Line = argLines;
		}
	}

	/// <summary>
	/// Nine patches result. The first index is the x location (row), the second index is the y location (column).
	/// </summary>
	class NinePatchResult
	{
		public readonly float[,] Data;
		public readonly bool[,] Patch;

		public NinePatchResult(float[,] arg)
		{
			Data = arg;
			Patch = new bool[3, 3];
			float[] values = new float[9];
			for (var x = 0; x < 3; x++)
			{
				for (var y = 0; y < 3; y++)
				{
					values[x * 3 + y] = arg[x, y];
				}
			}
			Array.Sort(values);
			for (int i = 8; i > 8-3; i--)
			{
				bool sign = false;
				for (var x = 0; x < 3; x++)
				{
					for (var y = 0; y < 3; y++)
					{
						if (arg[x, y].Equals(values[i]))
						{
							Patch[x, y] = true;
							sign = true;
							break;
						}
					}
					if (sign)
					{
						break;
					}
				}
				if (!sign)
				{
					throw new LogicErrorException();
				}
			}
		}
	}

	/// <summary>
	/// Linear regression result.
	/// </summary>
	class LinearRegressionResult
	{
		public readonly float Slope;
		public readonly long Count;

		public LinearRegressionResult(float argSlope, long argCount)
		{
			Slope = argSlope;
			Count = argCount;
		}
	}

	/// <summary>
	/// Color detect result. Powered by 史童舟: 1-green 2-blue 3-violet     -1--error
	/// </summary>
	class ColorDetectResult
	{
		public readonly float Result;

		public ColorDetectResult(float argResult)
		{
			Result = argResult;
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
		///
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

		/// <summary>
		/// Resize the image to the specified width and height.
		/// </summary>
		/// <param name="image">The image to resize.</param>
		/// <param name="width">The width to resize to.</param>
		/// <param name="height">The height to resize to.</param>
		/// <returns>The resized image.</returns>
		public static Bitmap ResizeImage(Image image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}
		#endregion

		#region Static CV Functions

		/// <summary>
		/// Detect basic elements (such as circlr, line, rectangle and triangle)
		/// </summary>
		/// <param name="argPath"></param>
		/// <param name="argMode"></param>
		/// <returns></returns>
		public static DetectBasicEleementResult DetectBasicElement(string argPath, DetectMode argMode)
		{
			//Load the image from file and resize it for display
			Image<Rgb, byte> img = new Image<Rgb, byte>(argPath).Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);
			return DetectBasicElement(img, argMode);
		}

		/// <summary>
		/// Detect lines.
		/// </summary>
		/// <param name="argImage"></param>
		/// <returns></returns>
		public static DetectLineResult DetectLine(Image<Gray, Byte> argImage)
		{
			Image<Rgb, Byte> grayImage = Image<Rgb, byte>.FromIplImagePtr(argImage);

			UMat uimage = new UMat();
			CvInvoke.CvtColor(grayImage, uimage, ColorConversion.Bgr2Gray);
			UMat pyrDown = new UMat();
			CvInvoke.PyrDown(uimage, pyrDown);
			CvInvoke.PyrUp(pyrDown, uimage);
			double cannyThreshold = 180.0;

			double cannyThresholdLinking = 120.0;
			UMat cannyEdges = new UMat();
			CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);

			LineSegment2D[] lines = CvInvoke.HoughLinesP(cannyEdges,
				1, //Distance resolution in pixel-related units
				Math.PI / 45.0, //Angle resolution measured in radians.
				20, //threshold
				30, //min Line width
				10); //gap between lines
			return new DetectLineResult(lines);

		}

		/// <summary>
		/// Detect basic elements (such as circlr, line, rectangle and triangle)
		/// </summary>
		/// <param name="argImage"></param>
		/// <param name="argMode"></param>
		/// <returns></returns>
		public static DetectBasicEleementResult DetectBasicElement(Image<Rgb, Byte> argImage, DetectMode argMode)
		{
			StringBuilder msgBuilder = new StringBuilder("Performance: ");

			//Convert the image to grayscale and filter out the noise
			UMat uimage = new UMat();
			CvInvoke.CvtColor(argImage, uimage, ColorConversion.Bgr2Gray);

			//use image pyr to remove noise
			UMat pyrDown = new UMat();
			CvInvoke.PyrDown(uimage, pyrDown);
			CvInvoke.PyrUp(pyrDown, uimage);

			//Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

			#region circle detection

			CircleF[] circles = null;
			Stopwatch watch = new Stopwatch();
			double cannyThreshold = 180.0;
			if (argMode == DetectMode.IncludeCircle)
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

			Image<Bgr, Byte> output = new Image<Bgr, byte>(argImage.Bitmap);
			return new DetectBasicEleementResult(output, triangleList, boxList, circles, lines, msgBuilder.ToString());
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

		/// <summary>
		/// Analyze the texture of image
		/// </summary>
		/// <param name="argImage"></param>
		/// <returns></returns>
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
			return new CannyTextureAnalysisResult(cannyImg, "Iteration");
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
			return new CannyTextureAnalysisResult(cannyImg, "Iteration");
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

		/// <summary>
		/// Suggest value sets are threshold1=[200] threshold2=[200,300,400]
		/// </summary>
		/// <param name="argImage"></param>
		/// <param name="threshold1"></param>
		/// <param name="threshold2"></param>
		/// <param name="factorBetweenMinAndMax"></param>
		/// <returns></returns>
		public static CannyTextureAnalysisResult AutoCannyTextureAnalysis(Image<Rgb,Byte> argImage, double[] threshold1, double[] threshold2, int factorBetweenMinAndMax = 3)
		{
			Image<Gray, Byte> cannyImg = new Image<Gray, byte>(argImage.Size);
			CannyTextureAnalysisResult bestCannyTextureAnalysisResult = null;
			double bestCannyTextureAnalysisResultValue = 0;
			bool FirstSign = true;
			foreach (var singleThresholdValue1 in threshold1)
			{
				foreach (var singleThresholdValue2 in threshold2)
				{
					cannyImg = argImage.Canny(singleThresholdValue1, singleThresholdValue2);
					CannyTextureAnalysisResult tempCannyTextureAnalysisResult = new CannyTextureAnalysisResult(cannyImg, string.Format("Threshold1/2 = {0:D} {1:D}", (int)singleThresholdValue1, (int)singleThresholdValue2));
					float sum=0, min= tempCannyTextureAnalysisResult.Data[0], max=min;
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
					if (FirstSign)
					{
						bestCannyTextureAnalysisResultValue = max / ave;
						bestCannyTextureAnalysisResult = tempCannyTextureAnalysisResult;
						FirstSign = false;
					}
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

		/// <summary>
		/// Suggest value sets are threshold1=[200] threshold2=[200,300,400]
		/// </summary>
		/// <param name="argImage"></param>
		/// <param name="threshold1"></param>
		/// <param name="threshold2"></param>
		/// <param name="factorBetweenMinAndMax"></param>
		/// <returns></returns>
		[Obsolete]
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
					float sum = 0, min = tempCannyTextureAnalysisResult.Data[0], max = min;
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

		/// <summary>
		/// Generate nine patch result.
		/// </summary>
		/// <param name="argImage"></param>
		/// <returns></returns>
		public static NinePatchResult CalculateNinePatch(Image<Gray,Byte> argImage)
		{
			int[,] counter = new int[3, 3];
			for (var x = 0; x < argImage.Width; x++)
			{
				for (var y = 0; y < argImage.Height; y++)
				{
					if (argImage.Data[y,x,0] == 255)
					{
						int index0 = 0, index1 = 0;
						if (x < argImage.Width / 3)
						{
							index0 = 0;
						}
						else if (x < argImage.Width * 2 / 3)
						{
							index0 = 1;
						}
						else
						{
							index0 = 2;
						}
						if (x < argImage.Height / 3)
						{
							index1 = 0;
						}
						else if (x < argImage.Height * 2 / 3)
						{
							index1 = 1;
						}
						else
						{
							index1 = 2;
						}
						counter[index0, index1]++;
					}
				}
			}
			int counterAll = 0;
			for (var x = 0; x < 3; x++)
			{
				for (var y = 0; y < 3; y++)
				{
					counterAll += counter[x, y];
				}
			}
			float[,] result = new float[3, 3];
			for (var x = 0; x < 3; x++)
			{
				for (var y = 0; y < 3; y++)
				{
					result[x, y] = (float) counter[x, y] / counterAll;
				}
			}
			return new NinePatchResult(result);
		}

		/// <summary>
		/// Calculate the linear regression slope
		/// </summary>
		/// <param name="argImage"></param>
		/// <returns></returns>
		public static LinearRegressionResult CalculateLinearRegression(Image<Gray, Byte> argImage)
		{
			long countX = 0, countY = 0, count = 0, countXY = 0, countSquareX = 0;
			for (var x = 0; x < argImage.Width; x++)
			{
				for (var y = 0; y < argImage.Height; y++)
				{
					if (argImage.Data[y, x, 0] == 255)
					{
						countXY += x * y;
						count++;
						countX += x;
						countY += y;
						countSquareX += x * x;
					}
				}
			}
			float aveX = (float) countX / count;
			float aveY = (float) countY / count;
			float slope = (float) (countXY - count * aveX * aveY) / (countSquareX - count * aveX * aveX);
			return new LinearRegressionResult(slope, count);
		}

		/// <summary>
		/// Color detect function, powered by 史童舟.
		/// </summary>
		/// <param name="argImage"></param>
		/// <returns></returns>
		public static ColorDetectResult DetectColor(Image<Rgb, Byte> argImage)
		{
			argImage.ROI = Rectangle.FromLTRB(argImage.Width / 3, argImage.Height / 3, argImage.Width / 3 * 2,argImage.Height / 3 * 2);
			Image<Hsv, float> hsvImage = new Image<Hsv, float>(argImage.Size);
			Image<Rgb, float> floatRgbImage = new Image<Rgb, float>(argImage.Size);
			CvInvoke.cvConvertScale(argImage, floatRgbImage, 1.0, 0);
			CvInvoke.CvtColor(floatRgbImage, hsvImage, ColorConversion.Rgb2Hsv);

			float counter = 0;
			for (int x = 0; x < hsvImage.Width; x++)
			{
				for (int y = 0; y < hsvImage.Height; y++)
				{
					counter += hsvImage.Data[y, x, 0];
				}
			}
			float ave = counter / hsvImage.Width / hsvImage.Height;
			return new ColorDetectResult(ave);
		}
		#endregion
	}
}
