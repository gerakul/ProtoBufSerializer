using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerakul.ProtoBufSerializer
{
    public class MessageReadData
    {
        public IList<int> ReadFieldNums { get; private set; }
        public IList<uint> UnknownTags { get; private set; }

        public MessageReadData(IList<int> readFieldNums, IList<uint> unknownTags)
        {
            this.ReadFieldNums = readFieldNums;
            this.UnknownTags = unknownTags;
        }
    }
}
