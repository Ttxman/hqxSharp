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
	[StructLayout(LayoutKind.Auto)]
	public struct HqxSharpParameters : IEquatable<HqxSharpParameters>
	{
		public static readonly HqxSharpParameters Default = Create();

		/// <summary>The Y (luminance) threshold.</summary>
		public uint LumaThreshold { get; set; }

		/// <summary>The U (chrominance) threshold.</summary>
		public uint BlueishThreshold { get; set; }

		/// <summary>The V (chrominance) threshold.</summary>
		public uint ReddishThreshold { get; set; }

		/// <summary>The A (transparency) threshold.</summary>
		public uint AlphaThreshold { get; set; }

		/// <summary>Used for images that can be seamlessly repeated horizontally.</summary>
		public bool WrapX { get; set; }

		/// <summary>Used for images that can be seamlessly repeated vertically.</summary>
		public bool WrapY { get; set; }

		public static HqxSharpParameters Create(uint luma = 48, uint u = 7, uint v = 6, uint alpha = 0, bool wrapX = false, bool wrapY = false)
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
			return (this.LumaThreshold == other.LumaThreshold) &&
			 (this.BlueishThreshold == other.BlueishThreshold) &&
			 (this.ReddishThreshold == other.ReddishThreshold) &&
			 (this.AlphaThreshold == other.AlphaThreshold) &&
			 (this.WrapX == other.WrapX) &&
			 (this.WrapY == other.WrapY);
		}

		public override int GetHashCode()
		{
			return (int)((this.AlphaThreshold << 24) |
			 (this.LumaThreshold << 16) |
			 (this.ReddishThreshold << 8) |
			 this.BlueishThreshold |
			 (this.WrapX ? 1u : 0u) << 15 |
			 (this.WrapY ? 1u : 0u) << 7);
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
			 this.LumaThreshold, this.BlueishThreshold, this.ReddishThreshold, this.AlphaThreshold,
			 this.WrapX, this.WrapY);
		}
	}
}
