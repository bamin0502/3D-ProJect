using System;
using System.Net;
using System.Net.Sockets;

namespace MNF
{
    public struct ParsingResult
    {
        public enum ParsingResultEnum
        {
            PARSING_NONE,
            PARSING_INCOMPLETE,
            PARSING_COMPLETE,
            PARSING_ERROR,
        };

        public ParsingResultEnum parsingResultEnum;
        public Delegate dispatcher;
        public object message;
        public int messageSize;

        public ParsingResult(ParsingResultEnum parsingResultEnum, Delegate dispatcher, object message, int messageSize)
        {
            this.parsingResultEnum = parsingResultEnum;
            this.dispatcher = dispatcher;
            this.message = message;
            this.messageSize = messageSize;
        }
    }

    class AsyncIOResult
    {
        public TCPSession session;
        public Message.Serializer serializer;
    }

    // Singleton class
    class AsyncIO
    {
        [System.ComponentModel.DefaultValue(null)]
        public static SystemMessageDispatcher SystemMessageCollection { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public static UInt64 TotalSend { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public static UInt64 TotalRecv { get; set; }

        static void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            // determine which type of operation just completed and call the associated handler
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    ProcessAccept(e);
                    break;
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                case SocketAsyncOperation.Connect:
                    ProcessConnect(e);
                    break;
                default:
                   LogManager.Instance.WriteError("sender({0}) wrong operation({1})", sender.GetType(), e.LastOperation);
                    break;
            }
        }

        // accept
        internal static bool StartAccept(SocketAsyncEventArgs acceptEventArg, IAcceptHelper accepterHelper)
        {
            try
            {
                if (acceptEventArg == null)
                {
                    acceptEventArg = new SocketAsyncEventArgs();
                    acceptEventArg.UserToken = accepterHelper;
                    acceptEventArg.Completed += IO_Completed;
                }
                else
                {
                    // socket must be cleared since the context object is being reused
                    acceptEventArg.AcceptSocket = null;
                }

                bool willRaiseEvent = accepterHelper.ListenSocket.AcceptAsync(acceptEventArg);
                if (willRaiseEvent == false)
                {
                    ProcessAccept(acceptEventArg);
                }
                return true;
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "Accept({0}) begin failed", accepterHelper.ToString());
                return false;
            }
        }
        internal static void ProcessAccept(SocketAsyncEventArgs e)
        {
            try
            {
                IAcceptHelper acceptor = (IAcceptHelper)e.UserToken;
                if (acceptor == null)
                    throw new Exception(string.Format("Acceptor is Invalid object({0})", e.GetType().Name));

                Socket clientSocket = e.AcceptSocket;
                var session = acceptor.AllocSession() as TCPSession;
                if (session == null)
                    throw new Exception(string.Format("Acceptor didn't alloc session({0})", acceptor.SessionType));

                session.Init();

                // push accept message
                var dispatchExporter = DispatchExporterCollection.Instance.Get(acceptor.DispatchExporterType);
                session.SetNetworkReady(clientSocket, dispatchExporter);
                session.IsConnected = true;
                if (session.Socket.RemoteEndPoint != null)
                    session.EndPoint = session.Socket.RemoteEndPoint as IPEndPoint;
                else
                    session.EndPoint = Utility.GetIPEndPoint(acceptor.IP, acceptor.Port);

                SystemMessageCollection.pushAcceptMessage(session);

                // start receive
                SocketAsyncEventArgs readEventArgs = new SocketAsyncEventArgs();
                readEventArgs.UserToken = session;
                readEventArgs.SetBuffer(session.AsyncRecvBuffer, 0, session.AsyncRecvBuffer.Length);
                readEventArgs.Completed += IO_Completed;

                bool willRaiseEvent = session.Socket.ReceiveAsync(readEventArgs);
                if (willRaiseEvent == false)
                    ProcessReceive(readEventArgs);
                
                LogManager.Instance.WriteSystem("Acceptor({0}) session({1}) accepted", acceptor.GetType().Name, session);
            }
            catch (Exception exception)
            {
                LogManager.Instance.WriteException(exception, "Acceptor({0}) accept result failed", e.GetType().Name);
            }

            StartAccept(e, (IAcceptHelper)e.UserToken);
        }

