using System;

namespace MNF.Message
{
    class AsyncSendMessage : IMessage
    {
        private int id = 0;

        public AsyncSendMessage(int id, object session, Delegate dispatcher, object messageData)
            : base(session, dispatcher, messageData)
        {
            this.id = id;
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
