using System;
using System.Text;
using System.Runtime.InteropServices;

namespace MNF.Message
{
    public static class JsonMessageBuffer
    {
        public static int MaxMessageSize()
        {
            return 1024 * 32;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class JsonMessageHeader
    {
        [MarshalAs(UnmanagedType.I2)]
        public short messageSize;

        [MarshalAs(UnmanagedType.U2)]
        public ushort messageID;
    }

    public class JsonMessageSerializer : Serializer<JsonMessageHeader>
    {
        public JsonMessageSerializer() : base(JsonMessageBuffer.MaxMessageSize())
        {
        }

        public byte[] convertToByte(JsonMessageHeader header)
        {
            int byteArray = 0;
            byteArray = header.messageID;
            byteArray <<= 16;
            byteArray |= header.messageSize;

            return BitConverter.GetBytes(byteArray);
        }

        protected override void _Serialize<T>(int messageID, T managedData)
        {
            var jsonData = JsonSupport.Serialize<T>(managedData);
            var convertedData = Encoding.UTF8.GetBytes(jsonData);
            SerializedLength = convertedData.Length;

            MessageHeader.messageSize = (short)convertedData.Length;
            MessageHeader.messageID = (ushort)messageID;

            var convertedHeader = convertToByte(MessageHeader);
            SerializedLength += convertedHeader.Length;

            Buffer.BlockCopy(convertedHeader, 0, GetSerializedBuffer(), 0, convertedHeader.Length);
            Buffer.BlockCopy(convertedData, 0, GetSerializedBuffer(), convertedHeader.Length, convertedData.Length);
        }
    }

    public class JsonMessageDeserializer : Deserializer<JsonMessageHeader>
    {
        IntPtr marshalAllocatedBuffer;
        int marshalAllocatedBufferSize;

        public JsonMessageDeserializer() : base(JsonMessageBuffer.MaxMessageSize())
        {
            marshalAllocatedBufferSize = JsonMessageBuffer.MaxMessageSize();
            marshalAllocatedBuffer = MarshalHelper.AllocGlobalHeap(marshalAllocatedBufferSize);
        }

        ~JsonMessageDeserializer()
        {
            MarshalHelper.DeAllocGlobalHeap(marshalAllocatedBuffer);
        }

        protected override void _Deserialize(SessionBase session, ref ParsingResult parsingResult)
        {
            var tcpSession = session as TCPSession;

            // check readable header
            if (tcpSession.RecvCircularBuffer.ReadableSize < SerializedHeaderSize)
            {
                parsingResult.parsingResultEnum = ParsingResult.ParsingResultEnum.PARSING_INCOMPLETE;
                return;
            }

            // read header
            if (tcpSession.RecvCircularBuffer.read(SerializedBuffer, SerializedHeaderSize) == false)
            {
                parsingResult.parsingResultEnum = ParsingResult.ParsingResultEnum.PARSING_ERROR;
                return;
            }

            int messageBodySize = SerializedBuffer[1] * 256 + SerializedBuffer[0];
            int messageSize = messageBodySize + SerializedHeaderSize;
            int messageId = SerializedBuffer[3] * 256 + SerializedBuffer[2];

            // check header id
            var dispatchInfo = tcpSession.DispatchHelper.TryGetMessageDispatch(messageId);
            if (dispatchInfo == null)
            {
                parsingResult.parsingResultEnum = ParsingResult.ParsingResultEnum.PARSING_ERROR;
                return;
            }

            // check readalbe body
            if (tcpSession.RecvCircularBuffer.ReadableSize < SerializedHeaderSize + messageBodySize)
            {
                parsingResult.parsingResultEnum = ParsingResult.ParsingResultEnum.PARSING_INCOMPLETE;
                return;
            }

            // read body
            if (tcpSession.RecvCircularBuffer.read(SerializedBuffer, SerializedHeaderSize, messageBodySize) == false)
            {
                parsingResult.parsingResultEnum = ParsingResult.ParsingResultEnum.PARSING_ERROR;
                return;
            }

            // byte -> string
            var jsonMessage = Encoding.UTF8.GetString(SerializedBuffer, 0, messageBodySize);

            // string -> object
            var message = JsonSupport.DeSerialize(jsonMessage, dispatchInfo.messageType);
            if (message == null)
            {
                parsingResult.parsingResultEnum = ParsingResult.ParsingResultEnum.PARSING_ERROR;
                return;
            }

            parsingResult.parsingResultEnum = ParsingResult.ParsingResultEnum.PARSING_COMPLETE;
            parsingResult.dispatcher = dispatchInfo.dispatcher;
            parsingResult.message = message;
            parsingResult.messageSize = messageSize;

            // pop dispatched message size
            tcpSession.RecvCircularBuffer.pop(messageSize);
        }
	}

	public class JsonSession : TCPSession<JsonMessageSerializer, JsonMessageDeserializer>
	{
	}
}
