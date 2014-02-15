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
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    #endregion

    /// <summary>
    /// </summary>
    public class ItemIcon
    {
        #region Static Fields

        /// <summary>
        /// </summary>
        public static ItemIcon instance;

        #endregion

        #region Fields

        /// <summary>
        /// </summary>
        private Dictionary<int, byte[]> iconDictionary = null;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        static ItemIcon()
        {
            instance = new ItemIcon();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="iconId">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// </exception>
        public Image Get(int iconId)
        {
            if (this.iconDictionary == null)
            {
                this.Read("icons.dat");
            }

            if (!this.iconDictionary.ContainsKey(iconId))
            {
                throw new IndexOutOfRangeException("Icon Id not found: " + iconId);
            }

            Bitmap bmp = new Bitmap(new MemoryStream(this.iconDictionary[iconId]));
            bmp.MakeTransparent(Color.FromArgb(0, 255, 0));
            return bmp;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public int GetRandomIconId()
        {
            Random rnd = new Random((int)(DateTime.Now.Ticks - DateTime.MinValue.Ticks));
            int zz = rnd.Next(this.iconDictionary.Count);
            foreach (int z in this.iconDictionary.Keys)
            {
                zz--;
                if (zz > 0)
                {
                    continue;
                }

                return z;
            }

            return -1;
        }

        /// <summary>
        /// </summary>
        /// <param name="filename">
        /// </param>
        /// <returns>
        /// </returns>
        public int Read(string filename)
        {
            this.iconDictionary = MessagePackZip.UncompressData<int, byte[]>(filename);
            return this.iconDictionary.Count;
        }

        #endregion
    }
}