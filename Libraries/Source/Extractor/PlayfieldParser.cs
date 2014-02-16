using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractor
{
    using System.IO;

    using Playfields;

    public class PlayfieldParser
    {
        public static PlayfieldData ParsePlayfield(byte[] data, int pfId)
        {
            PlayfieldData pf= new PlayfieldData();
            pf.Name = ParseName(data);
            pf.Id = pfId;
            return pf;
        }

        public static string ParseName(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);
            br.ReadInt32();
            br.ReadInt32();
            string name = string.Empty;
            byte c = 0;
            while ((c = br.ReadByte()) != 0)
            {
                name += (char)c;
            }

            br.Close();
            ms.Close();
            return name;
   
        }
    }
}
