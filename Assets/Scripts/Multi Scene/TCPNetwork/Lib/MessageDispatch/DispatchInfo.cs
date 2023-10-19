using System;

namespace MNF
{
    public delegate int onDispatch<in T>(T session, object message) where T : new();
    public delegate int onCustomDispatch<in T>(T message) where T : new();

    public class DispatchInfo
    {
        public Delegate dispatcher;
        public Type messageType;

        public DispatchInfo(Delegate messageDispatch, Type messageType)
        {
            this.dispatcher = messageDispatch;
            this.messageType = messageType;
        }

        public DispatchInfo(Delegate messageDispatch)
        {
            this.dispatcher = messageDispatch;
            this.messageType = null;
        }

        public override string ToString()
        {
            return $"{dispatcher.ToString()}:{messageType.ToString()}";
        }
    }
}
