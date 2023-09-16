using System.Diagnostics;

namespace MNF
{
    internal class Dispatcher
    {
        private DISPATCH_TYPE dispatchType = DISPATCH_TYPE.DISPATCH_NONE;
        private EventNofier eventNotifier = null;
        private SwapableMessgeQueue<IMessage> dispatchMessageQueue = new SwapableMessgeQueue<IMessage>();

        public Dispatcher(EventNofier eventNotifier, DISPATCH_TYPE dispatchType)
        {
            Debug.Assert(dispatchType != DISPATCH_TYPE.DISPATCH_NONE);
            this.eventNotifier = eventNotifier;
            this.dispatchType = dispatchType;
        }

        internal bool pushMessage(IMessage message)
        {
            lock (dispatchMessageQueue)
            {
                dispatchMessageQueue.getWritableQueue().Enqueue(message);
            }
            eventNotifier.notify();
            return true;
        }

        internal void dispatchMessage(bool isSignal)
        {
            lock (dispatchMessageQueue)
            {
                if (dispatchMessageQueue.getWritableQueue().Count > 0)
                {
                    dispatchMessageQueue.swap();
                }
            }

            int dispatchCount = 0;
            while (dispatchMessageQueue.getReadableQueue().Count > 0)
            {
                var message = dispatchMessageQueue.getReadableQueue().Peek();
                var session = message.Session as SessionBase;
                if (session != null)
                {
                    if (session.SessionType == SessionType.SESSION_TCP
                        && session.IsConnected == false
                        && message.MessageData != null)
                    {
                        dispatchMessageQueue.getReadableQueue().Dequeue();
                        continue;
                    }
                }

                int returnValue = message.execute();
                if (returnValue != 0)
                {
                    if (LogManager.Instance.IsWriteLog(ENUM_LOG_TYPE.LOG_TYPE_SYSTEM_DEBUG) == true)
                    {
                        LogManager.Instance.WriteDebug("Dispatcher({0}) Message({1}) return value({2})",
                        dispatchType.ToString(), message.ToString(), returnValue);
                    }
                }

                dispatchMessageQueue.getReadableQueue().Dequeue();

                ++dispatchCount;
            }

            if (dispatchCount > 0)
            {
                if (LogManager.Instance.IsWriteLog(ENUM_LOG_TYPE.LOG_TYPE_SYSTEM_DEBUG) == true)
                {
                    LogManager.Instance.WriteSystemDebug("Dispatcher({0}) dispatch count({1})", dispatchType.ToString(), dispatchCount);
                }
            }
        }
    }
}
