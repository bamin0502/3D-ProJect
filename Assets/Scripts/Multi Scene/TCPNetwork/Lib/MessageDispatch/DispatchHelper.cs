using System;
using System.Collections.Generic;
using System.Reflection;

namespace MNF
{
	/**
     * @brief Process the received message.
     * @details
     * The top-level class that handles incoming messages. 
     * All messages received from MNF are handled by the class that inherits this class.
     */
	public abstract class IDispatchHelper
    {
        protected Dictionary<int, DispatchInfo> dispatchList;
        bool isInit;

        public IDispatchHelper()
        {
            dispatchList = new Dictionary<int, DispatchInfo>();
        }

		/**
         * @brief Initialize the IDispatchHelper.
         * @return Returns true if initialization is successful, false if it fails.
         */
		public bool Init()
        {
            if (isInit == true)
                return true;

            isInit = OnInit();

            return isInit;
        }

		/**
         * @brief Gets the function that will handle the messageType.
         * @param messageType Gets the function that will handle the messageType.
         * @return Returns a DispatchInfo if there is a function to handle the messageType, or null if there is none.
         */
		public DispatchInfo TryGetMessageDispatch(int messageType)
        {
            if (dispatchList.ContainsKey(messageType) == false)
                return null;

            DispatchInfo dispatchInfo = null;
            if (dispatchList.TryGetValue(messageType, out dispatchInfo) == false)
                return null;

            return dispatchInfo;
        }

		/**
         * @brief Create a message object that stores the information needed to process the messageType.
         * @param messageType Message to process.
         * @param target The target object that receives the message.
         * @param data Received message data.
         * @return Returns IMessage if there is a dispatch to handle the messageType, null otherwise.
         */
		public IMessage TryCreateMessage<TTarget, TData>(int messageType, TTarget target, TData data)
        {
            DispatchInfo dispatchInfo = TryGetMessageDispatch(messageType);
            if (dispatchInfo == null)
                return null;

            return AllocMessage(target, dispatchInfo.dispatcher, data);
        }

		/**
         * @brief Create a message object to process the data.
         * @param Who should receive the message.
         * @param dispatcher The function to process the message.
         * @param data Received message data.
         * @return Returns IMessage.
         */
		public IMessage CreateMessage<TTarget, TData>(TTarget target, Delegate dispatcher, TData data)
        {
            return AllocMessage(target, dispatcher, data);
        }

        protected abstract bool OnInit();
        public abstract IMessage AllocMessage<TTarget, TData>(TTarget target, Delegate dispatcher, TData data);
        public abstract IMessage AllocMessage<TData>(Delegate dispatcher, TData data);
    }

