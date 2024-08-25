/*
 * Copyright © 2003 Maxim Stepin (maxst@hiend3d.com)
 *
 * Copyright © 2010 Cameron Zemek (grom@zeminvaders.net)
 *
 * Copyright © 2011, 2012 Tamme Schichler (tamme.schichler@googlemail.com)
 *
 * Copyright © 2020 René Rhéaume (repzilon@users.noreply.github.com)
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

using System.Runtime.CompilerServices;

namespace hqx
{
	/// <summary>
	/// Contains the color-blending operations used internally by hqx.
	/// </summary>
	internal static class Interpolation
	{
		// 1572 calls :  288 in 2x, 756 in 3x,  528 in 4x
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Mix3To1(uint c1, uint c2)
		{
			return MixColours(3, 1, c1, c2);
		}

        //  676 calls :  316 in 2x, 180 in 3x,  180 in 4x
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Mix2To1To1(uint c1, uint c2, uint c3)
		{
			return MixColours(2, 1, 1, c1, c2, c3);
		}

        //  760 calls : NONE in 2x, 120 in 3x,  640 in 4x
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Mix7To1(uint c1, uint c2)
		{
			return MixColours(7, 1, c1, c2);
		}

        //   72 calls : NONE in 2x,  72 in 3x, NONE in 4x
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Mix2To7To7(uint c1, uint c2, uint c3)
		{
			return MixColours(2, 7, 7, c1, c2, c3);
		}

        /*
		From 2xSaI source
		In 15bpp,
			colorMask		is 0x7BDE or binary 0111 1011 1101 1110, i.e. 3 sequences of 11110
			lowPixelMask	is 0x0421 or binary 0000 0100 0010 0001, i.e. 3 sequences of 00001, complement of the above

		The function below works with two 15/16 bits pixels packed in a 32 bits variable
		static inline uint32 INTERPOLATE(uint32 A, uint32 B)
		{
			if (A != B) {
				return ( ((A & colorMask) >> 1) + ((B & colorMask) >> 1) + (A & B & lowPixelMask) );
			} else {
				return A;
			}
		}
		*/

        //  264 calls : NONE in 2x,  24 in 3x,  240 in 4x
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MixEven(uint c1, uint c2)
		{
			return MixColours(1, 1, c1, c2);
		}

        //  344 calls :   24 in 2x, NONE in 3x, 320 in 4x
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Mix5To2To1(uint c1, uint c2, uint c3)
		{
			return MixColours(5, 2, 1, c1, c2, c3);
		}

        //  152 calls :   52 in 2x, NONE in 3x, 100 in 4x
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Mix6To1To1(uint c1, uint c2, uint c3)
		{
			return MixColours(6, 1, 1, c1, c2, c3);
		}

        //  664 calls : NONE in 2x, NONE in 3x, 664 in 4x
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Mix5To3(uint c1, uint c2)
		{
			return MixColours(5, 3, c1, c2);
		}

        //   24 calls :   24 in 2x, NONE in 3x, NONE in 4x
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Mix2To3To3(uint c1, uint c2, uint c3)
		{
			return MixColours(2, 3, 3, c1, c2, c3);
		}

        //   28 calls :   28 in 2x, NONE in 3x, NONE in 4x
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Mix14To1To1(uint c1, uint c2, uint c3)
		{
			return MixColours(14, 1, 1, c1, c2, c3);
		}

        // This method can overflow between blue and red and from red to nothing when the sum of all weightings is higher than 255.
        // It only works for weightings with a sum that is a power of two, otherwise the blue value is corrupted.
        // Parameters: weighting0, weighting1[, ...], colour0, colour1[, ...]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MixColours(uint w1, uint w2, uint c1, uint c2)
		{
			if (c2 == c1) {
				return c1;
			} else {
				uint totalPartsColour = 0;
				uint totalGreen = 0;
				uint totalRedBlue = 0;
				uint totalAlpha = 0;

				Accumulate(w1, c1, ref totalPartsColour, ref totalGreen, ref totalRedBlue, ref totalAlpha);
				Accumulate(w2, c2, ref totalPartsColour, ref totalGreen, ref totalRedBlue, ref totalAlpha);
				return Reduce(totalPartsColour, w1 + w2, totalGreen, totalRedBlue, totalAlpha);
			}
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MixColours(uint w1, uint w2, uint w3, uint c1, uint c2, uint c3)
		{
			if ((c2 == c1) && (c3 == c1)) {
				return c1;
			} else {
				uint totalPartsColour = 0;
				uint totalGreen = 0;
				uint totalRedBlue = 0;
				uint totalAlpha = 0;

				Accumulate(w1, c1, ref totalPartsColour, ref totalGreen, ref totalRedBlue, ref totalAlpha);
				Accumulate(w2, c2, ref totalPartsColour, ref totalGreen, ref totalRedBlue, ref totalAlpha);
				Accumulate(w3, c3, ref totalPartsColour, ref totalGreen, ref totalRedBlue, ref totalAlpha);
				return Reduce(totalPartsColour, w1 + w2 + w3, totalGreen, totalRedBlue, totalAlpha);
			}
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Accumulate(uint weighting, uint colour, ref uint totalPartsColour, ref uint totalGreen, ref uint totalRedBlue, ref uint totalAlpha)
		{
			const uint MaskGreen = 0x0000ff00;
			const uint MaskRedBlue = 0x00ff00ff;
			const int AlphaShift = 24;

			var alpha = (colour >> AlphaShift) * weighting;
			if (alpha != 0) {
				totalAlpha += alpha;

				totalPartsColour += weighting;
				totalGreen += (colour & MaskGreen) * weighting;
				totalRedBlue += (colour & MaskRedBlue) * weighting;
			}
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Reduce(uint totalPartsColour, uint totalPartsAlpha, uint totalGreen, uint totalRedBlue, uint totalAlpha)
		{
			const uint MaskGreen = 0x0000ff00;
			const uint MaskRedBlue = 0x00ff00ff;
			const int AlphaShift = 24;

			var alpha2 = (totalAlpha / totalPartsAlpha) << AlphaShift;

			if (totalPartsColour > 0) {
				totalGreen = (totalGreen / totalPartsColour) & MaskGreen;
				totalRedBlue = (totalRedBlue / totalPartsColour) & MaskRedBlue;
			}

			return alpha2 | totalGreen | totalRedBlue;
		}
	}
}
