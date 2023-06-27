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
using System.IO;
using System.Text.RegularExpressions;

namespace HqxTerser
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			for (byte scale = 2; scale <= 4; scale++) {
				var strFilename = "Hqx_" + scale + "x.cs";
				var strHqxOrig = File.ReadAllText(strFilename);
				var strHqxCopy = strHqxOrig;

				strHqxCopy = Regex.Replace(strHqxCopy, @"\*dp = Interpolation.Mix2To1To1\(w\[4\], w\[(\d)\], w\[(\d)\]\);\s+\*\(dp \+ 1\) = Interpolation.Mix2To1To1\(w\[4\], w\[(\d)\], w\[(\d)\]\);\s+\*\(dp \+ dpL\) = Interpolation.Mix2To1To1\(w\[4\], w\[(\d)\], w\[(\d)\]\);\s+\*\(dp \+ dpL \+ 1\) = Interpolation.Mix2To1To1\(w\[4\], w\[(\d)\], w\[(\d)\]\);", @"All4Mix2To1To1(dp, dpL, w, $1, $2, $3, $4, $5, $6, $7, $8);");
				strHqxCopy = Regex.Replace(strHqxCopy, @"(\W)w[[]4[]](\W)", "$1middle$2").Replace("middle = middle;", "middle = w[4];"); ;

				if (strHqxCopy != strHqxOrig) {
					File.WriteAllText(strFilename, strHqxCopy);
				}
			}
		}
	}
}
