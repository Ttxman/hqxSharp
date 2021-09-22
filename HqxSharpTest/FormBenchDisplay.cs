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
using System.Windows.Forms;

// Disable warnings for features introduced in later versions of C#
#pragma warning disable CA1507 // Use nameof to express symbol names

namespace HqxSharpTest
{
	internal sealed class FormBenchDisplay : TestBenchDisplay, IDisposable
	{
		private readonly Form m_frmWindow;
		private Graphics m_gfxForm;
		private bool m_blnDisposed; // To detect redundant calls

		public FormBenchDisplay(Form window)
		{
			if (window == null) {
				throw new ArgumentNullException("window");
			}
			m_frmWindow = window;
		}

		public override void Info(string message)
		{
			Console.WriteLine(message);
		}

		public override void Error(string message)
		{
			this.ShowModal(MessageBoxIcon.Error, "Error", message);
		}

		public override void Progress(string message)
		{
			m_frmWindow.Text = message;
		}

		public override void Draw(Bitmap scaled)
		{
			var frmThis = m_frmWindow;

			// Ensure the window is correctly sized
			var szForm = frmThis.ClientSize;
			if ((szForm.Width < scaled.Width) || (szForm.Height < scaled.Height)) {
				frmThis.ClientSize = scaled.Size;
				// Dispose the old graphics for the old window size
				if (m_gfxForm != null) {
					m_gfxForm.Dispose();
					m_gfxForm = null;
				}
			}

			// Ensure our graphics object is usable
			if (m_blnDisposed) {
				throw new ObjectDisposedException("FormBenchDisplay");
			} else if (m_gfxForm == null) {
				m_gfxForm = frmThis.CreateGraphics();
			}

			// Draw the picture
			m_gfxForm.DrawImageUnscaled(scaled, 0, 0);

			// Yield time for the Windows Forms event loop
			Application.DoEvents();
		}

		public override void OnEnd(DateTime globalStart)
		{
			this.ShowModal(MessageBoxIcon.Information, "Completed", FormatEndMessage(globalStart));
			m_frmWindow.Close();
		}

		private void ShowModal(MessageBoxIcon icon, string caption, string message)
		{
			MessageBox.Show(m_frmWindow, message, caption, MessageBoxButtons.OK, icon);
		}

		public void Dispose()
		{
			if (m_gfxForm != null) {
				m_gfxForm.Dispose();
			}
			m_blnDisposed = true;
		}
	}
}
