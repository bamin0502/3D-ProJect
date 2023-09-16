using System;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Threading;

#if !NETFX_CORE
using System.Net.Sockets;
#else
using System.Threading.Tasks;
using Windows.System.Diagnostics;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

namespace MNF
{
	public class SendMessage
	{
		public int ID { get; private set; }
		public Type Type { get; private set; }
		public object Message { get; private set; }

		public SendMessage(int id, Type type, object message)
		{
			ID = id;
            Type = type;
			Message = message;
		}
	}

	public class RecvMessage
	{
        public bool IsMine { get; private set; }
		public int ID { get; private set; }
		public Type Type { get; private set; }
		public object Message { get; private set; }

		public RecvMessage(bool isMine, int id, Type type, object message)
		{
            IsMine = isMine;
            ID = id;
            Type = type;
            Message = message;
		}
	}

    public class ResponseEndPointInfo
    {
        public IPEndPoint ipEndPoint;
        public bool isServer;
        public string uniqueKey;
        public DateTime registedDateTime;

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}:{3}", ipEndPoint, isServer, uniqueKey, registedDateTime);
        }
    }

    public class LookAround : Singleton<LookAround>
    {
#if !NETFX_CORE
		UdpClient udpClient;
#else
        DatagramSocket listenerSocket;
#endif
        public bool IsRunning { get; private set; }

        public bool IsServer { get; private set; }
        public bool IsFoundServer { get; private set; }

        public string ServerIP { get; private set; }
        public string ServerPort { get; private set; }

        public bool IsSetMyInfo { get; private set; }
        public string MyIP { get; private set; }
        public string MyPort { get; private set; }

        IPAddress multicastaddress = IPAddress.Parse("224.0.0.224");
		int multicastPort = 5100;

		List<ResponseEndPointInfo> responseEndPoint = new List<ResponseEndPointInfo>();
		string GeneratedKey { get; set; }
        string LookAroundGeneratedKey { get; set; }
        string MessageGeneratedKey { get; set; }

		StringBuilder messageStringBuilder = new StringBuilder(1024 * 100);
		string LookAroundCmd { get { return "@LOOKAROUND@"; } }
        string MessageCmd { get { return "@MESSAGE@"; } }

		SwapableMessgeQueue<object> sendSwapableMessgeQueue = new SwapableMessgeQueue<object>();
		SwapableMessgeQueue<RecvMessage> recvSwapableMessgeQueue = new SwapableMessgeQueue<RecvMessage>();
		AutoResetEvent sendResetEvent = new AutoResetEvent(false);
		ThreadAdapter lookSendThreadAdapter;

		~LookAround()
        {
            Stop();
        }

        public List<ResponseEndPointInfo> GetResponseEndPoint(int timeoutSecond = 0)
        {
            lock (responseEndPoint)
            {
				var returnEndPoints = new List<ResponseEndPointInfo>(responseEndPoint.Count);
				for (int i = 0; i < responseEndPoint.Count; ++i)
				{
                    if (timeoutSecond == 0)
                    {
                        returnEndPoints.Add(responseEndPoint[i]);
                    }
                    else if (responseEndPoint[i].registedDateTime.AddSeconds(timeoutSecond) >= DateTime.Now)
					{
                        returnEndPoints.Add(responseEndPoint[i]);
                    }
                    else
					{
						responseEndPoint.RemoveAt(i);
						--i;
					}
				}
                return returnEndPoints;
            }
        }

		public void SetSendFrequence(int sendFrequence)
		{
            if (lookSendThreadAdapter != null)
    			lookSendThreadAdapter.WaitTime = sendFrequence;
		}

#if !NETFX_CORE
		public void Start(string port, bool isServer, int sendFrequence = 1000)
#else
        async public void Start(string port, bool isServer)
#endif
        {
            if (IsRunning == true)
            {
                LogManager.Instance.WriteSystem("LookAround has already start");
                return;
            }

            IsServer = isServer;
            MyPort = port;

			GeneratedKey = string.Format("{0}#{1}#{2}", Utility.GetProcessID(), MyPort, IsServer);
			LookAroundGeneratedKey = string.Format("{0}#{1}", LookAroundCmd, GeneratedKey);
			MessageGeneratedKey = string.Format("{0}#{1}", MessageCmd, GeneratedKey);

#if !NETFX_CORE
            try
            {
                udpClient = new UdpClient();
                udpClient.Client.Ttl = 1;
                udpClient.Client.MulticastLoopback = true;

                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.ExclusiveAddressUse = false;

                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, multicastPort));
                udpClient.JoinMulticastGroup(multicastaddress);

				// send thread
                lookSendThreadAdapter = new ThreadAdapter(sendResetEvent);
                lookSendThreadAdapter.ThreadEvent += LookSendThread;
                lookSendThreadAdapter.WaitTime = sendFrequence;
                if (lookSendThreadAdapter.Start() == false)
                    throw new Exception("Send thread starg failed");

                udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), this);

                IsRunning = true;
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "start() failed");
                Stop();
                return;
            }
