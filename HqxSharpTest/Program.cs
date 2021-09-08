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
using System.Drawing;
using System.IO;
using hqx;

namespace HqxSharpTest
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine("Pointer size is {0}", IntPtr.Size);
			var strImageDirectory = (args.Length > 0) ? args[0] : Environment.CurrentDirectory;
			Console.WriteLine("Will process directory {0} not recursively", strImageDirectory);
			var strarImages = Directory.GetFiles(strImageDirectory, "*.*g*", SearchOption.TopDirectoryOnly);
			Array.Sort(strarImages);
			Console.WriteLine("Found {0} images in that directory", strarImages.Length);

			var dtmGlobalStart = DateTime.UtcNow;
			var tsGlobalLoad = TimeSpan.Zero;
			var tsGlobalScale = TimeSpan.Zero;
			var dtmLastSecond = DateTime.UtcNow;
			long lngGlobalSize = 0;
			long lngGlobalPixels = 0;

			for (int i = 0; i < strarImages.Length; i++) {
				var strImage = strarImages[i];
				lngGlobalSize += new FileInfo(strImage).Length;
				var dtmImageStart = DateTime.UtcNow;
				using (var bmpSource = new Bitmap(strImage)) {
					tsGlobalLoad += DateTime.UtcNow - dtmImageStart;
					var dtmScaleStart = DateTime.UtcNow;
					var bmpScaled = HqxSharp.Scale(bmpSource, 3, HqxSharpParameters.Default);
					tsGlobalScale += DateTime.UtcNow - dtmScaleStart;
					lngGlobalPixels += bmpScaled.Width * bmpScaled.Height;
				}
				if (((DateTime.UtcNow - dtmLastSecond).TotalSeconds >= 1) || (i >= strarImages.Length - 1)) {
					dtmLastSecond = DateTime.UtcNow;
					var lngSinceStart = (DateTime.UtcNow - dtmGlobalStart).Ticks;
					var f64ScaleSeconds = tsGlobalScale.TotalSeconds;
					Console.WriteLine("{0,5}/{1,5} {2:##0}% done  Avg {3:##0.0}% load {4:##0.0}% scale {5:n1}kiB/s {6:n1}FPS {7:n3}Mpx/s",
					 i + 1, strarImages.Length, (100 * (i + 1)) / strarImages.Length,
					 100.0 * tsGlobalLoad.Ticks / lngSinceStart,
					 100.0 * tsGlobalScale.Ticks / lngSinceStart,
					 lngGlobalSize / (1024.0 * f64ScaleSeconds),
					 (i + 1) / f64ScaleSeconds,
					 lngGlobalPixels * 0.000001 / f64ScaleSeconds);
				}
			}
			Console.WriteLine("Press Enter to exit...");
			Console.ReadLine();
		}
	}
}
