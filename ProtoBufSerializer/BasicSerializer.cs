using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Gerakul.ProtoBufSerializer
{
    public class BasicSerializer
    {
        internal Stream stream;

        public BasicSerializer(Stream stream)
        {
            this.stream = stream;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint EncodeZigZag32(int n)
        {
            return (uint)((n << 1) ^ (n >> 31));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong EncodeZigZag64(long n)
        {
            return (ulong)((n << 1) ^ (n >> 63));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteRawLittleEndian32(uint value)
        {
            stream.WriteByte((byte)value);
            stream.WriteByte((byte)(value >> 8));
            stream.WriteByte((byte)(value >> 16));
            stream.WriteByte((byte)(value >> 24));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteRawLittleEndian64(ulong value)
        {
            stream.WriteByte((byte)value);
            stream.WriteByte((byte)(value >> 8));
            stream.WriteByte((byte)(value >> 16));
            stream.WriteByte((byte)(value >> 24));
            stream.WriteByte((byte)(value >> 32));
            stream.WriteByte((byte)(value >> 40));
            stream.WriteByte((byte)(value >> 48));
            stream.WriteByte((byte)(value >> 56));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteRawVarint32(uint value)
        {
            while (value > 127)
            {
                stream.WriteByte((byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }

            stream.WriteByte((byte)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteRawVarint64(ulong value)
        {
            while (value > 127)
            {
                stream.WriteByte((byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }

            stream.WriteByte((byte)value);
        }

        public void WriteRawTag(byte[] tag)
        {
            stream.Write(tag, 0, tag.Length);
        }

        public void WriteLength(int length)
        {
            uint v = (uint)length;
            WriteRawVarint32(v);
        }

        public void WriteDouble(double value)
        {
            ulong v = (ulong)BitConverter.DoubleToInt64Bits(value);
            WriteRawLittleEndian64(v);
        }

        public void WriteFloat(float value)
        {
            byte[] buff = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                SerializerHelper.Reverse(buff);
            }

            stream.Write(buff, 0, 4);
        }

        public void WriteInt32(int value)
        {
            if (value >= 0)
            {
                uint v = (uint)value;
                WriteRawVarint32(v);
            }
            else
            {
                ulong v = (ulong)value;
                WriteRawVarint64(v);
            }
        }
        public void WriteInt64(long value)
        {
            ulong v = (ulong)value;
            WriteRawVarint64(v);
        }

        public void WriteUInt32(uint value)
        {
            WriteRawVarint32(value);
        }

        public void WriteUInt64(ulong value)
        {
            WriteRawVarint64(value);
        }

        public void WriteSInt32(int value)
        {
            uint v = EncodeZigZag32(value);
            WriteRawVarint32(v);
        }

        public void WriteSInt64(long value)
        {
            ulong v = EncodeZigZag64(value);
            WriteRawVarint64(v);
        }

        public void WriteFixed32(uint value)
        {
            WriteRawLittleEndian32(value);
        }

        public void WriteFixed64(ulong value)
        {
            WriteRawLittleEndian64(value);
        }

        public void WriteSFixed32(int value)
        {
            uint v = (uint)value;
            WriteRawLittleEndian32(v);
        }

        public void WriteSFixed64(long value)
        {
            ulong v = (ulong)value;
            WriteRawLittleEndian64(v);
        }

        public void WriteBool(bool value)
        {
            stream.WriteByte(value ? (byte)1 : (byte)0);
        }

        public void WriteString(string value)
        {
            byte[] buff = Encoding.UTF8.GetBytes(value);
            WriteLength(buff.Length);
            stream.Write(buff, 0, buff.Length);
        }

        public void WriteBytes(byte[] value)
        {
            WriteLength(value.Length);
            stream.Write(value, 0, value.Length);
        }

        public void WriteEnum(int value)
        {
            WriteInt32(value);
        }
    }
}
