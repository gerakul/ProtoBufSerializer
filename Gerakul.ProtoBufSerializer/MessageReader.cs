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
    public sealed class MessageReader<T> : IUntypedMessageReader, IDisposable where T : new()
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

        public IEnumerable<T> ReadLenDelimitedStream()
        {
            int len;
            while ((len = serializer.ReadLength(true)) > 0)
            {
                yield return lenLimitedReadAction(serializer, len);
            }
        }

        public void Close()
        {
            if (ownStream)
            {
                stream?.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        #region IUntypedMessageReader

        object IUntypedMessageReader.Read()
        {
            return Read();
        }

        object IUntypedMessageReader.ReadWithLen()
        {
            return ReadWithLen();
        }

        IEnumerable IUntypedMessageReader.ReadLenDelimitedStream()
        {
            return ReadLenDelimitedStream();
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion
    }

    public interface IUntypedMessageReader : IDisposable
    {
        object Read();
        object ReadWithLen();
        IEnumerable ReadLenDelimitedStream();
    }
}
