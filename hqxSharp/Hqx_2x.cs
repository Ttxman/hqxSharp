﻿/*
 * Copyright © 2003 Maxim Stepin (maxst@hiend3d.com)
 * 
 * Copyright © 2010 Cameron Zemek (grom@zeminvaders.net)
 * 
 * Copyright © 2011, 2012 Tamme Schichler (tamme.schichler@googlemail.com)
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

using System.Drawing;

namespace hqx
{
	public static partial class HqxSharp
	{
		/// <summary>
		/// This is the extended C# port of the hq2x algorithm.
		/// <para>The image is scaled to twice its size.</para>
		/// </summary>
		/// <param name="bitmap">The source image.</param>
		/// <param name="parameters">Thresholds and wrapping options for scaling.</param>
		/// <returns>A new Bitmap instance that contains the source image scaled to twice its size.</returns>
		public static unsafe Bitmap Scale2(Bitmap bitmap, HqxSharpParameters parameters)
		{
			return Scale(bitmap, 2, Scale2, parameters);
		}

		public static unsafe Bitmap Scale2(Bitmap bitmap)
		{
			return Scale(bitmap, 2, Scale2, HqxSharpParameters.Default);
		}

		private static unsafe void Scale2(uint* sp, uint* dp, int Xres, int Yres, uint trY, uint trU, uint trV, uint trA, bool wrapX, bool wrapY)
		{
			//Don't shift trA, as it uses shift right instead of a mask for comparisons.
			trY <<= 2 * 8;
			trU <<= 1 * 8;
			var dpL = Xres * 2;

			int prevline, nextline;
			var w = new uint[9];

			for (var j = 0; j < Yres; j++) {
				prevline = j > 0 ? -Xres : wrapY ? Xres * (Yres - 1) : 0;
				nextline = j < Yres - 1 ? Xres : wrapY ? -(Xres * (Yres - 1)) : 0;
				for (var i = 0; i < Xres; i++) {
					PrepareWorkPixels(sp, Xres, wrapX, prevline, nextline, w, i);
					var pattern = FindPattern(trY, trU, trV, trA, w);

#pragma warning disable CC0120 // Your Switch maybe include default clause
					switch (pattern) {
						case 0:
						case 1:
						case 4:
						case 32:
						case 128:
						case 5:
						case 132:
						case 160:
						case 33:
						case 129:
						case 36:
						case 133:
						case 164:
						case 161:
						case 37:
						case 165: {
								All4Mix2To1To1(dp, dpL, w, 3, 1, 1, 5, 7, 3, 5, 7);
								break;
							}
						case 2:
						case 34:
						case 130:
						case 162: {
								All4Mix2To1To1(dp, dpL, w, 0, 3, 2, 5, 7, 3, 5, 7);
								break;
							}
						case 16:
						case 17:
						case 48:
						case 49: {
								All4Mix2To1To1(dp, dpL, w, 3, 1, 2, 1, 7, 3, 8, 7);
								break;
							}
						case 64:
						case 65:
						case 68:
						case 69: {
								All4Mix2To1To1(dp, dpL, w, 3, 1, 1, 5, 6, 3, 8, 5);
								break;
							}
						case 8:
						case 12:
						case 136:
						case 140: {
								All4Mix2To1To1(dp, dpL, w, 0, 1, 1, 5, 6, 7, 5, 7);
								break;
							}
						case 3:
						case 35:
						case 131:
						case 163: {
								// TODO : Make smaller calls
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 6:
						case 38:
						case 134:
						case 166: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 20:
						case 21:
						case 52:
						case 53: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 144:
						case 145:
						case 176:
						case 177: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 192:
						case 193:
						case 196:
						case 197: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 96:
						case 97:
						case 100:
						case 101: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 40:
						case 44:
						case 168:
						case 172: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 9:
						case 13:
						case 137:
						case 141: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 18:
						case 50: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[2]) : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 80:
						case 81: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[8]) : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 72:
						case 76: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[6]) : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 10:
						case 138: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[0]) : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 66: {
								All4Mix2To1To1(dp, dpL, w, 0, 3, 2, 5, 6, 3, 8, 5);
								break;
							}
						case 24: {
								All4Mix2To1To1(dp, dpL, w, 0, 1, 2, 1, 6, 7, 8, 7);
								break;
							}
						case 7:
						case 39:
						case 135: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 148:
						case 149:
						case 180: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 224:
						case 228:
						case 225: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 41:
						case 169:
						case 45: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 22:
						case 54: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 208:
						case 209: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 104:
						case 108: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 11:
						case 139: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 19:
						case 51: {
								if (Diff(w[1], w[5], trY, trU, trV, trA)) {
									*dp = Interpolation.Mix3To1(w[4], w[3]);
									*(dp + 1) = Interpolation.Mix3To1(w[4], w[2]);
								} else {
									*dp = Interpolation.Mix5To2To1(w[4], w[1], w[3]);
									*(dp + 1) = Interpolation.Mix2To3To3(w[4], w[1], w[5]);
								}
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 146:
						case 178: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								if (Diff(w[1], w[5], trY, trU, trV, trA)) {
									*(dp + 1) = Interpolation.Mix3To1(w[4], w[2]);
									*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								} else {
									*(dp + 1) = Interpolation.Mix2To3To3(w[4], w[1], w[5]);
									*(dp + dpL + 1) = Interpolation.Mix5To2To1(w[4], w[5], w[7]);
								}
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								break;
							}
						case 84:
						case 85: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								if (Diff(w[5], w[7], trY, trU, trV, trA)) {
									*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
									*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[8]);
								} else {
									*(dp + 1) = Interpolation.Mix5To2To1(w[4], w[5], w[1]);
									*(dp + dpL + 1) = Interpolation.Mix2To3To3(w[4], w[5], w[7]);
								}
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								break;
							}
						case 112:
						case 113: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								if (Diff(w[5], w[7], trY, trU, trV, trA)) {
									*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
									*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[8]);
								} else {
									*(dp + dpL) = Interpolation.Mix5To2To1(w[4], w[7], w[3]);
									*(dp + dpL + 1) = Interpolation.Mix2To3To3(w[4], w[5], w[7]);
								}
								break;
							}
						case 200:
						case 204: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								if (Diff(w[7], w[3], trY, trU, trV, trA)) {
									*(dp + dpL) = Interpolation.Mix3To1(w[4], w[6]);
									*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								} else {
									*(dp + dpL) = Interpolation.Mix2To3To3(w[4], w[7], w[3]);
									*(dp + dpL + 1) = Interpolation.Mix5To2To1(w[4], w[7], w[5]);
								}
								break;
							}
						case 73:
						case 77: {
								if (Diff(w[7], w[3], trY, trU, trV, trA)) {
									*dp = Interpolation.Mix3To1(w[4], w[1]);
									*(dp + dpL) = Interpolation.Mix3To1(w[4], w[6]);
								} else {
									*dp = Interpolation.Mix5To2To1(w[4], w[3], w[1]);
									*(dp + dpL) = Interpolation.Mix2To3To3(w[4], w[7], w[3]);
								}
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 42:
						case 170: {
								if (Diff(w[3], w[1], trY, trU, trV, trA)) {
									*dp = Interpolation.Mix3To1(w[4], w[0]);
									*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								} else {
									*dp = Interpolation.Mix2To3To3(w[4], w[3], w[1]);
									*(dp + dpL) = Interpolation.Mix5To2To1(w[4], w[3], w[7]);
								}
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 14:
						case 142: {
								if (Diff(w[3], w[1], trY, trU, trV, trA)) {
									*dp = Interpolation.Mix3To1(w[4], w[0]);
									*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								} else {
									*dp = Interpolation.Mix2To3To3(w[4], w[3], w[1]);
									*(dp + 1) = Interpolation.Mix5To2To1(w[4], w[1], w[5]);
								}
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 67: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 70: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 28: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 152: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 194: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 98: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 56: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 25: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 26:
						case 31: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 82:
						case 214: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 88:
						case 248: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 74:
						case 107: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 27: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[2]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 86: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[8]);
								break;
							}
						case 216: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[6]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 106: {
								*dp = Interpolation.Mix3To1(w[4], w[0]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 30: {
								*dp = Interpolation.Mix3To1(w[4], w[0]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 210: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[2]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 120: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[8]);
								break;
							}
						case 75: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[6]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 29: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 198: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 184: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 99: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 57: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 71: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 156: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 226: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 60: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 195: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 102: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 153: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 58: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[0]) : Interpolation.Mix6To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[2]) : Interpolation.Mix6To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 83: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[2]) : Interpolation.Mix6To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[8]) : Interpolation.Mix6To1To1(w[4], w[5], w[7]);
								break;
							}
						case 92: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[6]) : Interpolation.Mix6To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[8]) : Interpolation.Mix6To1To1(w[4], w[5], w[7]);
								break;
							}
						case 202: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[0]) : Interpolation.Mix6To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[6]) : Interpolation.Mix6To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 78: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[0]) : Interpolation.Mix6To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[6]) : Interpolation.Mix6To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 154: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[0]) : Interpolation.Mix6To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[2]) : Interpolation.Mix6To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 114: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[2]) : Interpolation.Mix6To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[8]) : Interpolation.Mix6To1To1(w[4], w[5], w[7]);
								break;
							}
						case 89: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[6]) : Interpolation.Mix6To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[8]) : Interpolation.Mix6To1To1(w[4], w[5], w[7]);
								break;
							}
						case 90: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[0]) : Interpolation.Mix6To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[2]) : Interpolation.Mix6To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[6]) : Interpolation.Mix6To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[8]) : Interpolation.Mix6To1To1(w[4], w[5], w[7]);
								break;
							}
						case 55:
						case 23: {
								if (Diff(w[1], w[5], trY, trU, trV, trA)) {
									*dp = Interpolation.Mix3To1(w[4], w[3]);
									*(dp + 1) = w[4];
								} else {
									*dp = Interpolation.Mix5To2To1(w[4], w[1], w[3]);
									*(dp + 1) = Interpolation.Mix2To3To3(w[4], w[1], w[5]);
								}
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 182:
						case 150: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								if (Diff(w[1], w[5], trY, trU, trV, trA)) {
									*(dp + 1) = w[4];
									*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								} else {
									*(dp + 1) = Interpolation.Mix2To3To3(w[4], w[1], w[5]);
									*(dp + dpL + 1) = Interpolation.Mix5To2To1(w[4], w[5], w[7]);
								}
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								break;
							}
						case 213:
						case 212: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								if (Diff(w[5], w[7], trY, trU, trV, trA)) {
									*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
									*(dp + dpL + 1) = w[4];
								} else {
									*(dp + 1) = Interpolation.Mix5To2To1(w[4], w[5], w[1]);
									*(dp + dpL + 1) = Interpolation.Mix2To3To3(w[4], w[5], w[7]);
								}
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								break;
							}
						case 241:
						case 240: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								if (Diff(w[5], w[7], trY, trU, trV, trA)) {
									*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
									*(dp + dpL + 1) = w[4];
								} else {
									*(dp + dpL) = Interpolation.Mix5To2To1(w[4], w[7], w[3]);
									*(dp + dpL + 1) = Interpolation.Mix2To3To3(w[4], w[5], w[7]);
								}
								break;
							}
						case 236:
						case 232: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								if (Diff(w[7], w[3], trY, trU, trV, trA)) {
									*(dp + dpL) = w[4];
									*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								} else {
									*(dp + dpL) = Interpolation.Mix2To3To3(w[4], w[7], w[3]);
									*(dp + dpL + 1) = Interpolation.Mix5To2To1(w[4], w[7], w[5]);
								}
								break;
							}
						case 109:
						case 105: {
								if (Diff(w[7], w[3], trY, trU, trV, trA)) {
									*dp = Interpolation.Mix3To1(w[4], w[1]);
									*(dp + dpL) = w[4];
								} else {
									*dp = Interpolation.Mix5To2To1(w[4], w[3], w[1]);
									*(dp + dpL) = Interpolation.Mix2To3To3(w[4], w[7], w[3]);
								}
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 171:
						case 43: {
								if (Diff(w[3], w[1], trY, trU, trV, trA)) {
									*dp = w[4];
									*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								} else {
									*dp = Interpolation.Mix2To3To3(w[4], w[3], w[1]);
									*(dp + dpL) = Interpolation.Mix5To2To1(w[4], w[3], w[7]);
								}
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 143:
						case 15: {
								if (Diff(w[3], w[1], trY, trU, trV, trA)) {
									*dp = w[4];
									*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								} else {
									*dp = Interpolation.Mix2To3To3(w[4], w[3], w[1]);
									*(dp + 1) = Interpolation.Mix5To2To1(w[4], w[1], w[5]);
								}
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 124: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[8]);
								break;
							}
						case 203: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[6]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 62: {
								*dp = Interpolation.Mix3To1(w[4], w[0]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 211: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[2]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 118: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[8]);
								break;
							}
						case 217: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[6]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 110: {
								*dp = Interpolation.Mix3To1(w[4], w[0]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 155: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[2]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 188: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 185: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 61: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 157: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 103: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 227: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 230: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 199: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 220: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[6]) : Interpolation.Mix6To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 158: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[0]) : Interpolation.Mix6To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 234: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[0]) : Interpolation.Mix6To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 242: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[2]) : Interpolation.Mix6To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 59: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[2]) : Interpolation.Mix6To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 121: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[8]) : Interpolation.Mix6To1To1(w[4], w[5], w[7]);
								break;
							}
						case 87: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[8]) : Interpolation.Mix6To1To1(w[4], w[5], w[7]);
								break;
							}
						case 79: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[6]) : Interpolation.Mix6To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 122: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[0]) : Interpolation.Mix6To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[2]) : Interpolation.Mix6To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[8]) : Interpolation.Mix6To1To1(w[4], w[5], w[7]);
								break;
							}
						case 94: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[0]) : Interpolation.Mix6To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[6]) : Interpolation.Mix6To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[8]) : Interpolation.Mix6To1To1(w[4], w[5], w[7]);
								break;
							}
						case 218: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[0]) : Interpolation.Mix6To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[2]) : Interpolation.Mix6To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[6]) : Interpolation.Mix6To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 91: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[2]) : Interpolation.Mix6To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[6]) : Interpolation.Mix6To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[8]) : Interpolation.Mix6To1To1(w[4], w[5], w[7]);
								break;
							}
						case 229: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 167: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 173: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 181: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 186: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[0]) : Interpolation.Mix6To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[2]) : Interpolation.Mix6To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 115: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[2]) : Interpolation.Mix6To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[8]) : Interpolation.Mix6To1To1(w[4], w[5], w[7]);
								break;
							}
						case 93: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[6]) : Interpolation.Mix6To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[8]) : Interpolation.Mix6To1To1(w[4], w[5], w[7]);
								break;
							}
						case 206: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[0]) : Interpolation.Mix6To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[6]) : Interpolation.Mix6To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 205:
						case 201: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[6]) : Interpolation.Mix6To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 174:
						case 46: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[0]) : Interpolation.Mix6To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 179:
						case 147: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[2]) : Interpolation.Mix6To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 117:
						case 116: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? Interpolation.Mix3To1(w[4], w[8]) : Interpolation.Mix6To1To1(w[4], w[5], w[7]);
								break;
							}
						case 189: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 231: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 126: {
								*dp = Interpolation.Mix3To1(w[4], w[0]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[8]);
								break;
							}
						case 219: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[2]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[6]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 125: {
								if (Diff(w[7], w[3], trY, trU, trV, trA)) {
									*dp = Interpolation.Mix3To1(w[4], w[1]);
									*(dp + dpL) = w[4];
								} else {
									*dp = Interpolation.Mix5To2To1(w[4], w[3], w[1]);
									*(dp + dpL) = Interpolation.Mix2To3To3(w[4], w[7], w[3]);
								}
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[8]);
								break;
							}
						case 221: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								if (Diff(w[5], w[7], trY, trU, trV, trA)) {
									*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
									*(dp + dpL + 1) = w[4];
								} else {
									*(dp + 1) = Interpolation.Mix5To2To1(w[4], w[5], w[1]);
									*(dp + dpL + 1) = Interpolation.Mix2To3To3(w[4], w[5], w[7]);
								}
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[6]);
								break;
							}
						case 207: {
								if (Diff(w[3], w[1], trY, trU, trV, trA)) {
									*dp = w[4];
									*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								} else {
									*dp = Interpolation.Mix2To3To3(w[4], w[3], w[1]);
									*(dp + 1) = Interpolation.Mix5To2To1(w[4], w[1], w[5]);
								}
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[6]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 238: {
								*dp = Interpolation.Mix3To1(w[4], w[0]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								if (Diff(w[7], w[3], trY, trU, trV, trA)) {
									*(dp + dpL) = w[4];
									*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								} else {
									*(dp + dpL) = Interpolation.Mix2To3To3(w[4], w[7], w[3]);
									*(dp + dpL + 1) = Interpolation.Mix5To2To1(w[4], w[7], w[5]);
								}
								break;
							}
						case 190: {
								*dp = Interpolation.Mix3To1(w[4], w[0]);
								if (Diff(w[1], w[5], trY, trU, trV, trA)) {
									*(dp + 1) = w[4];
									*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								} else {
									*(dp + 1) = Interpolation.Mix2To3To3(w[4], w[1], w[5]);
									*(dp + dpL + 1) = Interpolation.Mix5To2To1(w[4], w[5], w[7]);
								}
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 187: {
								if (Diff(w[3], w[1], trY, trU, trV, trA)) {
									*dp = w[4];
									*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								} else {
									*dp = Interpolation.Mix2To3To3(w[4], w[3], w[1]);
									*(dp + dpL) = Interpolation.Mix5To2To1(w[4], w[3], w[7]);
								}
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[2]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 243: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[2]);
								if (Diff(w[5], w[7], trY, trU, trV, trA)) {
									*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
									*(dp + dpL + 1) = w[4];
								} else {
									*(dp + dpL) = Interpolation.Mix5To2To1(w[4], w[7], w[3]);
									*(dp + dpL + 1) = Interpolation.Mix2To3To3(w[4], w[5], w[7]);
								}
								break;
							}
						case 119: {
								if (Diff(w[1], w[5], trY, trU, trV, trA)) {
									*dp = Interpolation.Mix3To1(w[4], w[3]);
									*(dp + 1) = w[4];
								} else {
									*dp = Interpolation.Mix5To2To1(w[4], w[1], w[3]);
									*(dp + 1) = Interpolation.Mix2To3To3(w[4], w[1], w[5]);
								}
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[8]);
								break;
							}
						case 237:
						case 233: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 175:
						case 47: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 183:
						case 151: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 245:
						case 244: {
								*dp = Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[5], w[7]);
								break;
							}
						case 250: {
								*dp = Interpolation.Mix3To1(w[4], w[0]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[2]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 123: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[2]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[8]);
								break;
							}
						case 95: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[6]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[8]);
								break;
							}
						case 222: {
								*dp = Interpolation.Mix3To1(w[4], w[0]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[6]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 252: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[5], w[7]);
								break;
							}
						case 249: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[1]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 235: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix2To1To1(w[4], w[2], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 111: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[5]);
								break;
							}
						case 63: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix2To1To1(w[4], w[8], w[7]);
								break;
							}
						case 159: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 215: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix2To1To1(w[4], w[6], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 246: {
								*dp = Interpolation.Mix2To1To1(w[4], w[0], w[3]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[5], w[7]);
								break;
							}
						case 254: {
								*dp = Interpolation.Mix3To1(w[4], w[0]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[5], w[7]);
								break;
							}
						case 253: {
								*dp = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[1]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[5], w[7]);
								break;
							}
						case 251: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[2]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 239: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Interpolation.Mix3To1(w[4], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[5]);
								break;
							}
						case 127: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[8]);
								break;
							}
						case 191: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[7]);
								*(dp + dpL + 1) = Interpolation.Mix3To1(w[4], w[7]);
								break;
							}
						case 223: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[6]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix2To1To1(w[4], w[5], w[7]);
								break;
							}
						case 247: {
								*dp = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Interpolation.Mix3To1(w[4], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[5], w[7]);
								break;
							}
						case 255: {
								*dp = Diff(w[3], w[1], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[3], w[1]);
								*(dp + 1) = Diff(w[1], w[5], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[1], w[5]);
								*(dp + dpL) = Diff(w[7], w[3], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[7], w[3]);
								*(dp + dpL + 1) = Diff(w[5], w[7], trY, trU, trV, trA) ? w[4] : Interpolation.Mix14To1To1(w[4], w[5], w[7]);
								break;
							}
					}
					sp++;
					dp += 2;
				}
				dp += dpL;
			}
		}
	}
}
