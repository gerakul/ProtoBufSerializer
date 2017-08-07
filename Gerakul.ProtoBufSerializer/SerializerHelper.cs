using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerakul.ProtoBufSerializer
{
    internal static class SerializerHelper
    {
        public static void Reverse(byte[] bytes)
        {
            for (int first = 0, last = bytes.Length - 1; first < last; first++, last--)
            {
                byte temp = bytes[first];
                bytes[first] = bytes[last];
                bytes[last] = temp;
            }
        }
    }
}
