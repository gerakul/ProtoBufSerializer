using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerakul.ProtoBufSerializer
{
    public class InvalidProtocolBufferException : IOException
    {
        internal InvalidProtocolBufferException(string message)
            : base(message)
        {
        }

        internal static InvalidProtocolBufferException MalformedVarint()
        {
            return new InvalidProtocolBufferException(
                "BasicDeserializer encountered a malformed varint.");
        }

        internal static InvalidProtocolBufferException TooBigFieldNumber()
        {
            return new InvalidProtocolBufferException(
                "Too big FieldNumber");
        }

        internal static InvalidProtocolBufferException TruncatedMessage()
        {
            return new InvalidProtocolBufferException(
                "While parsing a protocol message, the input ended unexpectedly " +
                "in the middle of a field.  This could mean either than the " +
                "input has been truncated or that an embedded message " +
                "misreported its own length.");
        }

        internal static InvalidProtocolBufferException UnknownWireType()
        {
            return new InvalidProtocolBufferException(
                "Unknown wire type");
        }

        internal static InvalidProtocolBufferException AllowableMessageLengthWasExceeded()
        {
            return new InvalidProtocolBufferException(
                "Allowable message length was exceeded");
        }

        internal static InvalidProtocolBufferException AllowableFieldLengthWasExceeded()
        {
            return new InvalidProtocolBufferException(
                "Allowable field length was exceeded");
        }
    }
}
