using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Gerakul.ProtoBufSerializer
{
    public class MessageDescriptorBuilder<T> where T : new()
    {
        private List<FieldSetting<T>> fieldSettings = new List<FieldSetting<T>>();

        public ReadOnlyCollection<FieldSetting<T>> FieldSettings => fieldSettings.AsReadOnly();

        internal MessageDescriptorBuilder()
        {
        }

        public MessageDescriptor<T> CreateDescriptor(bool useHasValue = true,
             Func<T, IEnumerable<int>> getterFieldNumsForSerialization = null,
             Action<T, MessageReadData> actionOnMessageRead = null)
        {
            return MessageDescriptor<T>.Create(fieldSettings, useHasValue, getterFieldNumsForSerialization, actionOnMessageRead);
        }

        public MessageDescriptorBuilder<T> AddRange(IEnumerable<FieldSetting<T>> fieldSettings)
        {
            this.fieldSettings.AddRange(fieldSettings);
            return this;
        }

        public MessageDescriptorBuilder<T> Add(FieldSetting<T> fieldSetting)
        {
            this.fieldSettings.Add(fieldSetting);
            return this;
        }

        #region One value

        public MessageDescriptorBuilder<T> Double(int fieldNum, Func<T, double> valueGetter, Action<T, double> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateDouble(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> Float(int fieldNum, Func<T, float> valueGetter, Action<T, float> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateFloat(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> Int32(int fieldNum, Func<T, int> valueGetter, Action<T, int> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateInt32(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> Int64(int fieldNum, Func<T, long> valueGetter, Action<T, long> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateInt64(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> UInt32(int fieldNum, Func<T, uint> valueGetter, Action<T, uint> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateUInt32(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> UInt64(int fieldNum, Func<T, ulong> valueGetter, Action<T, ulong> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateUInt64(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> SInt32(int fieldNum, Func<T, int> valueGetter, Action<T, int> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateSInt32(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> SInt64(int fieldNum, Func<T, long> valueGetter, Action<T, long> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateSInt64(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> Fixed32(int fieldNum, Func<T, uint> valueGetter, Action<T, uint> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateFixed32(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> Fixed64(int fieldNum, Func<T, ulong> valueGetter, Action<T, ulong> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateFixed64(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> SFixed32(int fieldNum, Func<T, int> valueGetter, Action<T, int> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateSFixed32(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> SFixed64(int fieldNum, Func<T, long> valueGetter, Action<T, long> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateSFixed64(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> Bool(int fieldNum, Func<T, bool> valueGetter, Action<T, bool> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateBool(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> Enum<E>(int fieldNum, Func<T, E> valueGetter, Action<T, E> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateEnum<E>(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> String(int fieldNum, Func<T, string> valueGetter, Action<T, string> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateString(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> Bytes(int fieldNum, Func<T, byte[]> valueGetter, Action<T, byte[]> valueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateBytes(fieldNum, valueGetter, valueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> Message<EmbeddedT>(int fieldNum, Func<T, EmbeddedT> valueGetter, Action<T, EmbeddedT> valueSetter,
            MessageDescriptor<EmbeddedT> embeddedDescriptor, Func<T, bool> hasValueFunc = null) where EmbeddedT : new()
        {
            return Add(FieldSetting<T>.CreateMessage<EmbeddedT>(fieldNum, valueGetter, valueSetter, embeddedDescriptor, hasValueFunc));
        }

        #endregion

        #region Array

        public MessageDescriptorBuilder<T> DoubleArray(int fieldNum, Func<T, IEnumerable<double>> valueGetter, Action<T, double> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return Add(FieldSetting<T>.CreateDoubleArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadPacked));
        }

        public MessageDescriptorBuilder<T> FloatArray(int fieldNum, Func<T, IEnumerable<float>> valueGetter, Action<T, float> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return Add(FieldSetting<T>.CreateFloatArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadPacked));
        }

        public MessageDescriptorBuilder<T> Int32Array(int fieldNum, Func<T, IEnumerable<int>> valueGetter, Action<T, int> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return Add(FieldSetting<T>.CreateInt32Array(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadPacked));
        }

        public MessageDescriptorBuilder<T> Int64Array(int fieldNum, Func<T, IEnumerable<long>> valueGetter, Action<T, long> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return Add(FieldSetting<T>.CreateInt64Array(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadPacked));
        }

        public MessageDescriptorBuilder<T> UInt32Array(int fieldNum, Func<T, IEnumerable<uint>> valueGetter, Action<T, uint> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return Add(FieldSetting<T>.CreateUInt32Array(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadPacked));
        }

        public MessageDescriptorBuilder<T> UInt64Array(int fieldNum, Func<T, IEnumerable<ulong>> valueGetter, Action<T, ulong> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return Add(FieldSetting<T>.CreateUInt64Array(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadPacked));
        }

        public MessageDescriptorBuilder<T> SInt32Array(int fieldNum, Func<T, IEnumerable<int>> valueGetter, Action<T, int> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return Add(FieldSetting<T>.CreateSInt32Array(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadPacked));
        }

        public MessageDescriptorBuilder<T> SInt64Array(int fieldNum, Func<T, IEnumerable<long>> valueGetter, Action<T, long> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return Add(FieldSetting<T>.CreateSInt64Array(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadPacked));
        }

        public MessageDescriptorBuilder<T> Fixed32Array(int fieldNum, Func<T, IEnumerable<uint>> valueGetter, Action<T, uint> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return Add(FieldSetting<T>.CreateFixed32Array(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadPacked));
        }

        public MessageDescriptorBuilder<T> Fixed64Array(int fieldNum, Func<T, IEnumerable<ulong>> valueGetter, Action<T, ulong> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return Add(FieldSetting<T>.CreateFixed64Array(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadPacked));
        }

        public MessageDescriptorBuilder<T> SFixed32Array(int fieldNum, Func<T, IEnumerable<int>> valueGetter, Action<T, int> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return Add(FieldSetting<T>.CreateSFixed32Array(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadPacked));
        }

        public MessageDescriptorBuilder<T> SFixed64Array(int fieldNum, Func<T, IEnumerable<long>> valueGetter, Action<T, long> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return Add(FieldSetting<T>.CreateSFixed64Array(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadPacked));
        }

        public MessageDescriptorBuilder<T> BoolArray(int fieldNum, Func<T, IEnumerable<bool>> valueGetter, Action<T, bool> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return Add(FieldSetting<T>.CreateBoolArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadPacked));
        }

        public MessageDescriptorBuilder<T> EnumArray<E>(int fieldNum, Func<T, IEnumerable<E>> valueGetter, Action<T, E> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadPacked = true)
        {
            return Add(FieldSetting<T>.CreateEnumArray<E>(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadPacked));
        }

        public MessageDescriptorBuilder<T> StringArray(int fieldNum, Func<T, IEnumerable<string>> valueGetter, Action<T, string> oneValueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateStringArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> BytesArray(int fieldNum, Func<T, IEnumerable<byte[]>> valueGetter, Action<T, byte[]> oneValueSetter, Func<T, bool> hasValueFunc = null)
        {
            return Add(FieldSetting<T>.CreateBytesArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc));
        }

        public MessageDescriptorBuilder<T> MessageArray<EmbeddedT>(int fieldNum, Func<T, IEnumerable<EmbeddedT>> valueGetter, Action<T, EmbeddedT> oneValueSetter,
            MessageDescriptor<EmbeddedT> embeddedDescriptor, Func<T, bool> hasValueFunc = null) where EmbeddedT : new()
        {
            return Add(FieldSetting<T>.CreateMessageArray<EmbeddedT>(fieldNum, valueGetter, oneValueSetter, embeddedDescriptor, hasValueFunc));
        }

        #endregion

        #region Packed array

        public MessageDescriptorBuilder<T> DoublePackedArray(int fieldNum, Func<T, IEnumerable<double>> valueGetter, Action<T, double> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return Add(FieldSetting<T>.CreateDoublePackedArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadUnpacked));
        }

        public MessageDescriptorBuilder<T> FloatPackedArray(int fieldNum, Func<T, IEnumerable<float>> valueGetter, Action<T, float> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return Add(FieldSetting<T>.CreateFloatPackedArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadUnpacked));
        }

        public MessageDescriptorBuilder<T> Int32PackedArray(int fieldNum, Func<T, IEnumerable<int>> valueGetter, Action<T, int> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return Add(FieldSetting<T>.CreateInt32PackedArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadUnpacked));
        }

        public MessageDescriptorBuilder<T> Int64PackedArray(int fieldNum, Func<T, IEnumerable<long>> valueGetter, Action<T, long> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return Add(FieldSetting<T>.CreateInt64PackedArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadUnpacked));
        }

        public MessageDescriptorBuilder<T> UInt32PackedArray(int fieldNum, Func<T, IEnumerable<uint>> valueGetter, Action<T, uint> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return Add(FieldSetting<T>.CreateUInt32PackedArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadUnpacked));
        }

        public MessageDescriptorBuilder<T> UInt64PackedArray(int fieldNum, Func<T, IEnumerable<ulong>> valueGetter, Action<T, ulong> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return Add(FieldSetting<T>.CreateUInt64PackedArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadUnpacked));
        }

        public MessageDescriptorBuilder<T> SInt32PackedArray(int fieldNum, Func<T, IEnumerable<int>> valueGetter, Action<T, int> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return Add(FieldSetting<T>.CreateSInt32PackedArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadUnpacked));
        }

        public MessageDescriptorBuilder<T> SInt64PackedArray(int fieldNum, Func<T, IEnumerable<long>> valueGetter, Action<T, long> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return Add(FieldSetting<T>.CreateSInt64PackedArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadUnpacked));
        }

        public MessageDescriptorBuilder<T> Fixed32PackedArray(int fieldNum, Func<T, IEnumerable<uint>> valueGetter, Action<T, uint> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return Add(FieldSetting<T>.CreateFixed32PackedArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadUnpacked));
        }

        public MessageDescriptorBuilder<T> Fixed64PackedArray(int fieldNum, Func<T, IEnumerable<ulong>> valueGetter, Action<T, ulong> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return Add(FieldSetting<T>.CreateFixed64PackedArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadUnpacked));
        }

        public MessageDescriptorBuilder<T> SFixed32PackedArray(int fieldNum, Func<T, IEnumerable<int>> valueGetter, Action<T, int> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return Add(FieldSetting<T>.CreateSFixed32PackedArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadUnpacked));
        }

        public MessageDescriptorBuilder<T> SFixed64PackedArray(int fieldNum, Func<T, IEnumerable<long>> valueGetter, Action<T, long> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return Add(FieldSetting<T>.CreateSFixed64PackedArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadUnpacked));
        }

        public MessageDescriptorBuilder<T> BoolPackedArray(int fieldNum, Func<T, IEnumerable<bool>> valueGetter, Action<T, bool> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return Add(FieldSetting<T>.CreateBoolPackedArray(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadUnpacked));
        }

        public MessageDescriptorBuilder<T> EnumPackedArray<E>(int fieldNum, Func<T, IEnumerable<E>> valueGetter, Action<T, E> oneValueSetter, Func<T, bool> hasValueFunc = null, bool canReadUnpacked = true)
        {
            return Add(FieldSetting<T>.CreateEnumPackedArray<E>(fieldNum, valueGetter, oneValueSetter, hasValueFunc, canReadUnpacked));
        }

        #endregion
    }

    public static class MessageDescriptorBuilder
    {
        public static MessageDescriptorBuilder<T> New<T>() where T : new()
        {
            return new MessageDescriptorBuilder<T>();
        }
    }
}
