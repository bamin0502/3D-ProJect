using System;
using System.Runtime.InteropServices;

namespace MNF.Message
{
	static class BinaryMessageBuffer
	{
		public static int MaxMessageSize()
		{
			return 1024 * 32;
		}
	}

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class BinaryMessageHeader
    {
        [MarshalAs(UnmanagedType.U2)]
        public ushort messageSize;

        [MarshalAs(UnmanagedType.U2)]
        public ushort messageID;
    }

    public class BinaryMessageSerializer : Serializer<BinaryMessageHeader>
    {
        public BinaryMessageSerializer() : base(BinaryMessageBuffer.MaxMessageSize())
        {
        }

        protected override void _Serialize<T>(int messageID, T managedData)
        {
            int serializedSize = 0;
            MarshalHelper.RawSerialize(
                managedData
                , GetSerializedBuffer()
                , SerializedHeaderSize
                , ref serializedSize);
            SerializedLength = serializedSize;

            var messageHeader = MessageHeader;
            messageHeader.messageSize = (ushort)(serializedSize);
            messageHeader.messageID = (ushort)messageID;
            MarshalHelper.RawSerialize(
                messageHeader
                , GetSerializedBuffer()
                , 0
                , ref serializedSize);
            SerializedLength += serializedSize;
        }
    }

    public class BinaryMessageDeserializer : Deserializer<BinaryMessageHeader>
    {
        IntPtr marshalAllocatedBuffer;
        int marshalAllocatedBufferSize;

        public BinaryMessageDeserializer() : base(BinaryMessageBuffer.MaxMessageSize())
        {
            marshalAllocatedBufferSize = BinaryMessageBuffer.MaxMessageSize();
            marshalAllocatedBuffer = MarshalHelper.AllocGlobalHeap(marshalAllocatedBufferSize);
        }

        ~BinaryMessageDeserializer()
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

            object message = MarshalHelper.RawDeSerialize(
                SerializedBuffer
                , dispatchInfo.messageType
                , 0
                , ref marshalAllocatedBuffer
                , ref marshalAllocatedBufferSize);
            if (message == null)
                throw new Exception(string.Format("Dispatcher({0}), Expect packet size({1})",
                    dispatchInfo, MarshalHelper.GetManagedDataSize(dispatchInfo.messageType)));

            parsingResult.parsingResultEnum = ParsingResult.ParsingResultEnum.PARSING_COMPLETE;
            parsingResult.dispatcher = dispatchInfo.dispatcher;
            parsingResult.message = message;
            parsingResult.messageSize = messageSize;

            // pop dispatched message size
            tcpSession.RecvCircularBuffer.pop(messageSize);
        }
    }
}
