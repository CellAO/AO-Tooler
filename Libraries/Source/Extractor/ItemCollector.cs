using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractor
{
    using System.ComponentModel;

    using CellAO.Core.Items;

    using Extractor_Serializer;

    public class ItemCollector
    {
        private Extractor extractor;

        public ItemCollector(string rdbPath)
        {
                extractor = new Extractor(rdbPath);
        }

        public int CollectItems()
        {
            List<ItemTemplate> items = new List<ItemTemplate>();
            NewParser parser = new NewParser();
            // Items = 10000020
            foreach (int recnum in extractor.GetRecordInstances(1000020))
            {
                items.Add(parser.ParseItem(1000020, recnum, extractor.GetRecordData(1000020, recnum)));
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
            foreach (int recnum in extractor.GetRecordInstances(1010008))
            {
                if (itemsIconIds.Contains(recnum))
                {
                    icons.Add(recnum, extractor.GetRecordData(1010008, recnum));
                }
            }

            Utility.MessagePackZip.CompressData("icons.dat", "", icons);
            Utility.MessagePackZip.CompressData("items.dat", "", items);
            return items.Count();
        }
    }
}
