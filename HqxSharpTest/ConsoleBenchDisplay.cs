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

namespace HqxSharpTest
{
	internal sealed class ConsoleBenchDisplay : TestBenchDisplay
	{
		public override void Info(string message)
		{
			Console.WriteLine(message);
		}

		public override void Error(string message)
		{
			Console.Error.WriteLine(message);
		}

		public override void Progress(string message)
		{
			Console.WriteLine(message);
		}

		public override void Draw(Bitmap scaled)
		{
			// Do nothing. You could also write the image on disk here.
		}

		public override void OnEnd(DateTime globalStart)
		{
			Console.WriteLine(base.FormatEndMessage(globalStart));
			Console.WriteLine("Press Enter to exit...");
			Console.ReadLine();
		}
	}
}
