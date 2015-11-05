using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Gerakul.ProtoBufSerializer
{
    public class MessageDescriptor<T> where T : new()
    {
        private Dictionary<int, FieldSetting<T>> fieldSettings;
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

                        FieldSetting<T> set;
                        if (fieldSettings.TryGetValue(fnum, out set))
                        {
                            set.ReadActionWithoutTag(value, deserializer);
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

                        FieldSetting<T> set;
                        if (fieldSettings.TryGetValue(fnum, out set))
                        {
                            set.ReadActionWithoutTag(value, deserializer);
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
                        var fnum = WireFormat.GetTagFieldNumber(tag);

                        FieldSetting<T> set;
                        if (fieldSettings.TryGetValue(fnum, out set))
                        {
                            set.ReadActionWithoutTag(value, deserializer);
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
                        var fnum = WireFormat.GetTagFieldNumber(tag);

                        FieldSetting<T> set;
                        if (fieldSettings.TryGetValue(fnum, out set))
                        {
                            set.ReadActionWithoutTag(value, deserializer);
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
            return new MessageWriter<T>(writeAction, stream, ownStream);
        }

        public MessageReader<T> CreateReader(Stream stream, bool ownStream = false)
        {
            return new MessageReader<T>(readAction, lenLimitedReadAction, stream, ownStream);
        }
    }

    // Класс не потокобезопасный
    public class MessageWriter<T> : IDisposable
    {
        private Action<T, BasicSerializer> writeAction;
        private MemoryStream internalStream;
        private BasicSerializer internalSerializer;
        private Stream stream;
        private BasicSerializer serializer;
        private bool ownStream;

        internal MessageWriter(Action<T, BasicSerializer> writeAction, Stream stream, bool ownStream)
        {
            this.writeAction = writeAction;
            this.internalStream = new MemoryStream();
            this.internalSerializer = new BasicSerializer(this.internalStream);
            this.stream = stream;
            this.serializer = new BasicSerializer(stream);
            this.ownStream = ownStream;
        }

        public void Write(T value)
        {
            writeAction(value, serializer);
        }

        public void WriteWithLength(T value)
        {
            internalStream.Position = 0;
            writeAction(value, internalSerializer);

            int len = (int)internalStream.Position;
            serializer.WriteLength(len);
            stream.Write(internalStream.GetBuffer(), 0, len);
        }

        public void Dispose()
        {
            internalStream.Dispose();

            if (ownStream)
            {
                stream.Dispose();
            }
        }
    }

    // Класс не потокобезопасный
    public class MessageReader<T> : IDisposable where T : new ()
    {
        private Func<BasicDeserializer, T> readAction;
        private Func<BasicDeserializer, int, T> lenLimitedReadAction;
        private Stream stream;
        private BasicDeserializer serializer;
        private bool ownStream;

        internal MessageReader(Func<BasicDeserializer, T> readAction, Func<BasicDeserializer, int, T> lenLimitedReadAction, Stream stream, bool ownStream)
        {
            this.readAction = readAction;
            this.lenLimitedReadAction = lenLimitedReadAction;
            this.stream = stream;
            this.serializer = new BasicDeserializer(stream);
            this.ownStream = ownStream;
        }

        public T Read()
        {
            return readAction(serializer);
        }

        public T ReadWithLen()
        {
            var len = serializer.ReadLength();
            return lenLimitedReadAction(serializer, len);
        }

        public void Dispose()
        {
            if (ownStream)
            {
                stream.Dispose();
            }
        }
    }
}
