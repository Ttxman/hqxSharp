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

#define Graphical
using System;
using System.IO;
#if (Graphical)
using System.Windows.Forms;
#endif

namespace HqxSharpTest
{
	internal static class Program
	{
		internal static string ImageDirectory { get; private set; }

#if (Graphical)
		[STAThread]
#endif
		private static void Main(string[] args)
		{
			var strImageDirectory = (args.Length > 0) ? Path.GetFullPath(args[0]) : Environment.CurrentDirectory;

#if (Graphical)
			Program.ImageDirectory = strImageDirectory;

			// Standard Windows Forms application initialization. Test bench will be run in the Shown event of the form.
			Application.SetCompatibleTextRenderingDefault(false);
			using (var form1 = new Form1()) {
				Application.Run(form1);
			}
			Application.DoEvents();
#else
			new TestBench(strImageDirectory, new ConsoleBenchDisplay()).Run();
#endif
		}
	}
}
