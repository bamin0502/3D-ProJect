using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace MNF
{
	/**
     * @brief Helps the client connect to the server.
     */
	abstract class IAcceptHelper : IDisposable
    {
        public Type SessionType { get; protected set; }
        public Type DispatchExporterType { get; protected set; }

        public string IP { get; protected set; }
        public string Port { get; protected set; }
        public Socket ListenSocket { get; protected set; }

		/**
         * @brief Start accept.
         * @details Internally, it calls AsyncIO.StartAccept () to get the client's connection.
         * @return Returns true if it succeeds, false if it fails.
         */
		virtual public bool StartAccept()
        {
            return AsyncIO.StartAccept(null, this);
        }

		/**
         * @brief Function to actually generate IAcceptHelper.
         * @details 
         * Create a dispatcher to handle messages sent by connected clients.
         * And Socket creation, listening, and bind operations required to connect to the server are described here.
         * @param ipString The IP of the server.
         * @param portString Port of the server.
         * @param backLog The size of the queue to accept client connections.
         * @param dispatchExporterType The dispatcher to handle messages sent by connected clients.
         * @return Returns true if it succeeds, false if it fails.
         */
		protected bool _Create(string ipString, string portString, int backLog, Type dispatchExporterType)
        {
            IP = ipString;
            Port = portString;

            if (DispatchExporterCollection.Instance.Get(dispatchExporterType) == null)
            {
                if (DispatchExporterCollection.Instance.Add(dispatchExporterType) == false)
                    throw new Exception(string.Format("{0} is not DispatchHelper", dispatchExporterType));
            }

            if (DispatchExporterCollection.Instance.Get(dispatchExporterType).Init() == false)
                throw new Exception(string.Format("{0} init failed", dispatchExporterType));

            try
            {
                IPEndPoint ipEndpoint = Utility.GetIPEndPoint(ipString, portString);
                if (ipEndpoint == null)
                    throw new Exception("IP EndPoint is invalid");

                ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

#if !NETFX_CORE
                ListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
#endif

                ListenSocket.Bind(ipEndpoint);
                ListenSocket.Listen(backLog);

                return true;
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "Accept failed, IP({0}), Port({1})", ipString, portString);
                return false;
            }
        }

		/**
         * @brief Create a TSession object.
         * @return Returns the created SessionBase.
         */
		internal abstract object AllocSession();

#region IDisposable Support
        // Flag: Has Dispose already been called?
        bool disposedValue = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue == true)
                return;

            disposedValue = true;
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~IAcceptHelper()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }
#endregion
    }

	/**
     * @brief Helps the client connect to the server.
     * @param TSession An MNF network object that inherits SessionBase.
     * @param TDispatcher The object that will handle the TSession's message.
     */
	class AcceptHelper<TSession, TDispatcher> : IAcceptHelper
        where TSession : SessionBase, new()
    {
		/**
         * @brief Function to actually generate IAcceptHelper.
         * @details 
         * Create a dispatcher to handle messages sent by connected clients.
         * And Socket creation, listening, and bind operations required to connect to the server are described here.
         * @param ipString The IP of the server.
         * @param portString Port of the server.
         * @param backLog The size of the queue to accept client connections.
         * @return Returns true if it succeeds, false if it fails.
         */
		public bool Create(string ipString, string portString, int backLog)
        {
            SessionType = typeof(TSession);
            DispatchExporterType = typeof(TDispatcher);

            return _Create(ipString, portString, backLog, typeof(TDispatcher));
        }

		/**
         * @brief Create a TSession object.
         * @return Returns the created TSession.
         */
		internal override object AllocSession()
        {
            return new TSession();
        }
    }

	/**
     * @brief Helps the client connect to the server.
     * @param TSession An MNF network object that inherits SessionBase.
     * @param TDispatcher The object that will handle the TSession's message.
     */
	abstract class ISsslAcceptHelper : IAcceptHelper
	{
		public X509Certificate2 ClientCertificate { get; private set; }
		public X509Certificate2Collection CertificatesCollection { get; private set; }

		public bool CreateCertificate2(string certificatePath, string passwd)
		{
			try
			{
				ClientCertificate = new X509Certificate2(certificatePath, passwd);
				CertificatesCollection = new X509Certificate2Collection(ClientCertificate);
                return true;
			}
			catch (Exception e)
			{
                LogManager.Instance.WriteException(e, "Failed to create certificate2 {0}, {1}", certificatePath, passwd);
				return false;
			}
		}
	}

	/**
     * @brief Helps the client connect to the server.
     * @param TSession An MNF network object that inherits SessionBase.
     * @param TDispatcher The object that will handle the TSession's message.
     */
 //   class SsslAcceptHelper<TSession, TDispatcher> : ISsslAcceptHelper
	//	where TSession : SessionBase, new()
	//{
 //       public bool Create(string ipString, string portString, int backLog, string certificatePath, string passwd)
 //       {
	//		SessionType = typeof(TSession);
	//		DispatchExporterType = typeof(TDispatcher);

 //           if (CreateCertificate2(certificatePath, passwd) == false)
 //               return false;

 //           try
 //           {
	//			return _Create(ipString, portString, backLog, typeof(TDispatcher));
 //           }
 //           catch (Exception e)
 //           {
	//			LogManager.Instance.WriteException(e, "Accept failed, IP({0}), Port({1})", ipString, portString);
	//			return false;
 //           }
	//	}

 //       /**
 //        * @brief Start accept.
 //        * @details Internally, it calls AsyncIO.StartAccept () to get the client's connection.
 //        * @return Returns true if it succeeds, false if it fails.
 //        */
 //       override public bool StartAccept()
 //       {
	//		return AsyncIO_Ssl.Accept(this);
	//	}

	//	/**
 //        * @brief Create a TSession object.
 //        * @return Returns the created TSession.
 //        */
	//	internal override object AllocSession()
	//	{
	//		return new TSession();
	//	}
	//}
}
