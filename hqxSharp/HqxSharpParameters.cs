/*
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
using System.Runtime.InteropServices;

namespace hqx
{
	[StructLayout(LayoutKind.Explicit)]
	public struct HqxSharpParameters : IEquatable<HqxSharpParameters>
	{
		public static readonly unsafe int Size = sizeof(HqxSharpParameters);

		public static readonly HqxSharpParameters Default = Create(0x00300706);

		[FieldOffset(0)]
		public uint AllThresholds;

		/// <summary>The Y (luminance) threshold.</summary>
		[FieldOffset(1)]
		public byte LumaThreshold;

		/// <summary>The U (chrominance) threshold.</summary>
		[FieldOffset(2)]
		public byte BlueishThreshold;

		/// <summary>The V (chrominance) threshold.</summary>
		[FieldOffset(3)]
		public byte ReddishThreshold;

		/// <summary>The A (transparency) threshold.</summary>
		[FieldOffset(0)]
		public byte AlphaThreshold;

		/// <summary>Used for images that can be seamlessly repeated horizontally.</summary>
		[FieldOffset(4)]
		public bool WrapX;

		/// <summary>Used for images that can be seamlessly repeated vertically.</summary>
		[FieldOffset(5)]
		public bool WrapY;

		public static HqxSharpParameters Create(uint thresholds, bool wrapX = false, bool wrapY = false)
		{
			var parameters = new HqxSharpParameters();
			parameters.AllThresholds = thresholds;
			parameters.WrapX = wrapX;
			parameters.WrapY = wrapY;
			return parameters;
		}

		public static HqxSharpParameters Create(byte luma = 48, byte u = 7, byte v = 6, byte alpha = 0, bool wrapX = false, bool wrapY = false)
		{
			var parameters = new HqxSharpParameters();
			parameters.LumaThreshold = luma;
			parameters.BlueishThreshold = u;
			parameters.ReddishThreshold = v;
			parameters.AlphaThreshold = alpha;
			parameters.WrapX = wrapX;
			parameters.WrapY = wrapY;
			return parameters;
		}

		public override bool Equals(object obj)
		{
			return obj is HqxSharpParameters && this.Equals((HqxSharpParameters)obj);
		}

		public bool Equals(HqxSharpParameters other)
		{
			return (AllThresholds == other.AllThresholds) &&
			 (WrapX == other.WrapX) &&
			 (WrapY == other.WrapY);
		}

		public override int GetHashCode()
		{
			return (int)(AllThresholds |
			 (WrapX ? 1u : 0u) << 15 |
			 (WrapY ? 1u : 0u) << 7);
		}

		public static bool operator ==(HqxSharpParameters left, HqxSharpParameters right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(HqxSharpParameters left, HqxSharpParameters right)
		{
			return !(left == right);
		}

		public override string ToString()
		{
			return String.Format("Thresholds: {{ Y:{0}, U:{1}, V:{2}, A:{3} }}, Wrap: {{ X:{4}, Y:{5} }}",
			 LumaThreshold, BlueishThreshold, ReddishThreshold, AlphaThreshold,
			 WrapX, WrapY);
		}
	}
}
