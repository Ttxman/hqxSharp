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
using System.Windows.Forms;

namespace HqxSharpTest
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			this.InitializeComponent();
			this.Shown += this.Form1_Shown;
		}

		private void Form1_Shown(object sender, EventArgs e)
		{
			using (var display = new FormBenchDisplay(this)) {
				var bench = new TestBench(Program.ImageDirectory, display);
				bench.Run();
			}
		}
	}
}
