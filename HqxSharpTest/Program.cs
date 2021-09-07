using System;
using System.Drawing;
using System.IO;
using Hiend3D;

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
