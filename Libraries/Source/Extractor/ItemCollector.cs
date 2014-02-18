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
    using System.IO;
    using System.Linq;

    using CellAO.Core.Items;

    using Extractor_Serializer;

    using Playfields;

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

        private string version = "unknown";

        /// <summary>
        /// </summary>
        /// <param name="rdbPath">
        /// </param>
        public ItemCollector(string rdbPath)
        {
            this.extractor = new Extractor(rdbPath);
            if (File.Exists(Path.Combine(rdbPath, "..", "..", "..", "version.id")))
            {
                TextReader tr = new StreamReader(Path.Combine(rdbPath, "..", "..", "..", "version.id"));
                this.version = tr.ReadLine();
                tr.Close();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public int CollectItems()
        {
            List<ItemTemplate> items = new List<ItemTemplate>(this.extractor.GetRecordInstanceCount(1000020));
            NewParser parser = new NewParser();
            List<string> nameList = new List<string>(this.extractor.GetRecordInstanceCount(1000020));

            // Items = 10000020
            foreach (int recnum in this.extractor.GetRecordInstances(1000020))
            {
                ItemTemplate itt = parser.ParseItem(1000020, recnum, this.extractor.GetRecordData(1000020, recnum));
                if (!string.IsNullOrEmpty(itt.ItemName))
                {
                    nameList.Add(itt.ItemName);
                }

                items.Add(itt);
            }

            nameList.Sort();

            for (int i = nameList.Count - 1; i >= 1; i--)
            {
                if (nameList[i] == nameList[i - 1])
                {
                    nameList.RemoveAt(i);
                }
            }

            List<int> itemsIconIds = new List<int>(50000);
            foreach (ItemTemplate item in items)
            {
                if (!itemsIconIds.Contains(item.getItemAttribute(79)))
                {
                    itemsIconIds.Add(item.getItemAttribute(79));
                }
            }

            Dictionary<int, byte[]> icons = new Dictionary<int, byte[]>(this.extractor.GetRecordInstanceCount(1010008));

            // Icons = 1010008
            foreach (int recnum in this.extractor.GetRecordInstances(1010008))
            {
                {
                    // Commented it because it wouldnt get mission icons
                    // if (itemsIconIds.Contains(recnum))
                    icons.Add(recnum, this.extractor.GetRecordData(1010008, recnum));
                }
            }

            Dictionary<int, PlayfieldData> playfields = new Dictionary<int, PlayfieldData>(1000);
            foreach (int recnum in this.extractor.GetRecordInstances(1000001))
            {
                playfields.Add(
                    recnum, 
                    PlayfieldParser.ParsePlayfield(this.extractor.GetRecordData(1000001, recnum), recnum));
            }

            MessagePackZip.CompressData("icons.dat", version, icons);
            MessagePackZip.CompressData("items.dat", version, items);
            MessagePackZip.CompressData("playfields.dat", version, playfields);
            MessagePackZip.CompressData("itemnames.dat", version, nameList);
            return items.Count();
        }

        #endregion
    }
}