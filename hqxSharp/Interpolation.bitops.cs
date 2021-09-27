/*
 * Copyright © 2003 Maxim Stepin (maxst@hiend3d.com)
 *
 * Copyright © 2010 Cameron Zemek (grom@zeminvaders.net)
 * Copyright © 2011 Francois Gannaz (mytskine@gmail.com)
 *
 * Copyright © 2011, 2012 Tamme Schichler (tamme.schichler@googlemail.com)
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

namespace hqx
{
	/// <summary>
	/// Contains the color-blending operations used internally by hqx.
	/// </summary>
	internal static class Interpolation
	{
		public static uint Mix3To1(uint c1, uint c2) // Interp1
		{
			return MixColours(3, 1, c1, c2, 2);
		}

		public static uint Mix2To1To1(uint c1, uint c2, uint c3) // Interp2
		{
			return MixColours(2, 1, 1, c1, c2, c3, 2);
		}

		public static uint Mix7To1(uint c1, uint c2) // Interp3
		{
			return MixColours(7, 1, c1, c2, 3);
		}

		public static uint Mix2To7To7(uint c1, uint c2, uint c3) // Interp4
		{
			return MixColours(2, 7, 7, c1, c2, c3, 4);
		}

		public static uint MixEven(uint c1, uint c2) // Interp5
		{
			return MixColours(1, 1, c1, c2, 1);
		}

		public static uint Mix5To2To1(uint c1, uint c2, uint c3) // Interp6
		{
			return MixColours(5, 2, 1, c1, c2, c3, 3);
		}

		public static uint Mix6To1To1(uint c1, uint c2, uint c3) // Interp7
		{
			return MixColours(6, 1, 1, c1, c2, c3, 3);
		}

		public static uint Mix5To3(uint c1, uint c2) // Interp8
		{
			return MixColours(5, 3, c1, c2, 3);
		}

		public static uint Mix2To3To3(uint c1, uint c2, uint c3) // Interp9
		{
			return MixColours(2, 3, 3, c1, c2, c3, 3);
		}

		public static uint Mix14To1To1(uint c1, uint c2, uint c3) // Interp10
		{
			return MixColours(14, 1, 1, c1, c2, c3, 4);
		}

		private static uint MixColours(uint w1, uint w2, uint c1, uint c2, byte s) // Interpolate_2
		{
			const uint MaskGreen = 0x0000ff00;
			const uint MaskRedBlue = 0x00ff00ff;
			const uint MaskAlpha = 0xff000000;

			if (c1 == c2) {
				return c1;
			}
			return
			(((((c1 & MaskAlpha) >> 24) * w1 + ((c2 & MaskAlpha) >> 24) * w2) << (24 - s)) & MaskAlpha) +
			((((c1 & MaskGreen) * w1 + (c2 & MaskGreen) * w2) >> s) & MaskGreen) +
			((((c1 & MaskRedBlue) * w1 + (c2 & MaskRedBlue) * w2) >> s) & MaskRedBlue);
		}

		private static uint MixColours(uint w1, uint w2, uint w3, uint c1, uint c2, uint c3, byte s) // Interpolate_3
		{
			const uint MaskGreen = 0x0000ff00;
			const uint MaskRedBlue = 0x00ff00ff;
			const uint MaskAlpha = 0xff000000;

			return
			(((((c1 & MaskAlpha) >> 24) * w1 + ((c2 & MaskAlpha) >> 24) * w2 + ((c3 & MaskAlpha) >> 24) * w3) << (24 - s)) & MaskAlpha) +
			((((c1 & MaskGreen) * w1 + (c2 & MaskGreen) * w2 + (c3 & MaskGreen) * w3) >> s) & MaskGreen) +
			((((c1 & MaskRedBlue) * w1 + (c2 & MaskRedBlue) * w2 + (c3 & MaskRedBlue) * w3) >> s) & MaskRedBlue);
		}
	}
}
