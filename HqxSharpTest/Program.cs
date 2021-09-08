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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using hqx;

namespace HqxSharpTest
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine("Pointer size is {0} bytes", IntPtr.Size);
			var strImageDirectory = (args.Length > 0) ? Path.GetFullPath(args[0]) : Environment.CurrentDirectory;
			Console.WriteLine("Processing directory {0} NOT recursively", strImageDirectory);
			var lstImages = new List<string>(Directory.GetFiles(strImageDirectory, "*.*g*", SearchOption.TopDirectoryOnly));
			lstImages.RemoveAll(x => Path.GetExtension(x) == ".config");
			lstImages.Sort();
			var c = lstImages.Count;
			Console.WriteLine("Found {0} images in that directory", c);

			var dtmGlobalStart = DateTime.UtcNow;
			var tsGlobalLoad = TimeSpan.Zero;
			var tsGlobalScale = TimeSpan.Zero;
			var dtmLastSecond = DateTime.UtcNow;
			long lngGlobalSize = 0;
			long lngGlobalPixels = 0;
			var i = 0;
			try {
				for (i = 0; i < c; i++) {
					var strImage = lstImages[i];
					lngGlobalSize += new FileInfo(strImage).Length;
					var dtmImageStart = DateTime.UtcNow;
					Bitmap bmpSource = null, bmpScaled = null;
					try {
						bmpSource = new Bitmap(strImage);
						tsGlobalLoad += DateTime.UtcNow - dtmImageStart;
						var dtmScaleStart = DateTime.UtcNow;
						bmpScaled = HqxSharp.Scale(bmpSource, 3, HqxSharpParameters.Default);
						tsGlobalScale += DateTime.UtcNow - dtmScaleStart;
						lngGlobalPixels += bmpScaled.Width * bmpScaled.Height;
					} catch (ArgumentException excArg) {
						throw new ApplicationException(String.Format("Cannot load {0}, its {1} is not valid.", Path.GetFileName(strImage), excArg.ParamName), excArg);
					} finally {
						if (bmpSource != null) {
							bmpSource.Dispose();
						}
						if (bmpScaled != null) {
							bmpScaled.Dispose();
						}
					}

					if (((DateTime.UtcNow - dtmLastSecond).TotalSeconds >= 1) || (i >= c - 1)) {
						dtmLastSecond = DisplayTimings(i + 1, c, dtmGlobalStart, tsGlobalLoad, tsGlobalScale, lngGlobalSize, lngGlobalPixels);
					}
				}
			} catch (Exception ex) {
				dtmLastSecond = DisplayTimings(i, c, dtmGlobalStart, tsGlobalLoad, tsGlobalScale, lngGlobalSize, lngGlobalPixels);
				Console.Error.WriteLine(ex is ApplicationException ? ex.Message : ex.ToString());
			} finally {
				Console.WriteLine("Press Enter to exit...");
				Console.ReadLine();
			}
		}

		private static DateTime DisplayTimings(int done, int count, DateTime dtmGlobalStart, TimeSpan tsGlobalLoad, TimeSpan tsGlobalScale, long lngGlobalSize, long lngGlobalPixels)
		{
			var dtmLastSecond = DateTime.UtcNow;
			var lngSinceStart = (dtmLastSecond - dtmGlobalStart).Ticks;
			var f64ScaleSeconds = tsGlobalScale.TotalSeconds;
			Console.WriteLine("{0,5}/{1,5} {2,3}% done    Avg {3:##0.0}% load {4:##0.0}% hq3x {5:n1}kiB/s {6:n1}FPS {7:n3}Mpx/s",
			 done, count, (100 * done) / count,
			 100.0 * tsGlobalLoad.Ticks / lngSinceStart,
			 100.0 * tsGlobalScale.Ticks / lngSinceStart,
			 lngGlobalSize / (1024.0 * f64ScaleSeconds),
			 done / f64ScaleSeconds,
			 lngGlobalPixels * 0.000001 / f64ScaleSeconds);
			return dtmLastSecond;
		}
	}
}
