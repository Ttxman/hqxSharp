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

using System.Runtime.InteropServices;

namespace hqx
{
	[StructLayout(LayoutKind.Auto)]
	public struct HqxSharpParameters
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
	}
}
