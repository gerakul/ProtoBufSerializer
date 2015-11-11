using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerakul.ProtoBufSerializer
{
    public class BasicDeserializer
    {
        internal Stream stream;
        private int recursionDepth = 0;
        private byte[] buffForSeek = new byte[4096];

        public BasicDeserializer(Stream stream)
        {
            this.stream = stream;
        }

        private static int DecodeZigZag32(uint n)
        {
            return (int)(n >> 1) ^ -(int)(n & 1);
        }

        private static long DecodeZigZag64(ulong n)
        {
            return (long)(n >> 1) ^ -(long)(n & 1);
        }

        private void ReadFromStream(byte[] buff, int offset, int count)
        {
            int num = stream.Read(buff, offset, count);
            while (num < count)
            {
                if (num == 0)
                {
                    throw new EndOfStreamException();
                }

                offset += num;
                count -= num;
                num = stream.Read(buff, offset, count);
            }
        }

        private byte ReadByteFromStream()
        {
            int x = stream.ReadByte();
            if (x == -1)
            {
                throw new EndOfStreamException();
            }

            return (byte)x;
        }

        private uint ReadRawLittleEndian32()
        {
            uint b1 = ReadByteFromStream();
            uint b2 = ReadByteFromStream();
            uint b3 = ReadByteFromStream();
            uint b4 = ReadByteFromStream();
            return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24);
        }

        private ulong ReadRawLittleEndian64()
        {
            ulong b1 = ReadByteFromStream();
            ulong b2 = ReadByteFromStream();
            ulong b3 = ReadByteFromStream();
            ulong b4 = ReadByteFromStream();
            ulong b5 = ReadByteFromStream();
            ulong b6 = ReadByteFromStream();
            ulong b7 = ReadByteFromStream();
            ulong b8 = ReadByteFromStream();
            return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24)
                   | (b5 << 32) | (b6 << 40) | (b7 << 48) | (b8 << 56);
        }

        private uint ReadRawVarint32(bool zeroIfEndOfStream = false)
        {
            int tmp;
            if (zeroIfEndOfStream)
            {
                tmp = stream.ReadByte();

                if (tmp == -1)
                {
                    return 0;
                }
            }
            else
            {
                tmp = ReadByteFromStream();
            }

            if (tmp < 128)
            {
                return (uint)tmp;
            }

            int result = tmp & 0x7f;
            if ((tmp = ReadByteFromStream()) < 128)
            {
                result |= tmp << 7;
            }
            else
            {
                result |= (tmp & 0x7f) << 7;
                if ((tmp = ReadByteFromStream()) < 128)
                {
                    result |= tmp << 14;
                }
                else
                {
                    result |= (tmp & 0x7f) << 14;
                    if ((tmp = ReadByteFromStream()) < 128)
                    {
                        result |= tmp << 21;
                    }
                    else
                    {
                        result |= (tmp & 0x7f) << 21;
                        result |= (tmp = ReadByteFromStream()) << 28;
                        if (tmp >= 128)
                        {
                            // Discard upper 32 bits
                            for (int i = 0; i < 5; i++)
                            {
                                if (ReadByteFromStream() < 128)
                                {
                                    return (uint)result;
                                }
                            }

                            throw InvalidProtocolBufferException.MalformedVarint();
                        }
                    }
                }
            }
            return (uint)result;
        }

        private ulong ReadRawVarint64()
        {
            int shift = 0;
            ulong result = 0;
            while (shift < 64)
            {
                byte b = ReadByteFromStream();
                result |= (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    return result;
                }
                shift += 7;
            }

            throw InvalidProtocolBufferException.MalformedVarint();
        }

        public void SkipField(WireType wireType)
        {
            switch (wireType)
            {
                case WireType.Varint:
                    int tmp;
                    int cnt = 0;
                    while ((tmp = ReadByteFromStream()) >= 128)
                    {
                        cnt++;

                        if (cnt == 10)
                        {
                            throw InvalidProtocolBufferException.MalformedVarint();
                        }
                    }

                    break;
                case WireType.Fixed64:
                    ReadFromStream(buffForSeek, 0, 8);

                    break;
                case WireType.LengthDelimited:
                    var len = ReadLength();
                    if (len <= buffForSeek.Length)
                    {
                        ReadFromStream(buffForSeek, 0, len);
                    }
                    else
                    {
                        ReadFromStream(new byte[len], 0, len);
                    }

                    break;
                case WireType.StartGroup:
                    recursionDepth++;
                    uint tag;
                    WireType wt;
                    do
                    {
                        tag = ReadTag();
                        if (tag == 0)
                        {
                            throw InvalidProtocolBufferException.TruncatedMessage();
                        }

                        wt = WireFormat.GetTagWireType(tag);
                        SkipField(wt);
                    } while (wt != WireType.EndGroup);
                    recursionDepth--;

                    break;
                case WireType.EndGroup:
                    break;
                case WireType.Fixed32:
                    ReadFromStream(buffForSeek, 0, 4);
                    break;
                default:
                    throw InvalidProtocolBufferException.UnknownWireType();
                    break;
            }
        }

        public uint ReadTag()
        {
            return ReadRawVarint32(true);
        }

        public int ReadLength(bool zeroIfEndOfStream = false)
        {
            return (int)ReadRawVarint32(zeroIfEndOfStream);
        }

        public double ReadDouble()
        {
            return BitConverter.Int64BitsToDouble((long)ReadRawLittleEndian64());
        }

        public float ReadFloat()
        {
            byte[] buff = new byte[4];
            ReadFromStream(buff, 0, buff.Length);

            if (!BitConverter.IsLittleEndian)
            {
                SerializerHelper.Reverse(buff);
            }

            return BitConverter.ToSingle(buff, 0);
        }

        public int ReadInt32()
        {
            return (int)ReadRawVarint32();
        }

        public long ReadInt64()
        {
            return (long)ReadRawVarint64();
        }

        public uint ReadUInt32()
        {
            return ReadRawVarint32();
        }

        public ulong ReadUInt64()
        {
            return ReadRawVarint64();
        }

        public int ReadSInt32()
        {
            return DecodeZigZag32(ReadRawVarint32());
        }

        public long ReadSInt64()
        {
            return DecodeZigZag64(ReadRawVarint64());
        }

        public uint ReadFixed32()
        {
            return ReadRawLittleEndian32();
        }

        public ulong ReadFixed64()
        {
            return ReadRawLittleEndian64();
        }

        public int ReadSFixed32()
        {
            return (int)ReadRawLittleEndian32();
        }

        public long ReadSFixed64()
        {
            return (long)ReadRawLittleEndian64();
        }

        public bool ReadBool()
        {
            return ReadRawVarint32() != 0;
        }

        public string ReadString()
        {
            int length = ReadLength();

            if (length == 0)
            {
                return "";
            }

            byte[] buff = new byte[length];
            ReadFromStream(buff, 0, buff.Length);
            return Encoding.UTF8.GetString(buff, 0, buff.Length);
        }

        public byte[] ReadBytes()
        {
            int length = ReadLength();
            byte[] buff = new byte[length];
            ReadFromStream(buff, 0, buff.Length);

            return buff;
        }

        public int ReadEnum()
        {
            return (int)ReadRawVarint32();
        }
    }
}
