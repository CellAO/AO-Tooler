#region License

// Copyright (c) 2005-2014, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

namespace Utility
{
    #region Usings ...

    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    #endregion

    /// <summary>
    /// </summary>
    public static class AOData
    {
        #region Static Fields

        /// <summary>
        /// </summary>
        public static Process AOProcess = null;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        public static void Click(int x, int y)
        {
            if (AOProcess == null)
            {
                return;
            }

            Point oldPos = Cursor.Position;
            MoveMouseTo(x, y);

            mouse_event(0x02, 0, 0, 0, UIntPtr.Zero);
            mouse_event(0x04, 0, 0, 0, UIntPtr.Zero);
            Cursor.Position = oldPos;
        }

        /// <summary>
        /// </summary>
        /// <param name="p">
        /// </param>
        public static void Click(Point p)
        {
            Click(p.X, p.Y);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public static Point GetAOCursorPosition()
        {
            Point p = new Point(0, 0);
            GetCursorPos(out p);
            ScreenToClient(AOProcess.MainWindowHandle, ref p);
            return p;
        }

        /// <summary>
        /// </summary>
        /// <param name="lpPoint">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        /// <summary>
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        public static void MoveMouseTo(int x, int y)
        {
            Point p = new Point(x, y);
            ClientToScreen(AOProcess.MainWindowHandle, ref p);
            Cursor.Position = p;
        }

        /// <summary>
        /// </summary>
        /// <param name="p">
        /// </param>
        public static void MoveMouseTo(Point p)
        {
            MoveMouseTo(p.X, p.Y);
        }

        /// <summary>
        /// </summary>
        /// <param name="hWnd">
        /// </param>
        /// <param name="wMsg">
        /// </param>
        /// <param name="wParam">
        /// </param>
        /// <param name="lParam">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="hWnd">
        /// </param>
        /// <param name="lpPoint">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        /// <summary>
        /// </summary>
        /// <param name="hWnd">
        /// </param>
        /// <param name="lpPoint">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

        /// <summary>
        /// </summary>
        /// <param name="dwFlags">
        /// </param>
        /// <param name="dx">
        /// </param>
        /// <param name="dy">
        /// </param>
        /// <param name="dwData">
        /// </param>
        /// <param name="dwExtraInfo">
        /// </param>
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        #endregion
    }
}