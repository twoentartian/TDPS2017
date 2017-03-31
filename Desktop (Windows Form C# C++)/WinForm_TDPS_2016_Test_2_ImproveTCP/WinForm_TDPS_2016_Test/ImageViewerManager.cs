using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace WinForm_TDPS_2016_Test
{
	class ImageViewerManager
	{
		#region Singleton

		private static ImageViewerManager _singleton;

		protected ImageViewerManager()
		{
			IV.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClosingFunc);
		}

		public static ImageViewerManager Instance()
		{
			if (_singleton == null)
			{
				_singleton = new ImageViewerManager();
			}
			return _singleton;
		}
		#endregion

		#region Event
		private void ClosingFunc(object sender, FormClosingEventArgs e)
		{
			IV.Hide();
			GC.Collect();
			e.Cancel = true;
		}
		#endregion

		private ImageViewer IV = new ImageViewer();

		public void ShowPicture(Image<Bgr, byte> argImage)
		{
			IV.Image = argImage;
			IV.Show();
		}

		public void ShowPicture(Image<Gray, byte> argImage)
		{
			IV.Image = argImage;
			IV.Show();
		}
	}
}