        // recv
        static void ProcessReceive(SocketAsyncEventArgs e)
        {
            TCPSession session = (TCPSession)e.UserToken;
            try
            {
                if (session == null)
                    throw new Exception(string.Format("Recv session({0}) is invalid", e.GetType().Name));

                if ((e.BytesTransferred == 0) || (e.SocketError != SocketError.Success))
                {
                    throw new Exception(string.Format("session({0}) recvied bytes({1}) error({2})",
                        session.ToString(), e.BytesTransferred, e.SocketError));
                }

                if (LogManager.Instance.IsWriteLog(ENUM_LOG_TYPE.LOG_TYPE_SYSTEM_DEBUG) == true)
                {
                    LogManager.Instance.WriteSystemDebug("TCPSession({0}) Recv({1}) bytes from server.",
                        session.EndPoint.ToString(), e.BytesTransferred);
                }

                TotalRecv += (UInt64)e.BytesTransferred;

                // copy async buffer to circular buffer
                if (session.DoCopyReceivedData(e.BytesTransferred) == false)
                    throw new Exception(string.Format("TCPSession({0} copy received data({1}) failed", session, e.BytesTransferred));

                bool isLoop = true;
                while (isLoop == true)
                {
                    ParsingResult parsingResult = new ParsingResult();
                    session.DeserializeMessage(ref parsingResult);
                    switch (parsingResult.parsingResultEnum)
                    {
                        case ParsingResult.ParsingResultEnum.PARSING_INCOMPLETE:
                            isLoop = false;
                            break;

                        case ParsingResult.ParsingResultEnum.PARSING_COMPLETE:
                            {
                                var message = session.DispatchHelper.CreateMessage(session, parsingResult.dispatcher, parsingResult.message);
                                DispatcherCollection.Instance.PushMessage(DISPATCH_TYPE.DISPATCH_NETWORK_INTERNAL, message);
                            }
                            break;

                        default:
                            throw new Exception(string.Format(
                                "TCPSession({0} copy received data({1}) failed", session, e.BytesTransferred));
                    }
                }

                bool willRaiseEvent = session.Socket.ReceiveAsync(e);
                if (willRaiseEvent == false)
                    ProcessReceive(e);
            }
            catch (Exception exception)
            {
                LogManager.Instance.WriteException(exception, "TCPSession({0}) recv callback proc failed", e.GetType().Name);

                if (session != null)
                    ProcSessionDisconnect(session);
            }
        }

