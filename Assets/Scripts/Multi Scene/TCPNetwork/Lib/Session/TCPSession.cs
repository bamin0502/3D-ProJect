using System.Net.Sockets;
using MNF.Message;

namespace MNF
{
    /**
     * @brief TCP communication object.
     * @details Inherits SessionBase.
     */
    public abstract class TCPSession : SessionBase
    {
        // receive buffer
        public byte[] AsyncRecvBuffer { get; private set; }
        public int TotalRecvSize { get; protected set; }

        // serialize/deserialize/dispatch
        public int TotalParingSize { get; protected set; }

        // network
        public bool IsNotifyDisconnect { get; set; }

        /**
         * @brief Initialize the TCPSession.
         * @details Allocate the recv buffer. Create an IMessageFactory.
         */
        internal override void Init()
        {
			MessageFactory = AllocMessageFactory();
            MessageFactory.Init();

            AsyncRecvBuffer = new byte[MessageFactory.MaxDeserializerMessageBuffer];
            RecvCircularBuffer = new CircularBuffer(MessageFactory.MaxDeserializerMessageBuffer);
            IsNotifyDisconnect = false;
            SessionType = SessionType.SESSION_TCP;
        }

        /**
         * @brief Copy the data received by tcp to the circular queue.
         * @param receivedBufferSize Size of received data.
         * @return Returns true if copying data to RecvCircularBuffer succeeds, false if it fails.
         */
        internal bool DoCopyReceivedData(int receivedBufferSize)
        {
            // copy to circular buffer
            TotalRecvSize += receivedBufferSize;
            return RecvCircularBuffer.push(AsyncRecvBuffer, receivedBufferSize);
        }

        /**
         * @brief Deserialize the received message.
         * @details Deserialize the message received through the MessageFactory.
         * @param parsingResult It receives deserialize result.
         */
        internal void DeserializeMessage(ref ParsingResult parsingResult)
        {
            MessageFactory.GetDeserializer().Deserialize(this, ref parsingResult);
            TotalParingSize += parsingResult.messageSize;
        }

        /**
         * @brief This function is only called for sessions that have been connected by TCP.
         * @param clientSocket The connection is complete.
         * @param dispatchExporter An object that serves to deserialize and process the data received by the connected socket.
         */
        internal void SetNetworkReady(Socket clientSocket, IDispatchHelper dispatchExporter)
        {
            Socket = clientSocket;
            DispatchHelper = dispatchExporter;
        }

        /**
         * @brief Off tcp nagle
         * @details
         * Increase tcp communication speed by turning off nagle algorithm of TCP.
         * Note: The higher the speed, the higher the network traffic.
         */
        public void SetNoDelay()
        {
            Socket.NoDelay = true;
        }

        /**
         * @brief On tcp nagle
         * @details
         * By using TCP 's nagle algorithm, the tcp communication speed is lowered.
         * Note: As the speed is reduced, network traffic is reduced.
         */
        public void SetDelay()
        {
            Socket.NoDelay = false;
        }

        /**
         * @brief It sends a message to the other party connected by TCP.
         * @details
         * Sending a message is asynchronous, 
         * and delays such as serializing a message and copying a tcp send buffer occur in a thread 
         * other than the thread that called the function.
         * @param id The id of the message to send.
         * @param message The data of the message to be sent. The message to be sent must be a class.
         * @return Returns true if successful, false if failed.
         */
        public bool AsyncSend<T>(int id, T message) where T : new()
        {
            return AsyncIO.AsyncSend(this, id, message);
        }

        protected abstract IMessageFactory AllocMessageFactory();
    }

    /**
     * @brief TCP communication object.
     * @details Inherits TCPSession.
     * @param TMSerializer Serialize the message to send.
     * @param TMDeserializer Deserialize the received message.
     */
    public abstract class TCPSession<TMSerializer, TMDeserializer> : TCPSession
        where TMSerializer : Serializer, new()
        where TMDeserializer : Deserializer, new()
    {
        /**
         * @brief Allocate objects to serialize and deserialize messages.
         * @return Allocated IMessageFactory.
         */
        protected override IMessageFactory AllocMessageFactory()
        {
            return new MessageFactory<TMSerializer, TMDeserializer>();
        }
    }

	public class BinarySession : TCPSession<BinaryMessageSerializer, BinaryMessageDeserializer>
	{
	}

	public class JsonSession : TCPSession<JsonMessageSerializer, JsonMessageDeserializer>
	{
	}
    public class StreamBinSession : TCPSession<StreamBinMessageSerializer, StreamBinMessageDeserializer>
    {
    }
}
