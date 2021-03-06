﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gerakul.ProtoBufSerializer
{
    public class FieldSetting<T>
    {
        private static Type[] allowableEnumTypes = { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int) };

        public int FieldNum { get; private set; }
        public uint Tag { get; private set; }
        internal byte[] RawTag { get; private set; }
        public Action<T, BasicSerializer, byte[]> WriteAction { get; private set; }
        public Action<T, BasicDeserializer> ReadActionWithoutTag { get; private set; }
        public Func<T, bool> HasValueFunc { get; private set; }

        public uint AltTag { get; private set; }
        public Action<T, BasicDeserializer> AltReadActionWithoutTag { get; private set; }

        public byte[] GetRawTag()
        {
            byte[] newTag = new byte[RawTag.Length];

            for (int i = 0; i < newTag.Length; i++)
            {
                newTag[i] = RawTag[i];
            }

            return newTag;
        }


        protected FieldSetting(int fieldNum, uint tag,
            Action<T, BasicSerializer, byte[]> writeAction,
            Action<T, BasicDeserializer> readActionWithoutTag,
            Func<T, bool> hasValueFunc,
            uint altTag = 0, Action<T, BasicDeserializer> altReadActionWithoutTag = null)
        {
            this.FieldNum = fieldNum;
            this.Tag = tag;
            this.RawTag = WireFormat.GetTagBytes(tag);
            this.WriteAction = writeAction;
            this.ReadActionWithoutTag = readActionWithoutTag;
            this.HasValueFunc = hasValueFunc;

            this.AltTag = altTag;
            this.AltReadActionWithoutTag = altReadActionWithoutTag;
        }

        #region Factory

        protected static void CheckFieldNum(int fieldNum)
        {
            if (!WireFormat.CheckFieldNumber(fieldNum))
            {
                InvalidProtocolBufferException.TooBigFieldNumber();
            }
        }

        private static void CheckEnumType<E>()
        {
            if (!typeof(E).GetTypeInfo().IsEnum)
            {
                throw new ArgumentException($"Type {typeof(E)} must be Enum");
            }

            if (!Enum.GetUnderlyingType(typeof(E)).Equals(typeof(int)))
            {
                throw new ArgumentException($"Underlying type of enum {typeof(E)} must be {typeof(int)}");
            }
        }

        #region One value

        private static FieldSetting<T> CreateValue(int fieldNum, WireType wireType, Action<T, BasicSerializer, byte[]> writeAction, Action<T, BasicDeserializer> readActionWithoutTag, Func<T, bool> hasValueFunc)
        {
            CheckFieldNum(fieldNum);
            return new FieldSetting<T>(fieldNum, WireFormat.MakeTag(fieldNum, wireType), writeAction, readActionWithoutTag, hasValueFunc);
        }

        public static FieldSetting<T> CreateDouble(int fieldNum, Func<T, double> valueGetter, Action<T, double> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateValue(fieldNum, WireType.Fixed64,
                (value, serializer, rawTag) => { serializer.WriteRawTag(rawTag); serializer.WriteDouble(valueGetter(value)); },
                (value, serializer) => valueSetter(value, serializer.ReadDouble()),
                hasValueFunc);
        }

        public static FieldSetting<T> CreateFloat(int fieldNum, Func<T, float> valueGetter, Action<T, float> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateValue(fieldNum, WireType.Fixed32,
                (value, serializer, rawTag) => { serializer.WriteRawTag(rawTag); serializer.WriteFloat(valueGetter(value)); },
                (value, serializer) => valueSetter(value, serializer.ReadFloat()),
                hasValueFunc);
        }

        public static FieldSetting<T> CreateInt32(int fieldNum, Func<T, int> valueGetter, Action<T, int> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateValue(fieldNum, WireType.Varint,
                (value, serializer, rawTag) => { serializer.WriteRawTag(rawTag); serializer.WriteInt32(valueGetter(value)); },
                (value, serializer) => valueSetter(value, serializer.ReadInt32()),
                hasValueFunc);
        }

        public static FieldSetting<T> CreateInt64(int fieldNum, Func<T, long> valueGetter, Action<T, long> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateValue(fieldNum, WireType.Varint,
                (value, serializer, rawTag) => { serializer.WriteRawTag(rawTag); serializer.WriteInt64(valueGetter(value)); },
                (value, serializer) => valueSetter(value, serializer.ReadInt64()),
                hasValueFunc);
        }

        public static FieldSetting<T> CreateUInt32(int fieldNum, Func<T, uint> valueGetter, Action<T, uint> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateValue(fieldNum, WireType.Varint,
                (value, serializer, rawTag) => { serializer.WriteRawTag(rawTag); serializer.WriteUInt32(valueGetter(value)); },
                (value, serializer) => valueSetter(value, serializer.ReadUInt32()),
                hasValueFunc);
        }

        public static FieldSetting<T> CreateUInt64(int fieldNum, Func<T, ulong> valueGetter, Action<T, ulong> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateValue(fieldNum, WireType.Varint,
                (value, serializer, rawTag) => { serializer.WriteRawTag(rawTag); serializer.WriteUInt64(valueGetter(value)); },
                (value, serializer) => valueSetter(value, serializer.ReadUInt64()), hasValueFunc);
        }

        public static FieldSetting<T> CreateSInt32(int fieldNum, Func<T, int> valueGetter, Action<T, int> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateValue(fieldNum, WireType.Varint,
                (value, serializer, rawTag) => { serializer.WriteRawTag(rawTag); serializer.WriteSInt32(valueGetter(value)); },
                (value, serializer) => valueSetter(value, serializer.ReadSInt32()),
                hasValueFunc);
        }

        public static FieldSetting<T> CreateSInt64(int fieldNum, Func<T, long> valueGetter, Action<T, long> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateValue(fieldNum, WireType.Varint,
                (value, serializer, rawTag) => { serializer.WriteRawTag(rawTag); serializer.WriteSInt64(valueGetter(value)); },
                (value, serializer) => valueSetter(value, serializer.ReadSInt64()),
                hasValueFunc);
        }

        public static FieldSetting<T> CreateFixed32(int fieldNum, Func<T, uint> valueGetter, Action<T, uint> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateValue(fieldNum, WireType.Fixed32,
                (value, serializer, rawTag) => { serializer.WriteRawTag(rawTag); serializer.WriteFixed32(valueGetter(value)); },
                (value, serializer) => valueSetter(value, serializer.ReadFixed32()),
                hasValueFunc);
        }

        public static FieldSetting<T> CreateFixed64(int fieldNum, Func<T, ulong> valueGetter, Action<T, ulong> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateValue(fieldNum, WireType.Fixed64,
                (value, serializer, rawTag) => { serializer.WriteRawTag(rawTag); serializer.WriteFixed64(valueGetter(value)); },
                (value, serializer) => valueSetter(value, serializer.ReadFixed64()),
                hasValueFunc);
        }

        public static FieldSetting<T> CreateSFixed32(int fieldNum, Func<T, int> valueGetter, Action<T, int> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateValue(fieldNum, WireType.Fixed32,
                (value, serializer, rawTag) => { serializer.WriteRawTag(rawTag); serializer.WriteSFixed32(valueGetter(value)); },
                (value, serializer) => valueSetter(value, serializer.ReadSFixed32()),
                hasValueFunc);
        }

        public static FieldSetting<T> CreateSFixed64(int fieldNum, Func<T, long> valueGetter, Action<T, long> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateValue(fieldNum, WireType.Fixed64,
                (value, serializer, rawTag) => { serializer.WriteRawTag(rawTag); serializer.WriteSFixed64(valueGetter(value)); },
                (value, serializer) => valueSetter(value, serializer.ReadSFixed64()),
                hasValueFunc);
        }

        public static FieldSetting<T> CreateBool(int fieldNum, Func<T, bool> valueGetter, Action<T, bool> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateValue(fieldNum, WireType.Varint,
                (value, serializer, rawTag) => { serializer.WriteRawTag(rawTag); serializer.WriteBool(valueGetter(value)); },
                (value, serializer) => valueSetter(value, serializer.ReadBool()),
                hasValueFunc);
        }

        public static FieldSetting<T> CreateEnum<E>(int fieldNum, Func<T, E> valueGetter, Action<T, E> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            CheckEnumType<E>();
            return CreateInt32(fieldNum, x => valueGetter(x).CastTo<int>(), (x, y) => valueSetter(x, y.CastTo<E>()), hasValueFunc);
        }

        public static FieldSetting<T> CreateString(int fieldNum, Func<T, string> valueGetter, Action<T, string> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateValue(fieldNum, WireType.LengthDelimited,
                (value, serializer, rawTag) => { serializer.WriteRawTag(rawTag); serializer.WriteString(valueGetter(value)); },
                (value, serializer) => valueSetter(value, serializer.ReadString()),
                hasValueFunc);
        }

        public static FieldSetting<T> CreateBytes(int fieldNum, Func<T, byte[]> valueGetter, Action<T, byte[]> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateValue(fieldNum, WireType.LengthDelimited,
                (value, serializer, rawTag) => { serializer.WriteRawTag(rawTag); serializer.WriteBytes(valueGetter(value)); },
                (value, serializer) => valueSetter(value, serializer.ReadBytes()),
                hasValueFunc);
        }

        public static FieldSetting<T> CreateMessage<EmbeddedT>(int fieldNum, Func<T, EmbeddedT> valueGetter, Action<T, EmbeddedT> valueSetter,
            MessageDescriptor<EmbeddedT> embeddedDescriptor, Func<T, bool> hasValueFunc = null) where EmbeddedT : new()
        {
            return CreateValue(fieldNum, WireType.LengthDelimited,

                (value, serializer, rt) =>
                {
                    serializer.WriteRawTag(rt);

                    using (var writer = embeddedDescriptor.CreateWriter(serializer.stream))
                    {
                        writer.WriteWithLength(valueGetter(value));
                    }
                },

                (value, serializer) =>
                {
                    using (var reader = embeddedDescriptor.CreateReader(serializer.stream))
                    {
                        valueSetter(value, reader.ReadWithLen());
                    }
                },
                hasValueFunc);
        }

        #endregion

        #region Array

        private static FieldSetting<T> CreateArray<InternalT>(int fieldNum, WireType wireType, Func<T, IEnumerable<InternalT>> valueGetter, Action<T, InternalT> oneValueSetter, 
            Action<T, BasicDeserializer> readActionWithoutTag, Func<T, bool> hasValueFunc,
            Action<BasicSerializer, InternalT> oneValueWriter, Func<BasicDeserializer, InternalT> oneValueReader, 
            bool canReadPacked)
        {
            CheckFieldNum(fieldNum);
            return new FieldSetting<T>(fieldNum, WireFormat.MakeTag(fieldNum, wireType),

                (value, serializer, rt) =>
                {
                    foreach (var item in valueGetter(value))
                    {
                        serializer.WriteRawTag(rt);
                        oneValueWriter(serializer, item);
                    };
                },

                readActionWithoutTag,
                hasValueFunc,
                canReadPacked ? WireFormat.MakeTag(fieldNum, WireType.LengthDelimited) : 0,
                canReadPacked ? (value, serializer) =>
                {
                    var len = serializer.ReadLength();
                    var positionLimit = serializer.stream.Position + len;

                    while (serializer.stream.Position < positionLimit)
                    {
                        oneValueSetter(value, oneValueReader(serializer));
                    }

                    if (serializer.stream.Position > positionLimit)
                    {
                        throw InvalidProtocolBufferException.AllowableFieldLengthWasExceeded();
                    }
                } : (Action<T, BasicDeserializer>)null);
        }

        public static FieldSetting<T> CreateDoubleArray(int fieldNum, Func<T, IEnumerable<double>> valueGetter, Action<T, double> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return CreateArray(fieldNum, WireType.Fixed64, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadDouble()), hasValueFunc, 
                (ser, val) => ser.WriteDouble(val), ser => ser.ReadDouble(), canReadPacked);
        }

        public static FieldSetting<T> CreateFloatArray(int fieldNum, Func<T, IEnumerable<float>> valueGetter, Action<T, float> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return CreateArray(fieldNum, WireType.Fixed32, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadFloat()), hasValueFunc, 
                (ser, val) => ser.WriteFloat(val), ser => ser.ReadFloat(), canReadPacked);
        }

        public static FieldSetting<T> CreateInt32Array(int fieldNum, Func<T, IEnumerable<int>> valueGetter, Action<T, int> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return CreateArray(fieldNum, WireType.Varint, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadInt32()), hasValueFunc, 
                (ser, val) => ser.WriteInt32(val), ser => ser.ReadInt32(), canReadPacked);
        }

        public static FieldSetting<T> CreateInt64Array(int fieldNum, Func<T, IEnumerable<long>> valueGetter, Action<T, long> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return CreateArray(fieldNum, WireType.Varint, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadInt64()), hasValueFunc, 
                (ser, val) => ser.WriteInt64(val), ser => ser.ReadInt64(), canReadPacked);
        }

        public static FieldSetting<T> CreateUInt32Array(int fieldNum, Func<T, IEnumerable<uint>> valueGetter, Action<T, uint> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return CreateArray(fieldNum, WireType.Varint, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadUInt32()), hasValueFunc, 
                (ser, val) => ser.WriteUInt32(val), ser => ser.ReadUInt32(), canReadPacked);
        }

        public static FieldSetting<T> CreateUInt64Array(int fieldNum, Func<T, IEnumerable<ulong>> valueGetter, Action<T, ulong> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return CreateArray(fieldNum, WireType.Varint, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadUInt64()), hasValueFunc, 
                (ser, val) => ser.WriteUInt64(val), ser => ser.ReadUInt64(), canReadPacked);
        }

        public static FieldSetting<T> CreateSInt32Array(int fieldNum, Func<T, IEnumerable<int>> valueGetter, Action<T, int> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return CreateArray(fieldNum, WireType.Varint, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadSInt32()), hasValueFunc, 
                (ser, val) => ser.WriteSInt32(val), ser => ser.ReadSInt32(), canReadPacked);
        }

        public static FieldSetting<T> CreateSInt64Array(int fieldNum, Func<T, IEnumerable<long>> valueGetter, Action<T, long> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return CreateArray(fieldNum, WireType.Varint, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadSInt64()), hasValueFunc, 
                (ser, val) => ser.WriteSInt64(val), ser => ser.ReadSInt64(), canReadPacked);
        }

        public static FieldSetting<T> CreateFixed32Array(int fieldNum, Func<T, IEnumerable<uint>> valueGetter, Action<T, uint> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return CreateArray(fieldNum, WireType.Fixed32, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadFixed32()), hasValueFunc, 
                (ser, val) => ser.WriteFixed32(val), ser => ser.ReadFixed32(), canReadPacked);
        }

        public static FieldSetting<T> CreateFixed64Array(int fieldNum, Func<T, IEnumerable<ulong>> valueGetter, Action<T, ulong> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return CreateArray(fieldNum, WireType.Fixed64, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadFixed64()), hasValueFunc, 
                (ser, val) => ser.WriteFixed64(val), ser => ser.ReadFixed64(), canReadPacked);
        }

        public static FieldSetting<T> CreateSFixed32Array(int fieldNum, Func<T, IEnumerable<int>> valueGetter, Action<T, int> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return CreateArray(fieldNum, WireType.Fixed32, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadSFixed32()), hasValueFunc, 
                (ser, val) => ser.WriteSFixed32(val), ser => ser.ReadSFixed32(), canReadPacked);
        }

        public static FieldSetting<T> CreateSFixed64Array(int fieldNum, Func<T, IEnumerable<long>> valueGetter, Action<T, long> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return CreateArray(fieldNum, WireType.Fixed64, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadSFixed64()), hasValueFunc, 
                (ser, val) => ser.WriteSFixed64(val), ser => ser.ReadSFixed64(), canReadPacked);
        }

        public static FieldSetting<T> CreateBoolArray(int fieldNum, Func<T, IEnumerable<bool>> valueGetter, Action<T, bool> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return CreateArray(fieldNum, WireType.Varint, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadBool()), hasValueFunc, 
                (ser, val) => ser.WriteBool(val), ser => ser.ReadBool(), canReadPacked);
        }

        public static FieldSetting<T> CreateEnumArray<E>(int fieldNum, Func<T, IEnumerable<E>> valueGetter, Action<T, E> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            CheckEnumType<E>();
            return CreateInt32Array(fieldNum, x => valueGetter(x).Cast<int>(), (x, y) => oneValueSetter(x, y.CastTo<E>()), hasValueFunc, canReadPacked);
        }

        public static FieldSetting<T> CreateStringArray(int fieldNum, Func<T, IEnumerable<string>> valueGetter, Action<T, string> oneValueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateArray(fieldNum, WireType.LengthDelimited, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadString()), hasValueFunc, 
                (ser, val) => ser.WriteString(val), ser => ser.ReadString(), false);
        }

        public static FieldSetting<T> CreateBytesArray(int fieldNum, Func<T, IEnumerable<byte[]>> valueGetter, Action<T, byte[]> oneValueSetter, Func<T, bool> hasValueFunc = null)
        {
            return CreateArray(fieldNum, WireType.LengthDelimited, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadBytes()), hasValueFunc, 
                (ser, val) => ser.WriteBytes(val), ser => ser.ReadBytes(), false);
        }

        public static FieldSetting<T> CreateMessageArray<EmbeddedT>(int fieldNum, Func<T, IEnumerable<EmbeddedT>> valueGetter, Action<T, EmbeddedT> oneValueSetter,
            MessageDescriptor<EmbeddedT> embeddedDescriptor, Func<T, bool> hasValueFunc = null) where EmbeddedT : new()
        {
            CheckFieldNum(fieldNum);
            uint tag = WireFormat.MakeTag(fieldNum, WireType.LengthDelimited);
            return new FieldSetting<T>(fieldNum, tag,

                (value, serializer, rt) =>
                {
                    using (var writer = embeddedDescriptor.CreateWriter(serializer.stream))
                    {
                        foreach (var item in valueGetter(value))
                        {
                            serializer.WriteRawTag(rt);
                            writer.WriteWithLength(item);
                        };
                    }
                },

                (value, serializer) =>
                {
                    using (var reader = embeddedDescriptor.CreateReader(serializer.stream))
                    {
                        oneValueSetter(value, reader.ReadWithLen());
                    }
                },
                hasValueFunc);
        }

        #endregion

        #region Packed array

        private static FieldSetting<T> CreatePackedArray<InternalT>(int fieldNum, WireType wireType, Func<T, IEnumerable<InternalT>> valueGetter, Action<T, InternalT> oneValueSetter,
            Action<T, BasicDeserializer> altReadActionWithoutTag, Func<T, bool> hasValueFunc,
            Action<BasicSerializer, InternalT> oneValueWriter, Func<BasicDeserializer, InternalT> oneValueReader,
            bool canReadUnpacked)
        {
            CheckFieldNum(fieldNum);
            return new FieldSetting<T>(fieldNum, WireFormat.MakeTag(fieldNum, WireType.LengthDelimited),

                (value, serializer, rt) =>
                {
                    serializer.WriteRawTag(rt);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        BasicSerializer bs = new BasicSerializer(ms);
                        foreach (var item in valueGetter(value))
                        {
                            oneValueWriter(bs, item);
                        };

                        ms.Flush();

                        var len = (int)ms.Position;
                        serializer.WriteLength(len);
                        ArraySegment<byte> buff;
                        if (ms.TryGetBuffer(out buff))
                        {
                            serializer.stream.Write(buff.Array, 0, len);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unable to get buffer from {nameof(ms)}");
                        }
                    }
                },

                (value, serializer) =>
                {
                    var len = serializer.ReadLength();
                    var positionLimit = serializer.stream.Position + len;

                    while (serializer.stream.Position < positionLimit)
                    {
                        oneValueSetter(value, oneValueReader(serializer));
                    }

                    if (serializer.stream.Position > positionLimit)
                    {
                        throw InvalidProtocolBufferException.AllowableFieldLengthWasExceeded();
                    }
                },

                hasValueFunc,
                canReadUnpacked ? WireFormat.MakeTag(fieldNum, wireType) : 0,
                canReadUnpacked ? altReadActionWithoutTag : null);
        }

        public static FieldSetting<T> CreateDoublePackedArray(int fieldNum, Func<T, IEnumerable<double>> valueGetter, 
            Action<T, double> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return CreatePackedArray(fieldNum, WireType.Fixed64, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadDouble()), 
                hasValueFunc, (ser, val) => ser.WriteDouble(val), ser => ser.ReadDouble(), canReadUnpacked);
        }

        public static FieldSetting<T> CreateFloatPackedArray(int fieldNum, Func<T, IEnumerable<float>> valueGetter, 
            Action<T, float> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return CreatePackedArray(fieldNum, WireType.Fixed32, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadFloat()),
                hasValueFunc, (ser, val) => ser.WriteFloat(val), ser => ser.ReadFloat(), canReadUnpacked);
        }

        public static FieldSetting<T> CreateInt32PackedArray(int fieldNum, Func<T, IEnumerable<int>> valueGetter, 
            Action<T, int> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return CreatePackedArray(fieldNum, WireType.Varint, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadInt32()),
                hasValueFunc, (ser, val) => ser.WriteInt32(val), ser => ser.ReadInt32(), canReadUnpacked);
        }

        public static FieldSetting<T> CreateInt64PackedArray(int fieldNum, Func<T, IEnumerable<long>> valueGetter, 
            Action<T, long> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return CreatePackedArray(fieldNum, WireType.Varint, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadInt64()),
                hasValueFunc, (ser, val) => ser.WriteInt64(val), ser => ser.ReadInt64(), canReadUnpacked);
        }

        public static FieldSetting<T> CreateUInt32PackedArray(int fieldNum, Func<T, IEnumerable<uint>> valueGetter, 
            Action<T, uint> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return CreatePackedArray(fieldNum, WireType.Varint, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadUInt32()),
                hasValueFunc, (ser, val) => ser.WriteUInt32(val), ser => ser.ReadUInt32(), canReadUnpacked);
        }

        public static FieldSetting<T> CreateUInt64PackedArray(int fieldNum, Func<T, IEnumerable<ulong>> valueGetter, 
            Action<T, ulong> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return CreatePackedArray(fieldNum, WireType.Varint, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadUInt64()),
                hasValueFunc, (ser, val) => ser.WriteUInt64(val), ser => ser.ReadUInt64(), canReadUnpacked);
        }

        public static FieldSetting<T> CreateSInt32PackedArray(int fieldNum, Func<T, IEnumerable<int>> valueGetter, 
            Action<T, int> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return CreatePackedArray(fieldNum, WireType.Varint, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadSInt32()),
                hasValueFunc, (ser, val) => ser.WriteSInt32(val), ser => ser.ReadSInt32(), canReadUnpacked);
        }

        public static FieldSetting<T> CreateSInt64PackedArray(int fieldNum, Func<T, IEnumerable<long>> valueGetter, 
            Action<T, long> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return CreatePackedArray(fieldNum, WireType.Varint, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadSInt64()),
                hasValueFunc, (ser, val) => ser.WriteSInt64(val), ser => ser.ReadSInt64(), canReadUnpacked);
        }

        public static FieldSetting<T> CreateFixed32PackedArray(int fieldNum, Func<T, IEnumerable<uint>> valueGetter, 
            Action<T, uint> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return CreatePackedArray(fieldNum, WireType.Fixed32, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadFixed32()),
                hasValueFunc, (ser, val) => ser.WriteFixed32(val), ser => ser.ReadFixed32(), canReadUnpacked);
        }

        public static FieldSetting<T> CreateFixed64PackedArray(int fieldNum, Func<T, IEnumerable<ulong>> valueGetter, 
            Action<T, ulong> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return CreatePackedArray(fieldNum, WireType.Fixed64, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadFixed64()),
                hasValueFunc, (ser, val) => ser.WriteFixed64(val), ser => ser.ReadFixed64(), canReadUnpacked);
        }

        public static FieldSetting<T> CreateSFixed32PackedArray(int fieldNum, Func<T, IEnumerable<int>> valueGetter, 
            Action<T, int> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return CreatePackedArray(fieldNum, WireType.Fixed32, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadSFixed32()),
                hasValueFunc, (ser, val) => ser.WriteSFixed32(val), ser => ser.ReadSFixed32(), canReadUnpacked);
        }

        public static FieldSetting<T> CreateSFixed64PackedArray(int fieldNum, Func<T, IEnumerable<long>> valueGetter, 
            Action<T, long> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return CreatePackedArray(fieldNum, WireType.Fixed64, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadSFixed64()),
                hasValueFunc, (ser, val) => ser.WriteSFixed64(val), ser => ser.ReadSFixed64(), canReadUnpacked);
        }

        public static FieldSetting<T> CreateBoolPackedArray(int fieldNum, Func<T, IEnumerable<bool>> valueGetter, 
            Action<T, bool> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return CreatePackedArray(fieldNum, WireType.Varint, valueGetter, oneValueSetter, (value, serializer) => oneValueSetter(value, serializer.ReadBool()),
                hasValueFunc, (ser, val) => ser.WriteBool(val), ser => ser.ReadBool(), canReadUnpacked);
        }

        public static FieldSetting<T> CreateEnumPackedArray<E>(int fieldNum, Func<T, IEnumerable<E>> valueGetter,
            Action<T, E> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            CheckEnumType<E>();
            return CreateInt32PackedArray(fieldNum, x => valueGetter(x).Cast<int>(), (x, y) => oneValueSetter(x, y.CastTo<E>()), hasValueFunc, canReadUnpacked);
        }


        #endregion

        #endregion
    }
}
