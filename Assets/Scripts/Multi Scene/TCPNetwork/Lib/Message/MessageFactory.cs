using System;

namespace MNF.Message
{
    public class MessageBuffer<TMessageHeader> where TMessageHeader : new()
    {
        #region Header Support
        static int headerSize = MarshalHelper.GetManagedDataSize(typeof(TMessageHeader));
        public static int SerializedHeaderSize { get { return headerSize; } }
        #endregion Header Support

        public TMessageHeader MessageHeader { get; private set; }
        public byte[] SerializedBuffer { get; set; }
        public int SerializedBufferLength { get { return SerializedBuffer.Length; } }

        public MessageBuffer(int messageBufferSize)
        {
            MessageHeader = new TMessageHeader();
            SerializedBuffer = new byte[messageBufferSize];
        }
    }

    public abstract class Serializer
    {
        public int SerializedLength { get; set; }
		public int MaxMessageBufferSize { get; protected set; }

		public bool Serialize<T>(int messageID, T managedData) where T : new()
        {
            try
            {
                _Serialize(messageID, managedData);

                if (SerializedLength < 0)
                    throw new Exception(string.Format("SerializedLength:{0} is smaller than 0", SerializedLength));
                
                if (SerializedLength > MaxMessageBufferSize)
                    throw new Exception(string.Format("SerializedLength:{0} is lagger than MaxMessageBufferSize:{1}"
                          , SerializedLength, MaxMessageBufferSize));
                
                return true;
            }
            catch(Exception e)
            {
                LogManager.Instance.WriteException(e, "serialize failed, {0}:{1}", messageID, typeof(T));
                return false;
            }
        }

        protected abstract void _Serialize<T>(int messageID, T managedData) where T : new();
        public abstract byte[] GetSerializedBuffer();
    }

    public abstract class Serializer<T> : Serializer
        where T : new()
    {
        MessageBuffer<T> MessageBuffer { get; set; }

        public int SerializedHeaderSize { get { return MessageBuffer<T>.SerializedHeaderSize; } }
        public T MessageHeader { get { return MessageBuffer.MessageHeader; } }
        public Type HeaderType { get { return typeof(T); } }

		public Serializer(int messageBufferSize)
        {
            MaxMessageBufferSize = messageBufferSize;
            MessageBuffer = new MessageBuffer<T>(messageBufferSize);
        }

        public override byte[] GetSerializedBuffer()
        {
            return MessageBuffer.SerializedBuffer;
        }
    }

    public abstract class Deserializer
    {
        public int MaxMessageBufferSize { get; protected set; }

		public bool Deserialize(SessionBase session, ref ParsingResult parsingResult)
        {
            try
            {
                _Deserialize(session, ref parsingResult);
                return true;
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "deserialize failed, {0}", session);
                return false;
            }
        }

        protected abstract void _Deserialize(SessionBase session, ref ParsingResult parsingResult);
    }

    public abstract class Deserializer<T> : Deserializer
        where T : new()
    {
        MessageBuffer<T> MessageBuffer { get; set; }

        public int SerializedHeaderSize { get { return MessageBuffer<T>.SerializedHeaderSize; } }
        public byte[] SerializedBuffer { get { return MessageBuffer.SerializedBuffer; } }
        public T MessageHeader { get { return MessageBuffer.MessageHeader; } }
        public Type HeaderType { get { return typeof(T); } }

        public Deserializer(int messageBufferSize)
        {
            MaxMessageBufferSize = messageBufferSize;
            MessageBuffer = new MessageBuffer<T>(messageBufferSize);
        }
    }

    public abstract class IMessageFactory
    {
        public int MaxSerializerMessageBuffer { get; protected set; }
		public int MaxDeserializerMessageBuffer { get; protected set; }

		public abstract Serializer AllocSerializer();
        public abstract void FreeSerializer(Serializer serializer);
        public abstract Deserializer GetDeserializer();
        public abstract void Init();
	}

    public class MessageFactory<TMSerializer, TMDeserializer> : IMessageFactory
        where TMSerializer : Serializer, new()
        where TMDeserializer : Deserializer, new()
    {
        TObjectPool<TMSerializer> serializerPool = new TObjectPool<TMSerializer>();
        TMDeserializer deserializer = new TMDeserializer();

		public override void Init()
        {
            var serializer = AllocSerializer();
            MaxSerializerMessageBuffer = serializer.MaxMessageBufferSize;
            FreeSerializer(serializer);

            MaxDeserializerMessageBuffer = deserializer.MaxMessageBufferSize;
        }

        public override Serializer AllocSerializer()
        {
            return serializerPool.alloc();
        }

        public override void FreeSerializer(Serializer serializer)
        {
            serializerPool.free(serializer as TMSerializer);
        }

        public override Deserializer GetDeserializer()
        {
            return deserializer;
        }
    }
}
