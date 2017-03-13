using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinForm_TDPS_2016_Test
{
	class PicFunc
	{
		public static Bitmap Resize(Bitmap argBitmap, int width, int height)
		{
			try
			{
				Bitmap b = new Bitmap(width, height);
				Graphics g = Graphics.FromImage(b);
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.DrawImage(argBitmap, new Rectangle(0, 0, width, height), new Rectangle(0, 0, argBitmap.Width, argBitmap.Height), GraphicsUnit.Pixel);
				g.Dispose();
				return b;
			}
			catch
			{
				return null;
			}
		}

		public static Bitmap Resize(Image argBitmap, int width, int height)
		{
			try
			{
				Bitmap b = new Bitmap(width, height);
				Graphics g = Graphics.FromImage(b);
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.DrawImage(argBitmap, new Rectangle(0, 0, width, height), new Rectangle(0, 0, argBitmap.Width, argBitmap.Height), GraphicsUnit.Pixel);
				g.Dispose();
				return b;
			}
			catch
			{
				return null;
			}
		}
	}
}
