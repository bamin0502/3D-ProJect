using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MNF
{
    public abstract class EventNofier
    {
        public abstract void notify();
    }

    public class NotEventNotifier : EventNofier
    {
        public override void notify()
        {
        }
    }

    public class DoEventNotifier : EventNofier
    {
        private AutoResetEvent messageEvent = null;

        public DoEventNotifier(AutoResetEvent messageEvent)
        {
            this.messageEvent = messageEvent;
        }

        public override void notify()
        {
            messageEvent.Set();
        }
    }
}
