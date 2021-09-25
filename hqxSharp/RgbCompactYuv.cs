/*
 * Copyright © 2013 René Rhéaume (rene.rheaume@gmail.com)
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
using System.Collections.Generic;

namespace hqx
{
	/// <summary>
	/// Provides a compact cached RGB to YUV lookup without alpha support
	/// </summary>
	/// <remarks>
	/// Only source colors in use are in the lookup table. Each color will take
	/// more memory (it is an hash table entry), but the table will take
	/// less memory than a full array (which is 64 MiB), as you generally use hqx
	/// for pixel art and small non-photographic images with a low color count.
	/// </remarks>
	public static class RgbCompactYuv
	{
		private static readonly IDictionary<uint, int> s_dicRgbYuv =
		 new Dictionary<uint, int>();

		[CLSCompliant(false)]
		public static int GetYuv(uint rgb)
		{
			const uint RgbMask = 0x00ffffff;

			rgb &= RgbMask;
			int yuv;
			if (s_dicRgbYuv.TryGetValue(rgb, out yuv)) {
				return yuv;
			} else {
				var r = ((int)rgb & 0xff0000) >> 16;
				var g = ((int)rgb & 0x00ff00) >> 8;
				var b = (int)rgb & 0x0000ff;

				// To prevent a round-trip to the FPU, some fixed-point math is used.
				// RGB values go from 0 to 255. If we were multiplying them by
				// 65793, they would go from 0 to 16777215 (2^24-1)
				// After scaling coefficients (found in RgbYuv), a right shift 
				// is performed to bring numbers back to a 0..255 scale.
				var y = ((+19672 * r) + (38620 * g) + (7500 * b)) >> 16;
				var u = (((-11119 * r) - (21777 * g) + (32896 * b)) >> 16) + 128;
				var v = (((+32896 * r) - (27567 * g) - (5329 * b)) >> 16) + 128;

				yuv = (y << 16) | (u << 8) | v;
				s_dicRgbYuv.Add(rgb, yuv);
				return yuv;
			}
		}

		public static void UnloadLookupTable()
		{
			s_dicRgbYuv.Clear();
		}

		public static bool Initialized
		{
			get { return true; }
		}
	}
}
