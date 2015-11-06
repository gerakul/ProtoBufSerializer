using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerakul.ProtoBufSerializer
{
    // Класс не потокобезопасный
    public class MessageReader<T> : IDisposable where T : new()
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
