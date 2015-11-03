using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Gerakul.ProtoBufSerializer
{
    public class MessageDescriptor<T> : IDisposable where T : new()
    {
        private Dictionary<int, FieldSetting<T>> fieldSettings;
        private bool useHasValue;
        private Func<T, IEnumerable<int>> getterFieldNumsForSerialization;
        private Action<T, MessageReadData> actionOnMessageRead;

        private MessageWriter<T> writer;
        private MessageReader<T> reader;
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
            // create writer
            Action<T, BasicSerializer> writeAction;

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

            writer = new MessageWriter<T>(writeAction);

            // create reader
            Func<BasicDeserializer, T> readAction;
            Func<BasicDeserializer, int, T> lenLimitedReadAction;
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

            reader = new MessageReader<T>(readAction, lenLimitedReadAction);

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

        public void Write(T value, BasicSerializer serializer)
        {
            CheckInitialized();
            writer.WriteMessage(value, serializer);
        }

        public void Write(T value, Stream stream)
        {
            BasicSerializer ser = new BasicSerializer(stream);
            Write(value, ser);
        }

        public void WriteWithLength(T value, BasicSerializer serializer)
        {
            CheckInitialized();
            writer.WriteMessageWithLength(value, serializer);
        }

        public void WriteWithLength(T value, Stream stream)
        {
            BasicSerializer ser = new BasicSerializer(stream);
            WriteWithLength(value, ser);
        }

        public T Read(BasicDeserializer deserializer)
        {
            CheckInitialized();
            return reader.ReadMessage(deserializer);
        }

        public T Read(Stream stream)
        {
            BasicDeserializer deser = new BasicDeserializer(stream);
            return Read(deser);
        }

        public T ReadWithLen(BasicDeserializer deserializer)
        {
            CheckInitialized();
            return reader.ReadMessageWithLen(deserializer);
        }

        public T ReadWithLen(Stream stream)
        {
            BasicDeserializer deser = new BasicDeserializer(stream);
            return ReadWithLen(deser);
        }

        public void Dispose()
        {
            if (writer != null)
            {
                writer.Dispose();
            }
        }
    }

    // Класс не потокобезопасный
    internal class MessageWriter<T> : IDisposable
    {
        private const int InternalBufferLen = 1024 * 1024;

        private Action<T, BasicSerializer> writeAction;
        private MemoryStream internalStream;
        private BasicSerializer internalSerializer;

        internal MessageWriter(Action<T, BasicSerializer> writeAction)
        {
            this.writeAction = writeAction;
            this.internalStream = new MemoryStream(InternalBufferLen);
            this.internalSerializer = new BasicSerializer(this.internalStream);
        }

        public void WriteMessage(T value, BasicSerializer serializer)
        {
            writeAction(value, serializer);
        }

        public void WriteMessageWithLength(T value, BasicSerializer serializer)
        {
            internalStream.Position = 0;
            writeAction(value, internalSerializer);

            int len = (int)internalStream.Position;
            serializer.WriteLength(len);
            serializer.stream.Write(internalStream.GetBuffer(), 0, len);

            // не держим в памяти слишком большой буфер
            if (len > InternalBufferLen)
            {
                this.internalStream.Close();
                this.internalStream = new MemoryStream(InternalBufferLen);
                this.internalSerializer = new BasicSerializer(this.internalStream);
            }
        }

        public void Dispose()
        {
            internalStream.Dispose();
        }
    }

    // Класс не потокобезопасный
    internal class MessageReader<T> where T : new ()
    {
        private Func<BasicDeserializer, T> readAction;
        private Func<BasicDeserializer, int, T> lenLimitedReadAction;

        internal MessageReader(Func<BasicDeserializer, T> readAction, Func<BasicDeserializer, int, T> lenLimitedReadAction)
        {
            this.readAction = readAction;
            this.lenLimitedReadAction = lenLimitedReadAction;
        }

        public T ReadMessage(BasicDeserializer deserializer)
        {
            return readAction(deserializer);
        }

        public T ReadMessageWithLen(BasicDeserializer deserializer)
        {
            var len = deserializer.ReadLength();
            return lenLimitedReadAction(deserializer, len);
        }
    }
}
