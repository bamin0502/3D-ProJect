using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using MNF.Message;

namespace MNF
{
    public enum SessionType
    {
        SESSION_TCP,
        SESSION_UDP,
    }

    public abstract class SessionBase
    {
        public CircularBuffer RecvCircularBuffer { get; protected set; }

        bool isConnected = false;
        string logString = "";

        public object TargetLink { get; set; }
        public Socket Socket { get; set; }
        public IPEndPoint EndPoint { get; set; }

        public IDispatchHelper DispatchHelper { get; set; }
        public IMessageFactory MessageFactory { get; protected set; }

        public SessionType SessionType { get; protected set; }

        public bool IsConnected
        {
            get { return (this.isConnected && this.Socket.Connected); }
            set { this.isConnected = value; }
        }

        public override string ToString()
        {
            if (logString.Length == 0)
                logString = RuntimeHelpers.GetHashCode(this).ToString();
            return logString;
        }

        internal abstract void Init();

        public virtual int OnAccept()
        {
            throw new NotImplementedException();
        }
        public virtual int OnConnectSuccess()
        {
            throw new NotImplementedException();
        }
        public virtual int OnConnectFail()
        {
            throw new NotImplementedException();
        }
        public virtual int OnDisconnect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            if (Socket == null)
                return;

            if (IsConnected == true)
            {
                IsConnected = false;
                Socket.Shutdown(SocketShutdown.Both);
            }
#if !NETFX_CORE
            Socket.Close();
#else
            Socket.Dispose();
#endif
        }
    }
}
