/*
 * Copyright © 2003 Maxim Stepin (maxst@hiend3d.com)
 * 
 * Copyright © 2010 Cameron Zemek (grom@zeminvaders.net)
 * 
 * Copyright © 2011 Tamme Schichler (tamme.schichler@googlemail.com)
 *
 * Copyright © 2020 René Rhéaume (rene.rheaume@gmail.com)
 * 
 * This file is part of hqxSharp.
 *
 * hqxSharp is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * hqxSharp is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with hqxSharp. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;

// Disable warnings for features introduced in later versions of C#
#pragma warning disable CA1507 // Use nameof to express symbol names

namespace hqx
{
	/// <summary>
	/// Signature of the core hqx scaling methods
	/// </summary>
	/// <param name="sp">A pointer to the source image.</param>
	/// <param name="dp">A pointer to the destination image.</param>
	/// <param name="Xres">The horizontal resolution of the source image.</param>
	/// <param name="Yres">The vertical resolution of the source image.</param>
	/// <param name="trY">The Y (luminance) threshold.</param>
	/// <param name="trU">The U (chrominance) threshold.</param>
	/// <param name="trV">The V (chrominance) threshold.</param>
	/// <param name="trA">The A (transparency) threshold.</param>
	/// <param name="wrapX">Used for images that can be seamlessly repeated horizontally.</param>
	/// <param name="wrapY">Used for images that can be seamlessly repeated vertically.</param>
	internal unsafe delegate void Magnify(uint* sp, uint* dp, int Xres, int Yres, uint trY, uint trU, uint trV, uint trA, bool wrapX, bool wrapY);

	/// <summary>
	/// Provides access to hqxSharp, the extended port of the hqx pixel art magnification filter.
	/// </summary>
	/// <remarks>
	/// The main focus of hqxSharp lies on asset creation and use in tools; it is not necessarily intended as final output for real-time graphics.
	/// <para>This means that additional functionality (like alpha support and variable AYUV thresholds) and easier code are usually preferred over a small performance increase.</para>
	/// <para>Calls to hqx methods are compatible with the corresponding hqxSharp methods and the default thresholds are those used in hqx.</para>
	/// </remarks>
	public static partial class HqxSharp
	{
		/// <summary>
		/// Compares two ARGB colors according to the provided Y, U, V and A thresholds.
		/// </summary>
		/// <param name="c1">An ARGB color.</param>
		/// <param name="c2">A second ARGB color.</param>
		/// <param name="trY">The Y (luminance) threshold.</param>
		/// <param name="trU">The U (chrominance) threshold.</param>
		/// <param name="trV">The V (chrominance) threshold.</param>
		/// <param name="trA">The A (transparency) threshold.</param>
		/// <returns>Returns true if colors differ more than the thresholds permit, otherwise false.</returns>
		private static bool Diff(uint c1, uint c2, uint trY, uint trU, uint trV, uint trA)
		{
			return Diff(RgbCompactYuv.GetYuv(c1), c1, c2, trY, trU, trV, trA);
		}

		private static bool Diff(int yuv1, uint c1, uint c2, uint trY, uint trU, uint trV, uint trA)
		{
			const int Ymask = 0x00ff0000;
			const int Umask = 0x0000ff00;
			const int Vmask = 0x000000ff;

			var YUV2 = RgbCompactYuv.GetYuv(c2);

			return Diff(yuv1, YUV2, Ymask, trY) ||
			 Diff(yuv1, YUV2, Umask, trU) ||
			 Diff(yuv1, YUV2, Vmask, trV) ||
			 ((uint)FastAbs((int)(c1 >> 24) - (int)(c2 >> 24)) > trA);
		}

		private static bool Diff(int yuv1, int yuv2, int mask, uint threshold)
		{
			return (uint)FastAbs((yuv1 & mask) - (yuv2 & mask)) > threshold;
		}

		/// <summary>
		/// Faster version of absolute value, avoiding branch penalties, from
		/// https://graphics.stanford.edu/~seander/bithacks.html#IntegerAbs
		/// </summary>
		/// <param name="v">Any integer</param>
		/// <returns>Its absolute value</returns>
		private static int FastAbs(int v)
		{
			const int CHAR_BIT = 8;
			var mask = v >> (sizeof(int) * CHAR_BIT) - 1;
			return (v + mask) ^ mask;
		}

		private static readonly unsafe Magnify[] scalers = { Scale2, Scale3, Scale4 };

		public static Bitmap Scale(Bitmap bitmap, byte factor, HqxSharpParameters parameters)
		{
			if ((factor >= 2) && (factor <= 4)) {
				return ScaleCore(bitmap, factor, parameters);
			} else {
				throw new ArgumentOutOfRangeException("factor", factor, "Hqx provides a scale factor of 2x, 3x, or 4x");
			}
		}

		private static Bitmap ScaleCore(Bitmap bitmap, byte factor, HqxSharpParameters parameters)
		{
			return Scale(bitmap, factor, scalers[factor - 2], parameters);
		}

		private static unsafe Bitmap Scale(Bitmap bitmap, byte factor, Magnify magnify, HqxSharpParameters parameters)
		{
			if (bitmap == null) {
				throw new ArgumentNullException("bitmap");
			}
			if (magnify == null) {
				throw new ArgumentNullException("magnify");
			}
			Bitmap dest = null;
			BitmapData bmpData = null, destData = null;
			var Xres = bitmap.Width;
			var Yres = bitmap.Height;
			try {
				dest = new Bitmap(checked((short)(Xres * factor)), checked((short)(Yres * factor)));
				bmpData = LockBits(bitmap, ImageLockMode.ReadOnly);
				destData = LockBits(dest, ImageLockMode.WriteOnly);
				magnify(StartPointer(bmpData), StartPointer(destData), Xres, Yres,
				 parameters.LumaThreshold, parameters.BlueishThreshold, parameters.ReddishThreshold, parameters.AlphaThreshold,
				 parameters.WrapX, parameters.WrapY);
			} finally {
				if (bmpData != null) {
					bitmap.UnlockBits(bmpData);
				}
				if ((destData != null) && (dest != null)) {
					dest.UnlockBits(destData);
				}
			}
			return dest;
		}

		private static unsafe uint* StartPointer(BitmapData bmpData)
		{
			return (uint*)bmpData.Scan0.ToPointer();
		}

		private static BitmapData LockBits(Bitmap source, ImageLockMode lockMode)
		{
			return source.LockBits(new Rectangle(Point.Empty, source.Size), lockMode, PixelFormat.Format32bppArgb);
		}

		private static int FindPattern(uint[] w, uint trY, uint trU, uint trV, uint trA)
		{
			int k;
			var pattern = 0;
			var flag = 1;
			var middle = w[4];
			uint wk;
			var yuv1 = RgbCompactYuv.GetYuv(middle);
			for (k = 0; k < 9; k++) {
				if (k != 4) {
					wk = w[k];
					if ((wk != middle) && Diff(yuv1, middle, wk, trY, trU, trV, trA)) {
						pattern |= flag;
					}
					flag <<= 1;
				}
			}

			return pattern;
		}

		private static unsafe void PrepareWorkPixels(uint* sp, int Xres, bool wrapX, int prevline, int nextline, uint[] w, int i)
		{
			w[1] = *(sp + prevline);
			w[4] = *sp;
			w[7] = *(sp + nextline);

			if (i > 0) {
				w[0] = *(sp + prevline - 1);
				w[3] = *(sp - 1);
				w[6] = *(sp + nextline - 1);
			} else if (wrapX) {
				w[0] = *(sp + prevline + Xres - 1);
				w[3] = *(sp + Xres - 1);
				w[6] = *(sp + nextline + Xres - 1);
			} else {
				w[0] = w[1];
				w[3] = w[4];
				w[6] = w[7];
			}

			if (i < Xres - 1) {
				w[2] = *(sp + prevline + 1);
				w[5] = *(sp + 1);
				w[8] = *(sp + nextline + 1);
			} else if (wrapX) {
				w[2] = *(sp + prevline - Xres + 1);
				w[5] = *(sp - Xres + 1);
				w[8] = *(sp + nextline - Xres + 1);
			} else {
				w[2] = w[1];
				w[5] = w[4];
				w[8] = w[7];
			}
		}

		private static unsafe void All4Mix2To1To1(uint* dp, int dpL, uint[] w, int p0a2, int p0a3, int p1a2, int p1a3, int p2a2, int p2a3, int p3a2, int p3a3)
		{
			var middle = w[4];
			*dp = Interpolation.Mix2To1To1(middle, w[p0a2], w[p0a3]);
			*(dp + 1) = Interpolation.Mix2To1To1(middle, w[p1a2], w[p1a3]);
			*(dp + dpL) = Interpolation.Mix2To1To1(middle, w[p2a2], w[p2a3]);
			*(dp + dpL + 1) = Interpolation.Mix2To1To1(middle, w[p3a2], w[p3a3]);
		}
	}
}
