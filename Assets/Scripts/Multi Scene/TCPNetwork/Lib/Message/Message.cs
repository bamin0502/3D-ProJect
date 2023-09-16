using System;

namespace MNF
{
    public abstract class IMessage
    {
        [System.ComponentModel.DefaultValue(null)]
        public object Session { get; set; }

        [System.ComponentModel.DefaultValue(null)]
        public Delegate Dispatcher { get; set; }

        [System.ComponentModel.DefaultValue(null)]
        public object MessageData { get; set; }

        public IMessage()
        {
        }

        public IMessage(object session, Delegate dispatcher, object messageData)
        {
            Session = session;
            Dispatcher = dispatcher;
            MessageData = messageData;
        }

        public override string ToString()
        {
            if (Session == null)
                return string.Format("session(null):{0}", Dispatcher);

            if (Dispatcher == null)
                return Session.ToString();

            if (MessageData == null)
                return string.Format("{0}:{1}", Session, Dispatcher);

            return string.Format("{0}:{1}:{2}", Session, Dispatcher, MessageData);
        }

        public abstract int execute();
    }

    public abstract class IMessage<TSession> : IMessage where TSession : new()
    {
        public IMessage() : base()
        {
        }

        public IMessage(object session, Delegate dispatcher, object messageData) : base(session, dispatcher, messageData)
        {
        }
    }

    public class DefaultMessage<TSession> : IMessage
        where TSession : new()
    {
        public DefaultMessage() : base()
        {
        }

        public DefaultMessage(object session, Delegate dispatcher, object messageData) : base(session, dispatcher, messageData)
        {
        }

        public override int execute()
        {
            var tempDelegate = Dispatcher as onDispatch<TSession>;
            return tempDelegate((TSession)Session, MessageData);
        }
    }

    public class CustomMessage<TCustomMessage> : IMessage
        where TCustomMessage : new()
    {
        public CustomMessage() : base()
        {
        }

        public CustomMessage(object session, Delegate dispatcher, object messageData) : base(session, dispatcher, messageData)
        {
        }

        public override int execute()
        {
            var tempDelegate = Dispatcher as onCustomDispatch<TCustomMessage>;
            return tempDelegate((TCustomMessage)MessageData);
        }
    }
}
