using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace WinForm_TDPS_2016_Test
{
	static class Debug
	{

		public static void ShowImg(Image<Bgr, Byte> img)
		{
			ImageViewerManager IVM = ImageViewerManager.Instance();
			IVM.ShowPicture(img);
		}

		public static void ShowImg(Image<Gray, Byte> img)
		{
			ImageViewerManager IVM = ImageViewerManager.Instance();
			IVM.ShowPicture(img);
		}

		public static void ShowImg(System.Drawing.Bitmap img)
		{
			ImageViewerManager IVM = ImageViewerManager.Instance();
			IVM.ShowPicture(new Image<Bgr, Byte>(img));
		}

		private static Bitmap _resultImage;

		public static void Debug1()
		{
			string debugPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "DEBUG" + Path.DirectorySeparatorChar + "DEBUG3.jpg";
			//LbpTextureAnalysisResult textureResult = Cv.LbpTextureAnalysis(debugPath);
			Image<Rgb, Byte> rawImage = new Image<Rgb, byte>(debugPath);
			double[] threshold1 = new double[]
			{
				200
			};
			double[] threshold2 = new double[]
			{
				200,300,400
			};
			CannyTextureAnalysisResult textureAnalysisResult = Cv.AutoCannyTextureAnalysis(rawImage, threshold1, threshold2,0);
			ZedGraphForm tempZedGraphForm = new ZedGraphForm(textureAnalysisResult.Data);
			tempZedGraphForm.Show();
			MessageBox.Show(textureAnalysisResult.Info);
			//ShowImg(textureAnalysisResult.Img);

			/*
			
			FindCuttingPointResult cuttingPointResult = Cv.FindCuttingPoint(textureResult, Cv.FindCuttingPointMode.MaximumMethod  );
			_resultImage = textureResult.img.Bitmap;

			Bitmap newBitmap = new Bitmap(_resultImage.Width, _resultImage.Height);
			Graphics g = Graphics.FromImage(newBitmap);
			g.DrawImage(_resultImage, 0, 0);
			for (int i = 0; i < cuttingPointResult.Edges.Count; i++)
			{
				float location = (float)cuttingPointResult.Edges[i] / cuttingPointResult.Accuracy * newBitmap.Width;
				g.DrawLine(new Pen(Color.Red, 3), location, 0 * newBitmap.Height, location, 1 * newBitmap.Height);
				
			}
			g.Dispose();
			ZedGraphForm tempZedGraphForm = new ZedGraphForm(cuttingPointResult.GrayCounter);
			tempZedGraphForm.Show();
			ShowImg(newBitmap);
			*/

			/*
			string debugPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "DEBUG" + Path.DirectorySeparatorChar + "DEBUG2.jpg";
			int value1, value2;
			try
			{
				FormMain MainForm = FormMain.GetInstance();
				value1 = Convert.ToInt32(MainForm.GetTextBox_Value1().Text);
				value2 = Convert.ToInt32(MainForm.GetTextBox_Value2().Text);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return;
			}

			Image<Rgb, Byte> rawImage = new Image<Rgb, Byte>(debugPath);
			var cannyImage = rawImage.Canny(value1, value2);

			ShowImg(cannyImage);
			*/
		}

		public static void Debug2()
		{
			ShowImg(_resultImage);
		}

	}
}
