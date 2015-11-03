using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerakul.ProtoBufSerializer
{
    public static class WireFormat
    {
        private const int TagTypeBits = 3;
        private const uint TagTypeMask = (1 << TagTypeBits) - 1;
        private const int MaxFieldNumber = int.MaxValue >> 3; // ???? really?

        public static WireType GetTagWireType(uint tag)
        {
            return (WireType)(tag & TagTypeMask);
        }

        public static int GetTagFieldNumber(uint tag)
        {
            return (int)tag >> TagTypeBits;
        }

        public static uint MakeTag(int fieldNumber, WireType wireType)
        {
            return (uint)(fieldNumber << TagTypeBits) | (uint)wireType;
        }

        public static byte[] GetTagBytes(uint tag)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BasicSerializer bs = new BasicSerializer(ms);
                bs.WriteUInt32(tag);

                ms.Flush();

                return ms.ToArray();
            }
        }

        public static byte[] GetTagBytes(int fieldNumber, WireType wireType)
        {
            return GetTagBytes(MakeTag(fieldNumber, wireType));
        }

        public static bool CheckFieldNumber(int fieldNumber)
        {
            return fieldNumber <= MaxFieldNumber;
        }
    }

    public enum WireType : uint
    {
        Varint = 0,
        Fixed64 = 1,
        LengthDelimited = 2,
        StartGroup = 3,      // obsolete
        EndGroup = 4,        // obsolete
        Fixed32 = 5
    }
}
