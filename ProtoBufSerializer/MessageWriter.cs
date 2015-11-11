using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerakul.ProtoBufSerializer
{
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

        public void WriteLenDelimitedStream(IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                WriteWithLength(item);
            }
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
}
