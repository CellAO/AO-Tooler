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

namespace Extractor
{
    #region Usings ...

    using System.Collections.Generic;
    using System.Linq;

    using CellAO.Core.Items;

    using Extractor_Serializer;

    using Utility;

    #endregion

    /// <summary>
    /// </summary>
    public class ItemCollector
    {
        #region Fields

        /// <summary>
        /// </summary>
        private Extractor extractor;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="rdbPath">
        /// </param>
        public ItemCollector(string rdbPath)
        {
            this.extractor = new Extractor(rdbPath);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public int CollectItems()
        {
            List<ItemTemplate> items = new List<ItemTemplate>();
            NewParser parser = new NewParser();

            // Items = 10000020
            foreach (int recnum in this.extractor.GetRecordInstances(1000020))
            {
                items.Add(parser.ParseItem(1000020, recnum, this.extractor.GetRecordData(1000020, recnum)));
            }

            List<int> itemsIconIds = new List<int>();
            foreach (ItemTemplate item in items)
            {
                if (!itemsIconIds.Contains(item.getItemAttribute(79)))
                {
                    itemsIconIds.Add(item.getItemAttribute(79));
                }
            }

            Dictionary<int, byte[]> icons = new Dictionary<int, byte[]>();

            // Icons = 1010008
            foreach (int recnum in this.extractor.GetRecordInstances(1010008))
            {
                if (itemsIconIds.Contains(recnum))
                {
                    icons.Add(recnum, this.extractor.GetRecordData(1010008, recnum));
                }
            }

            MessagePackZip.CompressData("icons.dat", string.Empty, icons);
            MessagePackZip.CompressData("items.dat", string.Empty, items);
            return items.Count();
        }

        #endregion
    }
}