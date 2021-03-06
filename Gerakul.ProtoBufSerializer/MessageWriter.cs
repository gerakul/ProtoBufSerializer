﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerakul.ProtoBufSerializer
{
    // Класс не потокобезопасный
    public sealed class MessageWriter<T> : IUntypedMessageWriter, IDisposable
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

            ArraySegment<byte> buff;
            if (internalStream.TryGetBuffer(out buff))
            {
                stream.Write(buff.Array, 0, len);
            }
            else
            {
                throw new InvalidOperationException($"Unable to get buffer from {nameof(internalStream)}");
            }
        }

        public void WriteLenDelimitedStream(IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                WriteWithLength(item);
            }
        }

        public void Close()
        {
            internalStream?.Dispose();

            if (ownStream)
            {
                stream?.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        #region IUntypedMessageWriter

        void IUntypedMessageWriter.Write(object value)
        {
            Write((T)value);
        }

        void IUntypedMessageWriter.WriteWithLength(object value)
        {
            WriteWithLength((T)value);
        }

        void IUntypedMessageWriter.WriteLenDelimitedStream(IEnumerable values)
        {
            WriteLenDelimitedStream(values.Cast<T>());
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion
    }

    public interface IUntypedMessageWriter : IDisposable
    {
        void Write(object value);
        void WriteWithLength(object value);
        void WriteLenDelimitedStream(IEnumerable values);
    }

}
