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
using System.Drawing;

// Disable warnings for features introduced in later versions of C#
#pragma warning disable CC0048 // Use string interpolation instead of String.Format

namespace HqxSharpTest
{
	internal abstract class TestBenchDisplay
	{
		public abstract void Info(string message);

		public abstract void Error(string message);

		public abstract void Progress(string message);

		public abstract void Draw(Bitmap scaled);

		public abstract void OnEnd(DateTime globalStart);

		protected static string FormatEndMessage(DateTime globalStart)
		{
			return String.Format("Ended after {1:g} of processing{0}GC memory: {2:n1}MiB before collection, {3:n1}MiB after",
			 Environment.NewLine, DateTime.UtcNow - globalStart,
			 GC.GetTotalMemory(false) / (1024.0 * 1024.0), GC.GetTotalMemory(true) / (1024.0 * 1024.0));
		}

		public void Info(string format, object arg0)
		{
			this.Info(String.Format(format, arg0));
		}

		public void Info(string format, params object[] args)
		{
			this.Info(String.Format(format, args));
		}

		public void Progress(string format, params object[] args)
		{
			this.Progress(String.Format(format, args));
		}
	}
}
