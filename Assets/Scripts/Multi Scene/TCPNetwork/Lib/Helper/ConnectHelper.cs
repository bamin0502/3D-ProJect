using System;
using System.Net.Sockets;

namespace MNF
{
    static class ConnectHelper
    {
		/**
         * @brief Make an asynchronous connection with the specified ip and port.
         * @details
         * When the connection is completed, TSession's OnConnectSuccess () function is called,
         * If unsuccessful, TSession's OnConnectFail () is called.
         * @param TSession An MNF network object that inherits SessionBase.
         * @param TDispatcher The object that will handle the TSession's message.
         * @param ipString The IP of the server to connect to.
         * @param portString Port of the server to be connected.
         * @return Return TSession.
         */
		static public TSession AsyncConnect<TSession, TDispatcher>(string ipString, string portString)
            where TSession : SessionBase, new()
        {
            return _Connect<TSession, TDispatcher>(ipString, portString);
        }

        static TSession _Connect<TSession, TDispatcher>(string ipString, string portString)
            where TSession : SessionBase, new()
        {
            Type dispatchExporterType = typeof(TDispatcher);

            try
            {
                var dispatchExporter = DispatchExporterCollection.Instance.Get(dispatchExporterType);
                if (dispatchExporter == null)
                {
                    DispatchExporterCollection.Instance.Add(dispatchExporterType);
                    dispatchExporter = DispatchExporterCollection.Instance.Get(dispatchExporterType);
                }

                if (dispatchExporter.Init() == false)
                    throw new Exception(string.Format("{0} init failed", dispatchExporter.ToString()));

                var targetIPEndPoint = Utility.GetIPEndPoint(ipString, portString);
                if (targetIPEndPoint == null)
                    throw new Exception("IP EndPoint is invalid");

                var session = new TSession() as TCPSession;
                if (session == null)
                    throw new Exception("SessionType is invalid");

                session.Init();

                Socket socket = new Socket(targetIPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                session.SetNetworkReady(socket, dispatchExporter);

                if (AsyncIO.Connect(session, ipString, portString) == false)
                    throw new Exception("Async Connect failed");

                return session as TSession;
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "Connect failed, IP({0}), Port({1})", ipString, portString);
                return null;
            }
        }
    }
}