#else
            try
            {
                listenerSocket = new DatagramSocket();
                listenerSocket.MessageReceived += ReceiveCallback;
                listenerSocket.Control.MulticastOnly = true;

                // set multicast
                await listenerSocket.BindServiceNameAsync(multicastPort.ToString());
                listenerSocket.JoinMulticastGroup(new HostName(multicastaddress.ToString()));

                // send thread
                lookSendThreadAdapter = new ThreadAdapter(sendResetEvent);
                lookSendThreadAdapter.ThreadEvent += LookSendThread;
                lookSendThreadAdapter.WaitTime = sendFrequence;
                if (lookSendThreadAdapter.Start() == false)
                    throw new Exception("Send thread starg failed");

                IsRunning = true;
            }
            catch (Exception e)
            {
                listenerSocket.Dispose();
                listenerSocket = null;

                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(e.HResult) == SocketErrorStatus.Unknown)
                {
                    LogManager.Instance.WriteException(e, "uwp : lookSendThread");
                    throw;
                }
            }
#endif
		}

        public void Stop()
        {
            IsRunning = false;

#if !NETFX_CORE
            if (udpClient != null)
            {
				udpClient.Close();
                udpClient = null;
            }

            if (lookSendThreadAdapter != null)
            {
				lookSendThreadAdapter.Stop();
                lookSendThreadAdapter.Dispose();
                lookSendThreadAdapter = null;
			}
#else
            listenerSocket.Dispose();
            listenerSocket = null;
#endif
        }

#if !NETFX_CORE
        void SendLookAroundMessage()
#else
        async void SendLookAroundMessage()
#endif
        {
#if !NETFX_CORE
            try
            {
                var buffer = Encoding.Unicode.GetBytes(LookAroundGeneratedKey);
                var remoteEP = new IPEndPoint(multicastaddress, multicastPort);
                udpClient.Send(buffer, buffer.Length, remoteEP);
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "lookSendThread");
            }
#else
            try
	        {
	            var buffer = Encoding.Unicode.GetBytes(LookAroundGeneratedKey);
	            IOutputStream outputStream = (await listenerSocket.GetOutputStreamAsync(
	                new HostName(multicastaddress.ToString()), Convert.ToString(multicastPort)));
	            DataWriter writer = new DataWriter(outputStream);
	            writer.WriteBytes(buffer);
	            await writer.StoreAsync();
	        }
	        catch (Exception e)
	        {
	            // If this is an unknown status it means that the error is fatal and retry will likely fail.
	            if (SocketError.GetStatus(e.HResult) == SocketErrorStatus.Unknown)
	            {
	                LogManager.Instance.WriteException(e, "uwp : 1. lookSendThread");
	                throw;
	            }
	        }
#endif
        }

#if !NETFX_CORE
        void LookSendThread(bool isSignal)
#else
        async void LookSendThread(bool isSignal)
