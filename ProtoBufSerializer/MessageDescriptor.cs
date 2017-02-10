using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Gerakul.ProtoBufSerializer
{
    public class MessageDescriptor<T> : IUntypedMessageDescriptor where T : new()
    {
        private Dictionary<int, FieldSetting<T>> fieldSettings;
        private Dictionary<uint, Action<T, BasicDeserializer>> readActionsByTag;
        private bool useHasValue;
        private Func<T, IEnumerable<int>> getterFieldNumsForSerialization;
        private Action<T, MessageReadData> actionOnMessageRead;

        private Action<T, BasicSerializer> writeAction;
        private Func<BasicDeserializer, T> readAction;
        private Func<BasicDeserializer, int, T> lenLimitedReadAction;
        private bool initialized;

        public bool UseHasValue
        {
            get
            {
                return useHasValue;
            }
            set
            {
                useHasValue = value;
                initialized = false;
            }
        }

        public Func<T, IEnumerable<int>> GetterFieldNumsForSerialization
        {
            get
            {
                return getterFieldNumsForSerialization;
            }
            set
            {
                getterFieldNumsForSerialization = value;
                initialized = false;
            }
        }

        public Action<T, MessageReadData> ActionOnMessageRead
        {
            get
            {
                return actionOnMessageRead;
            }
            set
            {
                actionOnMessageRead = value;
                initialized = false;
            }
        }

        private MessageDescriptor(IEnumerable<FieldSetting<T>> fieldSettings,
             bool useHasValue = true,
             Func<T, IEnumerable<int>> getterFieldNumsForSerialization = null,
             Action<T, MessageReadData> actionOnMessageRead = null)
        {
            this.fieldSettings = fieldSettings.ToDictionary(x => x.FieldNum);

            this.readActionsByTag = fieldSettings.ToDictionary(x => x.Tag, x => x.ReadActionWithoutTag);

            foreach (var item in fieldSettings.Where(x => x.AltTag > 0))
            {
                this.readActionsByTag.Add(item.AltTag, item.AltReadActionWithoutTag);
            }

            this.useHasValue = useHasValue;
            this.getterFieldNumsForSerialization = getterFieldNumsForSerialization;
            this.actionOnMessageRead = actionOnMessageRead;
            this.initialized = false;
        }

        public static MessageDescriptor<T> Create(IEnumerable<FieldSetting<T>> fieldSettings,
             bool useHasValue = true,
             Func<T, IEnumerable<int>> getterFieldNumsForSerialization = null,
             Action<T, MessageReadData> actionOnMessageRead = null)
        {
            var descriptor = new MessageDescriptor<T>(fieldSettings, useHasValue, getterFieldNumsForSerialization, actionOnMessageRead);
            descriptor.Initialize();
            return descriptor;
        }

        private void Initialize()
        {
            // create write action
            if (useHasValue)
            {
                if (getterFieldNumsForSerialization != null)
                {
                    writeAction = (value, serializer) =>
                    {
                        var settings = getterFieldNumsForSerialization(value).Select(x => fieldSettings[x]).OrderBy(x => x.FieldNum).ToArray();
                        foreach (var s in settings)
                        {
                            if (s.HasValueFunc == null || s.HasValueFunc(value))
                            {
                                s.WriteAction(value, serializer, s.RawTag);
                            }
                        }
                    };
                }
                else
                {
                    writeAction = (value, serializer) =>
                    {
                        foreach (var s in fieldSettings.Values)
                        {
                            if (s.HasValueFunc == null || s.HasValueFunc(value))
                            {
                                s.WriteAction(value, serializer, s.RawTag);
                            }
                        }
                    };
                }
            }
            else
            {
                if (getterFieldNumsForSerialization != null)
                {
                    writeAction = (value, serializer) =>
                    {
                        var settings = getterFieldNumsForSerialization(value).Select(x => fieldSettings[x]).OrderBy(x => x.FieldNum).ToArray();
                        foreach (var s in settings)
                        {
                            s.WriteAction(value, serializer, s.RawTag);
                        }
                    };
                }
                else
                {
                    writeAction = (value, serializer) =>
                    {
                        foreach (var s in fieldSettings.Values)
                        {
                            s.WriteAction(value, serializer, s.RawTag);
                        }
                    };
                }
            }

            // create read actions
            if (actionOnMessageRead != null)
            {
                readAction = deserializer =>
                {
                    T value = new T();
                    List<int> readFieldNums = new List<int>();
                    List<uint> unknownTags = new List<uint>();
                    uint tag;
                    while ((tag = deserializer.ReadTag()) > 0)
                    {
                        var fnum = WireFormat.GetTagFieldNumber(tag);

                        Action<T, BasicDeserializer> ra;
                        if (readActionsByTag.TryGetValue(tag, out ra))
                        {
                            ra(value, deserializer);
                            readFieldNums.Add(fnum);
                        }
                        else
                        {
                            deserializer.SkipField(WireFormat.GetTagWireType(tag));
                            unknownTags.Add(tag);
                        }
                    }

                    actionOnMessageRead(value, new MessageReadData(readFieldNums, unknownTags));

                    return value;
                };

                lenLimitedReadAction = (deserializer, lenght) =>
                {
                    T value = new T();
                    List<int> readFieldNums = new List<int>();
                    List<uint> unknownTags = new List<uint>();
                    uint tag;

                    long limitPos = deserializer.stream.Position + lenght;

                    while (deserializer.stream.Position < limitPos)
                    {
                        tag = deserializer.ReadTag();
                        var fnum = WireFormat.GetTagFieldNumber(tag);

                        Action<T, BasicDeserializer> ra;
                        if (readActionsByTag.TryGetValue(tag, out ra))
                        {
                            ra(value, deserializer);
                            readFieldNums.Add(fnum);
                        }
                        else
                        {
                            deserializer.SkipField(WireFormat.GetTagWireType(tag));
                            unknownTags.Add(tag);
                        }
                    }

                    if (deserializer.stream.Position > limitPos)
                    {
                        throw InvalidProtocolBufferException.AllowableMessageLengthWasExceeded();
                    }

                    actionOnMessageRead(value, new MessageReadData(readFieldNums, unknownTags));

                    return value;
                };
            }
            else
            {
                readAction = deserializer =>
                {
                    T value = new T();
                    uint tag;
                    while ((tag = deserializer.ReadTag()) > 0)
                    {
                        Action<T, BasicDeserializer> ra;
                        if (readActionsByTag.TryGetValue(tag, out ra))
                        {
                            ra(value, deserializer);
                        }
                        else
                        {
                            deserializer.SkipField(WireFormat.GetTagWireType(tag));
                        }
                    }

                    return value;
                };

                lenLimitedReadAction = (deserializer, lenght) =>
                {
                    T value = new T();
                    uint tag;
                    long limitPos = deserializer.stream.Position + lenght;
                    while (deserializer.stream.Position < limitPos)
                    {
                        tag = deserializer.ReadTag();

                        Action<T, BasicDeserializer> ra;
                        if (readActionsByTag.TryGetValue(tag, out ra))
                        {
                            ra(value, deserializer);
                        }
                        else
                        {
                            deserializer.SkipField(WireFormat.GetTagWireType(tag));
                        }
                    }

                    if (deserializer.stream.Position > limitPos)
                    {
                        throw InvalidProtocolBufferException.AllowableMessageLengthWasExceeded();
                    }

                    return value;
                };
            }

            initialized = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckInitialized()
        {
            if (!initialized)
            {
                Initialize();
            }
        }

        public MessageWriter<T> CreateWriter(Stream stream, bool ownStream = false)
        {
            CheckInitialized();
            return new MessageWriter<T>(writeAction, stream, ownStream);
        }

        public MessageReader<T> CreateReader(Stream stream, bool ownStream = false)
        {
            CheckInitialized();
            return new MessageReader<T>(readAction, lenLimitedReadAction, stream, ownStream);
        }

        public T Read(byte[] message)
        {
            using (var r = CreateReader(new MemoryStream(message), true))
            {
                return r.Read();
            }
        }

        public T ReadWithLen(byte[] message)
        {
            using (var r = CreateReader(new MemoryStream(message), true))
            {
                return r.ReadWithLen();
            }
        }

        public IEnumerable<T> ReadLenDelimitedStream(byte[] messages)
        {
            using (var r = CreateReader(new MemoryStream(messages), true))
            {
                foreach (var item in r.ReadLenDelimitedStream())
                {
                    yield return item;
                }
            }
        }

        public byte[] Write(T value)
        {
            using (var ms = new MemoryStream())
            {
                using (var w = CreateWriter(ms))
                {
                    w.Write(value);
                }

                ms.Flush();

                return ms.ToArray();
            }
        }

        public byte[] WriteWithLength(T value)
        {
            using (var ms = new MemoryStream())
            {
                using (var w = CreateWriter(ms))
                {
                    w.WriteWithLength(value);
                }

                ms.Flush();

                return ms.ToArray();
            }
        }

        public byte[] WriteLenDelimitedStream(IEnumerable<T> values)
        {
            using (var ms = new MemoryStream())
            {
                using (var w = CreateWriter(ms))
                {
                    w.WriteLenDelimitedStream(values);
                }

                ms.Flush();

                return ms.ToArray();
            }
        }

        #region IUntypedMessageDescriptor

        IUntypedMessageWriter IUntypedMessageDescriptor.CreateWriter(Stream stream, bool ownStream)
        {
            return CreateWriter(stream, ownStream);
        }

        IUntypedMessageReader IUntypedMessageDescriptor.CreateReader(Stream stream, bool ownStream)
        {
            return CreateReader(stream, ownStream);
        }

        object IUntypedMessageDescriptor.Read(byte[] message)
        {
            return Read(message);
        }

        object IUntypedMessageDescriptor.ReadWithLen(byte[] message)
        {
            return ReadWithLen(message);
        }

        IEnumerable IUntypedMessageDescriptor.ReadLenDelimitedStream(byte[] message)
        {
            return ReadLenDelimitedStream(message);
        }

        byte[] IUntypedMessageDescriptor.Write(object value)
        {
            return Write((T)value);
        }

        byte[] IUntypedMessageDescriptor.WriteWithLength(object value)
        {
            return WriteWithLength((T)value);
        }

        byte[] IUntypedMessageDescriptor.WriteLenDelimitedStream(IEnumerable values)
        {
            return WriteLenDelimitedStream(values.Cast<T>());
        }

        #endregion
    }

    public interface IUntypedMessageDescriptor
    {
        IUntypedMessageWriter CreateWriter(Stream stream, bool ownStream = false);
        IUntypedMessageReader CreateReader(Stream stream, bool ownStream = false);
        object Read(byte[] message);
        object ReadWithLen(byte[] message);
        IEnumerable ReadLenDelimitedStream(byte[] message);
        byte[] Write(object value);
        byte[] WriteWithLength(object value);
        byte[] WriteLenDelimitedStream(IEnumerable values);
    }

    public static class UntypedMessageDescriptorExtensions
    {
        public static Type GetArgumentType(this IUntypedMessageDescriptor item)
        {
            var genArgs = item.GetType().GetGenericArguments();

            if (genArgs.Length != 1)
            {
                throw new ArgumentException("Invalid number of generic arguments", nameof(item));
            }

            return genArgs.First();
        }

    }
}
