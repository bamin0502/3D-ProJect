using System;

namespace MNF.Message
{
    class AsyncSendMessage : IMessage
    {
        private int id = 0;

        public AsyncSendMessage( object session, Delegate dispatcher)
            : base(session, dispatcher)
        {
        }

        public override int execute()
        {
            var tcpSession = Session as TCPSession;
            if (tcpSession.IsConnected == false)
                return -1;

            var tmpMessageData = MessageData;
            if (AsyncIO.SyncSend(tcpSession, id, tmpMessageData) == true)
                return 0;

            return -1;
        }
    }
}