        // Send
        internal static bool AsyncSend(TCPSession session)
        {
            return DispatcherCollection.Instance.PushMessage( DISPATCH_TYPE.DISPATCH_SEND, new Message.AsyncSendMessage(session, null));
        }
        internal static bool SyncSend<T>(TCPSession session, int id, T managedData) where T : new()
        {
            var serializer = session.MessageFactory.AllocSerializer();
            try
            {
                serializer.Serialize(id, managedData);

                var asyncIOResult = new AsyncIOResult();
                asyncIOResult.serializer = serializer;
                asyncIOResult.session = session;

                var sendEventArgs = new SocketAsyncEventArgs();
                sendEventArgs.UserToken = asyncIOResult;
                sendEventArgs.SetBuffer(serializer.GetSerializedBuffer(), 0, serializer.SerializedLength);
                sendEventArgs.Completed += IO_Completed;

                bool willRaiseEvent = session.Socket.SendAsync(sendEventArgs);
                if (willRaiseEvent == false)
                {
                    ProcessSend(sendEventArgs);
                }
                return true;
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "TCPSession({0}) packet({1}) send begin failed",
                    session.ToString(), managedData.ToString());
                session.MessageFactory.FreeSerializer(serializer);
                ProcSessionDisconnect(session);
                return false;
            }
        }
        static void ProcessSend(SocketAsyncEventArgs e)
        {
            AsyncIOResult asyncIOResult = (AsyncIOResult)e.UserToken;
            TCPSession session = asyncIOResult.session;
            try
            {
                if (session == null)
                    throw new Exception(string.Format("Send session({0}) is invalid", e.GetType().Name));

                session.MessageFactory.FreeSerializer(asyncIOResult.serializer);

                if (e.SocketError != SocketError.Success)
                    throw new Exception(string.Format("Send session({0}) is error({1})", e.GetType().Name, e.SocketError));

                int bytesSent = e.BytesTransferred;

                if (LogManager.Instance.IsWriteLog(ENUM_LOG_TYPE.LOG_TYPE_SYSTEM_DEBUG) == true)
                {
                    LogManager.Instance.WriteSystemDebug("TCPSession({0}) sent({1}) bytes to server",
                        session.EndPoint.ToString(), bytesSent);
                }

                TotalSend += (UInt64)bytesSent;

                if (bytesSent == 0)
                {
                    ProcSessionDisconnect(session);
                    return;
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.WriteException(exception, "TCPSession({0}) send callback proc failed", asyncIOResult.GetType().Name);

                if (session != null)
                    ProcSessionDisconnect(session);
            }
        }

        internal static bool Connect(TCPSession session, string ipString, string portString)
        {
            try
            {
                session.EndPoint = Utility.GetIPEndPoint(ipString, portString);
                if (session.EndPoint == null)
                    throw new Exception(string.Format("Connect EndPoint is invalid, ip({0}) port({1})", ipString, portString));

                SocketAsyncEventArgs connectEventArgs = new SocketAsyncEventArgs();
                connectEventArgs.UserToken = session;
                connectEventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ipString), Convert.ToInt32(portString));
                connectEventArgs.Completed += IO_Completed;

                bool willRaiseEvent = session.Socket.ConnectAsync(connectEventArgs);
                if (willRaiseEvent == false)
                    ProcessConnect(connectEventArgs);

                return true;
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "TCPSession({0}) connect begin failed, IP({1}), Port({2})",
                    session, ipString, portString);
                return false;
            }
        }
        static void ProcessConnect(SocketAsyncEventArgs e)
        {
            TCPSession session = (TCPSession)e.UserToken;
            try
            {
                if (session == null)
                    throw new Exception(string.Format("Invalid connect object({0})", e.GetType().Name));

                LogManager.Instance.WriteSystem("TCPSession({0}) connected to {1}", session.ToString(), session.Socket.RemoteEndPoint.ToString());

                // push connect message
                session.IsConnected = true;
                session.EndPoint = session.Socket.RemoteEndPoint as IPEndPoint;
                SystemMessageCollection.pushConnectSuccessMessage(session);

                // start receive
                SocketAsyncEventArgs readEventArgs = new SocketAsyncEventArgs();
                readEventArgs.UserToken = session;
                readEventArgs.SetBuffer(session.AsyncRecvBuffer, 0, session.AsyncRecvBuffer.Length);
                readEventArgs.Completed += IO_Completed;

                bool willRaiseEvent = session.Socket.ReceiveAsync(readEventArgs);
                if (willRaiseEvent == false)
                    ProcessReceive(readEventArgs);
            }
            catch (Exception exception)
            {
                LogManager.Instance.WriteException(exception, "Connected session({0} is invalid", e.GetType().Name);

                // push connect fail message
                if (session == null)
                    return;

                session.IsConnected = false;
                SystemMessageCollection.pushConnectFailMessage(session);
            }
        }

        static void ProcSessionDisconnect(TCPSession session)
        {
            if (session.IsNotifyDisconnect == true)
                return;

            LogManager.Instance.WriteSystem("TCPSession({0}) disconnect", session);
            session.Disconnect();
            session.IsConnected = false;
            SystemMessageCollection.pushDisconnectMessage(session);
            session.IsNotifyDisconnect = true;
        }
    }
}
