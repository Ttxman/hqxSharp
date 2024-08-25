/*
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Versioning;
using hqx;
using hqxSharp.Bitmaps;

// Disable warnings for features introduced in later versions of C#
#pragma warning disable CC0048 // Use string interpolation instead of String.Format
#pragma warning disable U2U1104 // Do not use composite formatting to concatenate strings

namespace HqxSharpTest
{
    [SupportedOSPlatform("windows")]
    internal sealed class TestBench
	{
		private string m_strImageDir;
		private TestBenchDisplay m_objDisplay;

		public TestBench() { }

		public TestBench(string imageDirectory, TestBenchDisplay display)
		{
			this.ImageDirectory = imageDirectory;
			this.Display = display;
		}

		public string ImageDirectory
		{
			get { return m_strImageDir; }
			set {
				if (String.IsNullOrEmpty(value)) {
#pragma warning disable RECS0143 // Cannot resolve symbol in text argument
					throw new ArgumentNullException("ImageDirectory");
				} else if (value.IndexOfAny(Path.GetInvalidPathChars()) >= 0) {
#pragma warning disable CC0002 // Invalid argument name
					throw new ArgumentException("This supplied value is not a valid path because it contains illegal characters.", "ImageDirectory");
#pragma warning restore CC0002 // Invalid argument name
				} else if (!Directory.Exists(value)) {
					throw new DirectoryNotFoundException("The image directory " + value + " does not exist.");
				} else {
					m_strImageDir = Path.GetFullPath(value);
				}
			}
		}

		public TestBenchDisplay Display
		{
			get { return m_objDisplay; }
			set {
				if (value == null) {
					throw new ArgumentNullException("Display");
#pragma warning restore RECS0143 // Cannot resolve symbol in text argument
				}
				m_objDisplay = value;
			}
		}

		public void Run()
		{
			var display = this.Display;
			display.Info("Pointer size is {0} bytes, HqxSharpParameters {1} bytes", IntPtr.Size, HqxSharpParameters.Size);
			var strImageDirectory = this.ImageDirectory;
			display.Info("Processing not recursively " + strImageDirectory);
			var lstImages = FindImages(strImageDirectory);
			var c = lstImages.Count;
			display.Info("Found {0} images in that directory", c);

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
						bmpScaled = HqxSharpBitmap.Scale(bmpSource, 3, HqxSharpParameters.Default);
						tsGlobalScale += DateTime.UtcNow - dtmScaleStart;
						lngGlobalPixels += bmpScaled.Width * bmpScaled.Height;

						display.Draw(bmpScaled);
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
						dtmLastSecond = this.DisplayTimings(i + 1, c, dtmGlobalStart, tsGlobalLoad, tsGlobalScale, lngGlobalSize, lngGlobalPixels);
					}
				}
			} catch (Exception ex) {
				dtmLastSecond = this.DisplayTimings(i, c, dtmGlobalStart, tsGlobalLoad, tsGlobalScale, lngGlobalSize, lngGlobalPixels);
				display.Error(ex is ApplicationException ? ex.Message : ex.ToString());
			} finally {
				display.OnEnd(dtmGlobalStart);
			}
		}

		private static bool RemoveConfigFiles(string path)
		{
			var ext = Path.GetExtension(path);
			return (ext == ".config") || (ext.IndexOf('g') < 0);
		}

		private static List<string> FindImages(string directory)
		{
			var lstImages = new List<string>(Directory.GetFiles(directory, "*.*g*", SearchOption.TopDirectoryOnly));
			lstImages.RemoveAll(RemoveConfigFiles);
			lstImages.Sort();
			return lstImages;
		}

		private DateTime DisplayTimings(int done, int count, DateTime dtmGlobalStart, TimeSpan tsGlobalLoad, TimeSpan tsGlobalScale, long lngGlobalSize, long lngGlobalPixels)
		{
			var dtmLastSecond = DateTime.UtcNow;
			var f64ScaleSeconds = tsGlobalScale.TotalSeconds;
			var f64SinceStart = (double)(dtmLastSecond - dtmGlobalStart).Ticks;
			this.Display.Progress("{0,5}/{1,5} {2,3}% done   Avg {3,4:#0.0}% load {4,4:#0.0}% hq3x {5,5:n1}kiB/s {6,4:n1}FPS {7:n2}Mpx/s",
			 done, count, 100 * done / count,
			 100 * tsGlobalLoad.Ticks / f64SinceStart,
			 100 * tsGlobalScale.Ticks / f64SinceStart,
			 lngGlobalSize * (1 / 1024.0) / f64ScaleSeconds,
			 done / f64ScaleSeconds,
			 lngGlobalPixels * 0.000001 / f64ScaleSeconds);
			return dtmLastSecond;
		}
	}
}
