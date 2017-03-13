using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace WinForm_TDPS_2016_Test_FFT
{
	public static class FFTPart
	{
		public static double[] Cmp2Mdl(Complex[] input)
		{
			double[] output = new double[input.Length];
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i].Real > 0)
				{
					output[i] = -input[i].Magnitude;
				}
				else
				{
					output[i] = input[i].Magnitude;
				}
			}
			return output;
		}

		public static Complex[] FFT(double[] input, bool invert)
		{
			Complex[] output = new Complex[input.Length];
			for (int i = 0; i < input.Length; i++)
			{
				output[i] = new Complex(input[i], 0.0d);
			}
			return output = FFT(output, invert);
		}

		private static Complex[] FFT(Complex[] input, bool invert)
		{
			if (input.Length == 1)
			{
				return new Complex[] { input[0] };
			}
			int length = input.Length;
			int half = length / 2;
			Complex[] output = new Complex[length];
			double fac = -2.0 * Math.PI / length;
			if (invert)
			{
				fac = -fac;
			}
			Complex[] evens = new Complex[half];

			for (int i = 0; i < half; i++)
			{
				evens[i] = input[2 * i];
			}
			Complex[] evenResult = FFT(evens, invert);
			Complex[] odds = new Complex[half];

			for (int i = 0; i < half; i++)
			{
				odds[i] = input[2 * i + 1];
			}
			Complex[] oddResult = FFT(odds, invert);

			for (int k = 0; k < half; k++)
			{
				double fack = fac * k;
				Complex oddPart = oddResult[k] * new Complex(Math.Cos(fack), Math.Sin(fack));
				output[k] = evenResult[k] + oddPart;
				output[k + half] = evenResult[k] - oddPart;
			}
			return output;
		}

		private static double[] FreqFilter(double[] data, int Nc, Complex[] Hd)
		{
			int N = data.Length;
			Complex[] fData = FFT(data, false);
			for (int i = 0; i < N; i++)
			{
				fData[i] = Hd[i] * fData[i];
			}
			fData = FFT(fData, true);
			data = Cmp2Mdl(fData);
			for (int i = 0; i < N; i++)
			{
				data[i] = -data[i] / N;
			}
			return data;
		}
	}
}
