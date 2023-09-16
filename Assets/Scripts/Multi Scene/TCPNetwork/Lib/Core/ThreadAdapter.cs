using System;
using System.Threading;
using System.Diagnostics;

#if NETFX_CORE
using System.Threading.Tasks;
using Windows.System.Diagnostics;
#endif

namespace MNF
{
    public class ThreadAdapter : IDisposable
    {
#if !NETFX_CORE
        Thread messageDispatchThread = null;
#else
        Task runTask = null;
#endif

        public delegate void ThreadEventHandler(bool isSignal);
        public event ThreadEventHandler ThreadEvent;

        AutoResetEvent messageEvent = null;
        bool isStop = false;
        int waitTime = 100;

        public int WaitTime
        {
            get { return waitTime; }
            set { waitTime = value; messageEvent.Set(); }
        }

        public ThreadAdapter(AutoResetEvent messageEvent)
        {
            this.messageEvent = messageEvent;
        }

        public bool Start()
        {
            Debug.Assert(isStop == false);

            if (messageEvent == null)
                messageEvent = new AutoResetEvent(false);

#if !NETFX_CORE
            if (messageDispatchThread != null)
                return false;

            messageDispatchThread = new Thread(RunThread);
            messageDispatchThread.Start();
#else
            runTask = Task.Factory.StartNew(() => RunThread());
#endif

            isStop = false;

            return true;
        }

        public void Stop()
        {
            isStop = true;
            messageEvent.Set();

#if !NETFX_CORE
            messageDispatchThread.Join();
#else
            runTask.Wait();
#endif
        }

        public bool IsRunning()
        {
            return (isStop == false);
        }

#if !NETFX_CORE
        void RunThread()
#else
        async void RunThread()
#endif
        {
            while (isStop == false)
            {
                ThreadEvent(messageEvent.WaitOne(waitTime));
            }
        }

        #region IDisposable Support
        bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
#if !NETFX_CORE
                    messageEvent.Close();
#else
                    messageEvent.Dispose();
#endif
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ThreadAdapter() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
