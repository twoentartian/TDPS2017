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

		public static void Debug1()
		{
			string debugPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "DEBUG" + Path.DirectorySeparatorChar + "DEBUG3.jpg";
			//LbpTextureAnalysisResult textureResult = Cv.LbpTextureAnalysis(debugPath);
			Image<Rgb, Byte> rawImage = new Image<Rgb, byte>(debugPath);
			Image<Rgb, Byte> rawimage = rawImage.Resize(480, 240, Inter.Linear);
			double[] threshold1 = new double[]
			{
				200
			};
			double[] threshold2 = new double[]
			{
				200,300,400
			};
			CannyTextureAnalysisResult textureAnalysisResult = Cv.AutoCannyTextureAnalysis(rawimage, threshold1, threshold2,0);
			ZedGraphForm tempZedGraphForm = new ZedGraphForm(textureAnalysisResult.Data);
			tempZedGraphForm.Show();
			MessageBox.Show(textureAnalysisResult.Info);
			//ShowImg(textureAnalysisResult.Img);
		}

		public static void Debug2()
		{
			
		}

	}
}