	/**
     * @brief Process the received message.
     * @details DispatchHelper is a class that handles messages received by TSession.
     * @param TSession A session to send and receive messages.
     * @param TMessage The message object to use when passing messages to the TSession. It is implemented as Command Patter.
     * @param TDelegate A delegate for the message function to be processed by TSession.
     */
	public abstract class DispatchHelper<TSession, TMessage, TDelegate> : IDispatchHelper
        where TSession : new()
        where TMessage : IMessage, new()
	{
		/**
         * @brief Extract the message defined in TEnum as a function.
         * @details
         * Associate the function that will process the message defined in TEnum with the member function of DispatchHelper class.
         * @return boolean The function to process the message returns true if the extraction succeeds, or False if the extraction fails.
         */
		public bool ExportFunctionFromEnum<TEnum>()
		{
			var dict = Utility.EnumDictionary<TEnum>();
			foreach (var enumMessage in dict)
			{
				string functionName = "on" + enumMessage.Value;
				try
				{
                    Delegate loadedFunction = Utility.LoadDelegate(this, functionName, typeof(TDelegate));
					if (loadedFunction == null)
						throw new Exception(string.Format("{0} Dispatch Create Failed", functionName));

					DispatchInfo dispatchInfo = TryGetMessageDispatch(enumMessage.Key);
					if (dispatchInfo == null)
						dispatchList.Add(enumMessage.Key, new DispatchInfo(loadedFunction));
					else
						dispatchInfo.dispatcher = loadedFunction;
				}
				catch (Exception e)
				{
					LogManager.Instance.WriteException(e, "Enum({0}) EnumValue({1}) Function({2}) Exception({3}) from {4}",
							typeof(TEnum).ToString(), enumMessage.Value, functionName, e.ToString(), ToString());
					return false;
				}
			}
			return true;
		}

		/**
         * @brief Extract the message class defined in TClass based on the message defined in TEnum.
         * @details
         * Extract the message class defined in TClass through TEnum.
         * Then we associate the function that handles the extracted class with the inherited class of DispatchHelper.
         * @return boolean The function to process the message returns true if the extraction succeeds, or False if the extraction fails.
         */
		public bool ExportClassFromEnum<TClass, TEnum>()
		{
			Assembly mscorlib = Utility.GetAssembly(typeof(TClass));
			var dict = Utility.EnumDictionary<TEnum>();
			foreach (var enumMessage in dict)
			{
				string packetName = typeof(TClass) + "+PACK_" + enumMessage.Value;
				try
				{
					Type messageType = null;
					foreach (Type type in mscorlib.GetTypes())
					{
						if (type.ToString() == packetName)
						{
							messageType = type;
							break;
						}
					}

					if (messageType == null)
						throw new Exception(string.Format("{0} Packet Invalid", packetName));

					DispatchInfo dispatchInfo = TryGetMessageDispatch(enumMessage.Key);
					if (dispatchInfo == null)
					{
						dispatchInfo = new DispatchInfo(null);
                        dispatchInfo.messageType = messageType;
						dispatchList.Add(enumMessage.Key, dispatchInfo);
					}
					else
					{
						dispatchInfo.messageType = messageType;
					}
				}
				catch (Exception e)
				{
					LogManager.Instance.WriteException(e, "Enum({0}) EnumValue({1}) PacketName({2}) Exception({3})",
							typeof(TClass).ToString(), enumMessage.Value, packetName, e.ToString());
					return false;
				}
			}
			return true;
		}

		/**
         * @brief DispatchHelper creates a message to process.
         * @param target Who should receive the message.
         * @param dispatcher The function to process the message.
         * @param data Message data to process.
         * @return IMessage Assigned messages.
         */
		public override IMessage AllocMessage<TTarget, TData>(TTarget target, Delegate dispatcher, TData data)
        {
            IMessage message = new TMessage();
            message.Session = target;
            message.Dispatcher = dispatcher;
            message.MessageData = data;
            return message;
        }

		/**
         * @brief DispatchHelper creates a message to process.
         * @param dispatcher The function to process the message.
         * @param data Message data to process.
         * @return IMessage Assigned messages.
         */
		public override IMessage AllocMessage<TData>(Delegate dispatcher, TData data)
        {
            IMessage message = new TMessage();
            message.Session = null;
            message.Dispatcher = dispatcher;
            message.MessageData = data;
            return message;
        }
    }

	/**
     * @brief Process the received message.
     * @details DefaultDispatchHelper is a class that handles messages received by TSession.
     * @param TSession A session to send and receive messages.
     * @param TMessageClass The class in which the message is defined.
     * @param TMessageEnum The enum where the message is defined.
     */
	public abstract class DefaultDispatchHelper<TSession, TMessageClass, TMessageEnum> : DispatchHelper<TSession, DefaultMessage<TSession>, onDispatch<TSession>>
		where TSession : new()
	{
		/**
         * @brief Called when MNF.IDispatchHelper is initialized.
         * @details
         * OnInit () is called when the MNF.IDispatchHelper class is Init ().
         * 
         * @return bool Returns true if OnInit () succeeds, false if it fails.
         */
		protected override bool OnInit()
		{
			// Extract the message defined in TMessageEnum.
			if (ExportFunctionFromEnum<TMessageEnum>() == false)
				return false;

			// The message object defined in the TMessageClass class and the message defined in the TMessageEnum are extracted and combined.
			if (ExportClassFromEnum<TMessageClass, TMessageEnum>() == false)
				return false;

			return true;
		}
	}

	/**
     * @brief Process the received message.
     * @details CustomDispatchHelper is a class that delivers a message to a Custom object.
     * @param Custom Cutsom is a user-defined class that is not an object for network communication.
     */
	public abstract class CustomDispatchHelper<Custom> : DispatchHelper<object, CustomMessage<Custom>, onCustomDispatch<Custom>>
        where Custom : new()
    {
		/**
         * @brief DispatchHelper creates a message to process.
         * @param target Who should receive the message.
         * @param dispatcher The function to process the message.
         * @param data Message data to process.
         * @return IMessage Assigned messages.
         */
		public override IMessage AllocMessage<TTarget, TData>(TTarget target, Delegate dispatcher, TData data)
        {
            IMessage message = new CustomMessage<Custom>();
            message.Session = target;
            message.Dispatcher = dispatcher;
            message.MessageData = data;
            return message;
        }

		/**
         * @brief DispatchHelper creates a message to process.
         * @param dispatcher The function to process the message.
         * @param data Message data to process.
         * @return Assigned messages.
         */
		public override IMessage AllocMessage<TData>(Delegate dispatcher, TData data)
        {
            IMessage message = new CustomMessage<Custom>();
            message.Session = null;
            message.Dispatcher = dispatcher;
            message.MessageData = data;
            return message;
        }
    }
}
