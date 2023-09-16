using System;
using System.Collections.Generic;
using MNF;

namespace MNF
{
	/**
     * @brief Classes that support tcp.
     * @details
     * With this class, MNF server and client can be created.
     * It also manages the server or client that is created, and includes some features that the server needs.
     * You can wrap the ConnectHelper and AcceptHelper to handle asynchronous connect, asynchronous accept, and so on.
     */
	public class TcpHelper : Singleton<TcpHelper>
    {
        List<IAcceptHelper> acceptHelperList = new List<IAcceptHelper>();
        Dictionary<int, TCPSession> connectedSessionList = new Dictionary<int, TCPSession>();
		IDispatchHelper dbMessageDispatchExporter;
		IDispatchHelper interMessageDispatchExporter;

		public bool IsRunning { get; private set; }
		public bool IsRunThread { get; private set; }

		/**
         * @brief Start TcpHelper.
         * @details
         * Create network_internal, async send dispatcher, which is essential for MNF Tcp functionality.
         * @param isRunThread 
         * If true, the dispatcher creates an individual thread for message processing, and if false, does not create a thread.
         * @return boolean True if starting succeeds, false if it fails.
         */
		public bool Start(bool isRunThread)
        {
            if (IsRunning == true)
                return true;
            
            if (DispatcherCollection.Instance.Start(
                DISPATCH_TYPE.DISPATCH_NETWORK_INTERNAL, isRunThread) == null)
                return false;

			if (DispatcherCollection.Instance.Start(
                DISPATCH_TYPE.DISPATCH_SEND, true) == null)
				return false;

            AsyncIO.SystemMessageCollection = new SystemMessageDispatcher();
            if (AsyncIO.SystemMessageCollection.init() == false)
                return false;

            IsRunning = true;
			IsRunThread = isRunThread;

			return true;
        }

		/**
         * @brief Stop TcpHelper.
         * @details Dispatches DispatcherCollection and Acceptor (Server).
         */
		public void Stop()
        {
            DispatcherCollection.Instance.Stop();
            StopAccept();
            IsRunning = false;
        }

        #region Connect
        TCPSession firstClientSession;

		/**
         * @brief Make an asynchronous connection with the specified ip and port.
         * @details
         * Inside, call ConnectHelper.AsyncConnect ().
         * When the connection is completed, TSession's OnConnectSuccess () function is called,
         * If unsuccessful, TSession's OnConnectFail () is called.
         * @param TSession An MNF network object that inherits SessionBase.
         * @param TDispatcher The object that will handle the TSession's message.
         * @param ipString The IP of the server to connect to.
         * @param portString Port of the server to be connected.
         * @return Return TSession.
         */
		public TSession AsyncConnect<TSession, TDispatcher>(string ipString, string portString)
            where TSession : SessionBase, new()
        {
            return ConnectHelper.AsyncConnect<TSession, TDispatcher>(ipString, portString);
        }

		/**
         * @brief Add the client.
         * @details You only need to add networked sessions. 
         * @param id The id of the client to add.
         * @param session The client session to add.
         */
		public void AddClientSession<TSession>(int id, TSession session)
            where TSession : TCPSession, new()
        {
            if (firstClientSession == null)
                firstClientSession = session;

            connectedSessionList.Add(id, session);
        }

		/**
         * @brief Remove the client.
         * @param id The id of the client to remove.
         */
		public void RemoveClientSession(int id)
        {
            if (firstClientSession == connectedSessionList[id])
                firstClientSession = null;

            connectedSessionList.Remove(id);
        }

		/**
         * @brief
         * @details
         * @param string a
         * @param string b
         * @return mixed|boolean
         * @throws ValidException
         */
		public void RemoveClientSession<TSession>(TSession session) where TSession : TCPSession, new()
        {
            int key = 0;
            foreach(var clientSession in connectedSessionList)
            {
                if (clientSession.Value == session)
                {
                    key = clientSession.Key;
                }
            }
            RemoveClientSession(key);
        }

		/**
         * @brief Returns the added client count.
         * @return int added client count
         */
		public int GetClientSessionCount()
        {
            return connectedSessionList.Count;
        }

		/**
         * @brief Returns an enumerator of the added client list.
         * @return Enumerator of the added client list.
         */
		public Dictionary<int, TCPSession>.Enumerator GetClientSessionEnumerator()
        {
            return connectedSessionList.GetEnumerator();
        }

		/**
         * @brief Return the first added client.
         * @return TSession The first added client.
         */
		public TSession GetFirstClient<TSession>()
            where TSession : TCPSession, new()
        {
            return firstClientSession as TSession;
        }
		#endregion

