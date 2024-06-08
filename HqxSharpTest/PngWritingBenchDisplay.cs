/*
 * 
 * Copyright © 2024 René Rhéaume (repzilon@users.noreply.github.com)
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

namespace HqxSharpTest
{
	internal sealed class PngWritingBenchDisplay : ConsoleBenchDisplay
	{
		private int i;

		public override void Draw(Bitmap scaled)
		{
			base.Draw(scaled);
			i++;
			var strFilename = String.Format("hq3x-{0:D5}.png", i);
			var strDirName = System.IO.Path.Combine(Program.ImageDirectory, "hq3x");
			var strPath = System.IO.Path.Combine(strDirName, strFilename);
			if (!System.IO.Directory.Exists(strDirName))
			{
				System.IO.Directory.CreateDirectory(strDirName);
			}
			scaled.Save(strPath, System.Drawing.Imaging.ImageFormat.Png);
		}
	}
}