#endif
        {
            if (isSignal == false)
            {
                SendLookAroundMessage();
            }

            if (sendSwapableMessgeQueue.getReadableQueue().Count == 0)
			{
				lock (sendSwapableMessgeQueue)
				{
					sendSwapableMessgeQueue.swap();
				}
			}

            if (sendSwapableMessgeQueue.getReadableQueue().Count == 0)
                return;

            var sendMessage = sendSwapableMessgeQueue.getReadableQueue().Peek() as SendMessage;
			sendSwapableMessgeQueue.getReadableQueue().Dequeue();

            var jsonData = JsonSupport.Serialize(sendMessage.Message);
			messageStringBuilder.Length = 0;
			messageStringBuilder.Append(MessageGeneratedKey);
			messageStringBuilder.Append("#");
            messageStringBuilder.Append(sendMessage.ID);
			messageStringBuilder.Append("#");
            messageStringBuilder.Append(sendMessage.Type.FullName);
			messageStringBuilder.Append("#");
			messageStringBuilder.Append(jsonData);

			var sendBytes = Encoding.Unicode.GetBytes(messageStringBuilder.ToString());

#if !NETFX_CORE
			udpClient.Send(sendBytes, sendBytes.Length, new IPEndPoint(multicastaddress, multicastPort));
#else
            IOutputStream outputStream = (await listenerSocket.GetOutputStreamAsync(
                new HostName(multicastaddress.ToString()), Convert.ToString(multicastPort)));
            DataWriter writer = new DataWriter(outputStream);
            writer.WriteBytes(sendBytes);
            await writer.StoreAsync();
#endif
        }

#if !NETFX_CORE
        static void ReceiveCallback(IAsyncResult ar)
		{
            LookAround lookAround = (LookAround)ar.AsyncState;

			string recvString = "";
            var ipEndPoint = new IPEndPoint(IPAddress.Any, lookAround.multicastPort);
			try
            {
                Byte[] receiveBytes = lookAround.udpClient.EndReceive(ar, ref ipEndPoint);
				recvString = Encoding.Unicode.GetString(receiveBytes);
            }
            catch (Exception ex)
            {
				LogManager.Instance.WriteException(ex, "Failed to recv data");
            }

            if (recvString.Contains(Instance.MessageCmd) == true)
			{
				lookAround.MessageResponse(recvString);
			}
            else if (recvString.Contains(Instance.LookAroundCmd) == true)
			{
                lookAround.LookAroundResponse(recvString, ipEndPoint.Address.ToString());
			}
			else
			{
				LogManager.Instance.WriteError("Not command : {0}", recvString);
			}

            lookAround.udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), lookAround);
		}
#else
        void ReceiveCallback(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs eventArguments)
        {
            try
            {
                uint receivedBufferLenght = eventArguments.GetDataReader().UnconsumedBufferLength;
                byte[] recvBuffer = new byte[receivedBufferLenght];
                eventArguments.GetDataReader().ReadBytes(recvBuffer);
                var recvString = Encoding.Unicode.GetString(recvBuffer);

				if (recvString.Contains(MessageCmd) == true)
				{
					MessageResponse(recvString);
				}
				else if (recvString.Contains(LookAroundCmd) == true)
				{
					LookAroundResponse(recvString, eventArguments.RemoteAddress.ToString());
				}
				else
				{
					LogManager.Instance.WriteError("Not command : {0}", recvString);
				}
            }
            catch (Exception e)
            {
                SocketErrorStatus socketError = SocketError.GetStatus(e.HResult);

                if (SocketError.GetStatus(e.HResult) == SocketErrorStatus.Unknown)
                {
                    LogManager.Instance.WriteException(e, "uwp : 4. lookRecvThread");
                    throw;
                }
            }
        }
