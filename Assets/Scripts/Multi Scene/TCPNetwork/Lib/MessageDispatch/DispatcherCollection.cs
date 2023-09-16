using System.Collections.Generic;
using System.Threading;

namespace MNF
{
    enum DISPATCH_TYPE
    {
        DISPATCH_NONE,
        DISPATCH_NETWORK_INTERNAL,  // For Tcp/Internal
        DISPATCH_UDP,               // For Udp
        DISPATCH_DB,                // For DB
        DISPATCH_SEND,              // For Async Send
    }

	/**
     * @brief The class that manages the Dispatcher.
     * @details Dispatcher and Thread can be connected.
     */
	class DispatcherThread
	{
		/**
		 * @brief DispatcherThread Constructor
		 * @details Depending on the option, you can attach threads to the dispatcher.
		 * @param dispatchType DISPATCH_TYPE
		 * @param isRunThread
		 * If true, the dispatcher creates an individual thread for message processing, and if false, does not create a thread.
		 */
		public DispatcherThread(DISPATCH_TYPE dispatchType, bool isRunThread)
		{
			MessageEvent = new AutoResetEvent(false);
			Dispatcher = new Dispatcher(new DoEventNotifier(MessageEvent), dispatchType);
			if (isRunThread == true)
			{
				ThreadAdapter = new ThreadAdapter(MessageEvent);
				ThreadAdapter.ThreadEvent += Dispatcher.dispatchMessage;
			}
		}

		AutoResetEvent MessageEvent { get; set; }
		public Dispatcher Dispatcher { get; private set; }
		public ThreadAdapter ThreadAdapter { get; private set; }
	}

	/**
     * @brief Manages a dispatcher set.
     * @details
     * There are various dispatchers used by MNF, all of which are managed in this class.
     * You can also create or send a message object to send to a specific dispatcher.
     */
	public class DispatcherCollection : Singleton<DispatcherCollection>
    {
        Dictionary<DISPATCH_TYPE, DispatcherThread> dispatcherThreads = new Dictionary<DISPATCH_TYPE, DispatcherThread>();

		/**
         * @brief Create and start the dispatcher.
         * @param dispatchType DISPATCH_TYPE to start.
         * @param isRunThread 
         * If true, the dispatcher creates an individual thread for message processing, and if false, does not create a thread.
         * @return Started Dispatcher.
         */
		internal Dispatcher Start(DISPATCH_TYPE dispatchType, bool isRunThread)
        {
			if (dispatcherThreads.ContainsKey(dispatchType) == true)
				return dispatcherThreads[dispatchType].Dispatcher;

			var dispatcherThread = new DispatcherThread(dispatchType, isRunThread);
			dispatcherThreads.Add(dispatchType, dispatcherThread);

			if (isRunThread == true)
				dispatcherThread.ThreadAdapter.Start();

			return dispatcherThread.Dispatcher;
		}

		/**
         * @brief Stops all Dispatchers that were started via DispatcherCollection.Start ().
         */
		internal void Stop()
		{
			foreach (var dispatcherThread in dispatcherThreads)
			{
				if (dispatcherThread.Value.ThreadAdapter != null)
					dispatcherThread.Value.ThreadAdapter.Stop();
			}
		}

		/**
         * @brief Send a message with the DISPATCH_TYPE received as an argument.
         * @param dispatchType Dispatcher to receive messages.
         * @param message The message to send.
         * @return boolean True if sending succeeds, false if it fails.
         */
		internal bool PushMessage(DISPATCH_TYPE dispatchType, IMessage message)
        {
            DispatcherThread dispatcherThread = null;
            if (dispatcherThreads.TryGetValue(dispatchType, out dispatcherThread) == false)
                return false;
            return dispatcherThread.Dispatcher.pushMessage(message);
        }

		/**
         * @brief Returns the specified dispatcher.
         * @param dispatchType The dispatchType to return.
         * @return Dispatcher Returns the specified dispatcher.
         */
		internal Dispatcher GetDispatcher(DISPATCH_TYPE dispatchType)
        {
            DispatcherThread dispatcherThread = null;
            if (dispatcherThreads.TryGetValue(dispatchType, out dispatcherThread) == false)
                return null;
            return dispatcherThread.Dispatcher;
        }
    }
}
