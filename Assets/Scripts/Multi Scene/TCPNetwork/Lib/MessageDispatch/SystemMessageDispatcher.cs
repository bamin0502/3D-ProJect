using System;
using System.Diagnostics;

namespace MNF
{
    internal class SystemMessageDispatcher
    {
        #region SystemMessage
        public enum SYSTEM_MESSAGE_TYPE
        {
            SYSTEM_MESSAGE_CONNECT_SUCCESS,
            SYSTEM_MESSAGE_CONNECT_FAILED,
            SYSTEM_MESSAGE_ACCEPT,
            SYSTEM_MESSAGE_DISCONNECT,
        }

        public class SystemMessage : IMessage
        {
            private SYSTEM_MESSAGE_TYPE messageType;

            public SystemMessage(SYSTEM_MESSAGE_TYPE messageType, SessionBase session) : base()
            {
                this.messageType = messageType;
                this.Session = session;
            }

            public override int execute()
            {
                var session = Session as SessionBase;
                switch(messageType)
                {
                    case SYSTEM_MESSAGE_TYPE.SYSTEM_MESSAGE_CONNECT_SUCCESS:
                        session.OnConnectSuccess();
                        break;

                    case SYSTEM_MESSAGE_TYPE.SYSTEM_MESSAGE_CONNECT_FAILED:
                        session.OnConnectFail();
                        break;

                    case SYSTEM_MESSAGE_TYPE.SYSTEM_MESSAGE_ACCEPT:
                        session.OnAccept();
                        break;

                    case SYSTEM_MESSAGE_TYPE.SYSTEM_MESSAGE_DISCONNECT:
                        session.OnDisconnect();
                        break;
                }
                return 0;
            }
        }
        #endregion

        public delegate bool onSystemMessageDispatch(IMessage message);
        public event onSystemMessageDispatch SystemMessageEvent;
        private bool isInit = false;

        internal bool init()
        {
            if (isInit == true)
                return true;

            try
            {
                var dispatcher = DispatcherCollection.Instance.GetDispatcher(DISPATCH_TYPE.DISPATCH_NETWORK_INTERNAL);
                if (dispatcher == null)
                    throw new Exception("Network/Internal dispatch didn't create");

                SystemMessageEvent += dispatcher.pushMessage;
                isInit = true;

                return isInit;
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "SystemMessageDispatcher object failed to create");
                return false;
            }
        }

        public void pushAcceptMessage(SessionBase session)
        {
            var message = new SystemMessage(SYSTEM_MESSAGE_TYPE.SYSTEM_MESSAGE_ACCEPT, session);
            SystemMessageEvent(message);
        }

        public void pushConnectSuccessMessage(SessionBase session)
        {
            var message = new SystemMessage(SYSTEM_MESSAGE_TYPE.SYSTEM_MESSAGE_CONNECT_SUCCESS, session);
            SystemMessageEvent(message);
        }

        public void pushConnectFailMessage(SessionBase session)
        {
            var message = new SystemMessage(SYSTEM_MESSAGE_TYPE.SYSTEM_MESSAGE_CONNECT_FAILED, session);
            SystemMessageEvent(message);
        }

        public void pushDisconnectMessage(SessionBase session)
        {
            var message = new SystemMessage(SYSTEM_MESSAGE_TYPE.SYSTEM_MESSAGE_DISCONNECT, session);
            SystemMessageEvent(message);
        }
    }
}
