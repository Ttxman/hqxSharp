﻿using System.IO;
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