		#region Accept
		/**
         * @brief Create a server.
         * @details
         * Internally, it creates an AcceptHelper and receives the client's connection through the generated AcceptHelper.
         * @param TSession An MNF network object that inherits SessionBase.
         * @param TDispatcher The object that will handle the TSession's message.
         * @param ipString The IP of the server.
         * @param portString Port of the server.
         * @param backLog The size of the queue to accept client connections.
         * @return Returns true if the client is ready to be contacted, false if it is not.
         */
		public bool StartAccept<TSession, TDispatcher>(string ipString, string portString, int backLog)
            where TSession : SessionBase, new()
        {
            try
            {
                foreach (var acceptHelperIter in acceptHelperList)
                {
                    if (acceptHelperIter.IP == ipString
                    && acceptHelperIter.Port == portString)
                        throw new Exception("Already bind!");
                }

                var acceptHelper = new AcceptHelper<TSession, TDispatcher>();
                if (acceptHelper.Create(ipString, portString, backLog) == false)
                        throw new Exception("Create failed!");

                acceptHelperList.Add(acceptHelper);

                return acceptHelper.StartAccept();
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "session({0}:{1}), IP({2}), Port({3}), startAaccept failed!",
                    typeof(TSession), typeof(TDispatcher), ipString, portString);
                return false;
            }
        }

		/**
         * @brief Do not receive client connections.
         */
		public void StopAccept()
        {
            if (acceptHelperList == null)
                return;

            foreach (var acceptHelper in acceptHelperList)
            {
                acceptHelper.Dispose();
            }
        }
		#endregion

		#region MessageDispatcher
		/**
         * @brief Processes messages received over the network, and internally generated messages.
         * @details
         * Do not call this function if you give isRunThread to true when calling the Start () function.
         */
		public void DipatchNetworkInterMessage()
        {
            if (IsRunThread == true)
                return;
            
            var dispatcher = DispatcherCollection.Instance.GetDispatcher(DISPATCH_TYPE.DISPATCH_NETWORK_INTERNAL);
            if (dispatcher != null)
                dispatcher.dispatchMessage(false);
        }

		/**
         * @brief DB Register the dispatcher to process the message.
         * @details Since db is slow to respond, it is handled by multiple threads.
         * @param TDBMessageDispatcher The object that will handle the DB message.
         * @return Returns true if registration succeeded, false if failed.
         */
		public bool RegistDBMsgDispatcher<TDBMessageDispatcher>()
        {
            if (dbMessageDispatchExporter != null)
                return false;
            
            dbMessageDispatchExporter = RegistMsgDispatcher(typeof(TDBMessageDispatcher));
            if (dbMessageDispatchExporter == null)
                return false;

			DispatcherCollection.Instance.Start(DISPATCH_TYPE.DISPATCH_DB, true);

			return true;
        }

		/**
         * @brief Register a dispatcher to handle internal messages.
         * @details Internal message processing is handled in the same thread as network messages.
         * @param TInterMessageDispatcher The object that will handle the internal message.
         * @return Returns true if registration succeeded, false if failed.
         */
		public bool RegistInterMsgDispatcher<TInterMessageDispatcher>()
        {
            if (interMessageDispatchExporter != null)
				return false;

			interMessageDispatchExporter = RegistMsgDispatcher(typeof(TInterMessageDispatcher));
            if (interMessageDispatchExporter == null)
                return false;

            return true;
        }

		/**
         * @brief Requests a database message to be processed by the database dispatcher.
         * @param messageID The requesting database message index.
         * @param message Data to be processed by the database dispatcher.
         * @return Returns true if the request succeeds, false if it fails.
         */
		public bool RequestDBMessage<TData>(int messageID, TData message)
        {
            var dispatchInfo = dbMessageDispatchExporter.TryGetMessageDispatch(messageID);
            var createdMessage = dbMessageDispatchExporter.AllocMessage(dispatchInfo.dispatcher, message);
            return DispatcherCollection.Instance.PushMessage(DISPATCH_TYPE.DISPATCH_DB, createdMessage);
        }

		/**
         * @brief Requests a internal message to be processed by the internal dispatcher.
         * @param messageID The requesting internal message index.
         * @param message Data to be processed by the internal dispatcher.
         * @return Returns true if the request succeeds, false if it fails.
         */
		public bool RequestInterMessage<TData>(int messageID, TData message)
        {
            var dispatchInfo = interMessageDispatchExporter.TryGetMessageDispatch(messageID);
            var createdMessage = interMessageDispatchExporter.AllocMessage(dispatchInfo.dispatcher, message);
            return DispatcherCollection.Instance.PushMessage(DISPATCH_TYPE.DISPATCH_NETWORK_INTERNAL, createdMessage);
        }

		/**
         * @brief Register the message dispatcher.
         * @details Create, register, and start a message dispatcher.
         * @param messageDispatcherType Type information for the message dispatcher.
         * @param dispatchType Dispatcher type.
         * @param isRunThread 
         * If true, the dispatcher creates an individual thread for message processing, and if false, does not create a thread.
         * @return The generated IDispatchHelper object.
         */
		IDispatchHelper RegistMsgDispatcher(Type messageDispatcherType)
        {
            try
            {
				if (DispatchExporterCollection.Instance.Add(messageDispatcherType) == false)
					throw new Exception("Message Dispatcher add failed");

				var dispatchExporter = DispatchExporterCollection.Instance.Get(messageDispatcherType);
                if (dispatchExporter == null)
					throw new Exception("Message Dispatcher get failed");

				if (dispatchExporter.Init() == false)
                    throw new Exception(string.Format("Message Dispatcher:({0}) init failed", messageDispatcherType));

                return dispatchExporter;
			}
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "Message Dispatcher register failed : {0}",
                    messageDispatcherType.ToString());

                return null;
            }
        }
        #endregion
    }
}