#endif

		enum SPLIT_INDEX
		{
            CMD,
            PROCESS_ID,
            TARGET_PORT,
            IS_SERVER,
            MESSAGE_ID,
            MESSAGE_TYPE,
            MESSAGE_JSON,
		}
        public RecvMessage PopReponseMessage()
        {
            if (recvSwapableMessgeQueue.getReadableQueue().Count == 0)
            {
                lock (recvSwapableMessgeQueue)
                {
					recvSwapableMessgeQueue.swap();
				}
            }

            if (recvSwapableMessgeQueue.getReadableQueue().Count == 0)
                return null;

            RecvMessage returnMessage = recvSwapableMessgeQueue.getReadableQueue().Peek();
            recvSwapableMessgeQueue.getReadableQueue().Dequeue();
            return returnMessage;
        }

        void MessageResponse(string recvString)
        {
            string[] splitedKey = recvString.Split('#');
            if (splitedKey.Length != 7)
                return;

			try
			{
				int processID = Convert.ToInt32(splitedKey[(int)SPLIT_INDEX.PROCESS_ID]);
                int messageID = Convert.ToInt32(splitedKey[(int)SPLIT_INDEX.MESSAGE_ID]);
				string messageType = splitedKey[(int)SPLIT_INDEX.MESSAGE_TYPE];
                string messageJson = splitedKey[(int)SPLIT_INDEX.MESSAGE_JSON];
                bool isMyMessage = (Utility.GetProcessID() == processID);

                Type t = Type.GetType(messageType);
				var message = JsonSupport.DeSerialize(messageJson, t);

                var responseMessage = new RecvMessage(isMyMessage, messageID, t, message);
                lock (recvSwapableMessgeQueue)
                {
					recvSwapableMessgeQueue.getWritableQueue().Enqueue(responseMessage);
				}
			}
			catch (Exception e)
			{
				LogManager.Instance.WriteException(e, "2. lookRecvThread");
				return;
			}
        }

        void LookAroundResponse(string recvString, string remoteIP)
        {
			string[] splitedKey = recvString.Split('#');
			if (splitedKey.Length != 4)
				return;

			string uniqueKey = splitedKey[(int)SPLIT_INDEX.PROCESS_ID];
			string port = "";
			bool isServer = false;
			try
			{
                int processID = Convert.ToInt32(splitedKey[(int)SPLIT_INDEX.PROCESS_ID]);
                port = splitedKey[(int)SPLIT_INDEX.TARGET_PORT];
                isServer = Convert.ToBoolean(splitedKey[(int)SPLIT_INDEX.IS_SERVER]);

				if (isServer == true)
				{
					IsFoundServer = true;
					ServerIP = remoteIP;
					ServerPort = port;
				}

                if (Utility.GetProcessID() == processID)
				{
					MyIP = remoteIP;
					IsSetMyInfo = true;
					LogManager.Instance.WriteSystem("My IP : {0}, MyPort : {1}", MyIP, port);
					return;
				}
			}
			catch (Exception e)
			{
				LogManager.Instance.WriteException(e, "2. lookRecvThread");
				return;
			}

			lock (responseEndPoint)
			{
				var recvIPEndPoint = Utility.GetIPEndPoint(remoteIP, port);
				try
				{
					foreach (var endPoint in responseEndPoint)
					{
                        if ((endPoint.ipEndPoint.ToString() == recvIPEndPoint.ToString()) && (uniqueKey == endPoint.uniqueKey))
						{
                            endPoint.registedDateTime = DateTime.Now;
							recvIPEndPoint = null;
							break;
						}
					}
				}
				catch (Exception e)
				{
					LogManager.Instance.WriteException(e, "3. lookRecvThread");
					return;
				}

				try
				{
					if (recvIPEndPoint == null)
						return;

					var responseEndPointInfo = new ResponseEndPointInfo();
					responseEndPointInfo.ipEndPoint = recvIPEndPoint;
					responseEndPointInfo.isServer = isServer;
					responseEndPointInfo.uniqueKey = uniqueKey;
                    responseEndPointInfo.registedDateTime = DateTime.Now;
					responseEndPoint.Add(responseEndPointInfo);

					LogManager.Instance.WriteSystem("Add : {0}", recvIPEndPoint.ToString());
				}
				catch (Exception e)
				{
					LogManager.Instance.WriteException(e, "4. lookRecvThread");
					return;
				}
			}
        }

        public void SendMessage<T>(int messageID, T managedData) where T : new()
        {
            var sendMessage = new SendMessage(messageID, typeof(T), managedData);
			lock (sendSwapableMessgeQueue)
			{
                sendSwapableMessgeQueue.getWritableQueue().Enqueue(sendMessage);
			}
            sendResetEvent.Set();
        }
    }
}
