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
            return new MessageWriter<T>(writeAction, stream, ownStream);
        }

        public MessageReader<T> CreateReader(Stream stream, bool ownStream = false)
        {
            return new MessageReader<T>(readAction, lenLimitedReadAction, stream, ownStream);
        }
    }
}
