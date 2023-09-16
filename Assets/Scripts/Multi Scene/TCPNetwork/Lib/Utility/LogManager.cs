using System;

namespace MNF
{
    public enum ENUM_LOG_TYPE
    {
        LOG_TYPE_SYSTEM         = 1,
        LOG_TYPE_SYSTEM_DEBUG   = 2,
        LOG_TYPE_NORMAL         = 4,
        LOG_TYPE_NORMAL_DEBUG   = 8,
        LOG_TYPE_ERROR          = 16,
        LOG_TYPE_EXCEPTION      = 32,
        LOG_TYPE_DEFAULT        = LOG_TYPE_SYSTEM + LOG_TYPE_NORMAL + LOG_TYPE_ERROR + LOG_TYPE_EXCEPTION,
        LOG_TYPE_ALL            = LOG_TYPE_SYSTEM + LOG_TYPE_SYSTEM_DEBUG + LOG_TYPE_NORMAL + LOG_TYPE_NORMAL_DEBUG + LOG_TYPE_ERROR + LOG_TYPE_EXCEPTION,
    }

	/**
     * @brief Logging interface.
     * @details The LogManager logs through this class.
     */
	public abstract class ILogWriter
    {
		/**
         * @brief ILogWriter initialization function.
         * @details When the LogManager's Init () function is called, it is called internally.
         * @return Returns true if initialization is successful, false if failure occurs.
         */
		public abstract bool OnInit();

		/**
         * @brief A function that records the log.
         * @details
         * Depending on the class that inherits ILogWriter, logs can be written in a variety of ways, 
         * including files, consoles, and sockets.
         * @param logType Specifies the log level. You can set it according to the situation.
         * @param logString Log to log.
         * @return Returns true if write is successful, false if failure occurs.
         */
		public abstract bool OnWrite(ENUM_LOG_TYPE logType, string logString);

		/**
         * @brief Releases the resources allocated to the ILogWriter.
         */
		public abstract void OnRelease();
    }

	/**
     * @brief Used to record logs.
     * @details MNF logs all logs through this class.
     */
	public class LogManager : Singleton<LogManager>
    {
        ILogWriter logWriter_ = null;
        int writeLogInfo_ = 0;

		/**
         * @brief Check whether the log is recorded.
         * @param logType Log type to log.
         * @return If true is returned, the log is recorded; otherwise, false is returned.
         */
		public bool IsWriteLog(ENUM_LOG_TYPE logType)
        {
            if ((writeLogInfo_ & (int)logType) == 0)
                return false;
            return true;
        }

		/**
         * @brief Sets the logging type of the ILogWriter to use in the LogManager.
         * @param logWriter The ILogWriter that the LogManager will use.
         * @param writeLogInfo Log type information that the LogManager will log.
         */
		public void SetLogWriter(ILogWriter logWriter, int writeLogInfo = (int)ENUM_LOG_TYPE.LOG_TYPE_DEFAULT)
        {
            logWriter_ = logWriter;
            writeLogInfo_ = writeLogInfo;
        }

		/**
         * @brief Initialize the LogManager.
         * @details Call ILogWriter's OnInit () function.
         * @return Returns true if initialization is successful, false if failure occurs.
         */
		public bool Init()
        {
            if (logWriter_ == null)
                return false;

            return logWriter_.OnInit();
        }

		/**
         * @brief Releases the resources allocated to the LogManager and ILogWriter.
         */
		public void Release()
        {
            if (logWriter_ == null)
                return;

            logWriter_.OnRelease();
            logWriter_ = null;
        }

        public void Write(string formatString, params object[] values)
        {
            if (logWriter_ == null)
                return;

            if ((writeLogInfo_ & (int)ENUM_LOG_TYPE.LOG_TYPE_NORMAL) == 0)
                return;

            logWriter_.OnWrite(ENUM_LOG_TYPE.LOG_TYPE_NORMAL, string.Format(formatString, values));
        }

        public void WriteDebug(string formatString, params object[] values)
        {
            if (logWriter_ == null)
                return;

            if ((writeLogInfo_ & (int)ENUM_LOG_TYPE.LOG_TYPE_NORMAL_DEBUG) == 0)
                return;

            logWriter_.OnWrite(ENUM_LOG_TYPE.LOG_TYPE_NORMAL_DEBUG, string.Format(formatString, values));
        }

        public void WriteSystem(string formatString, params object[] values)
        {
            if (logWriter_ == null)
                return;

            if ((writeLogInfo_ & (int)ENUM_LOG_TYPE.LOG_TYPE_SYSTEM) == 0)
                return;

            logWriter_.OnWrite(ENUM_LOG_TYPE.LOG_TYPE_SYSTEM, string.Format(formatString, values));
        }

        public void WriteSystemDebug(string formatString, params object[] values)
        {
            if (logWriter_ == null)
                return;

            if ((writeLogInfo_ & (int)ENUM_LOG_TYPE.LOG_TYPE_SYSTEM_DEBUG) == 0)
                return;

            logWriter_.OnWrite(ENUM_LOG_TYPE.LOG_TYPE_SYSTEM_DEBUG, string.Format(formatString, values));
        }

        public void WriteError(string formatString, params object[] values)
        {
            if (logWriter_ == null)
                return;

            if ((writeLogInfo_ & (int)ENUM_LOG_TYPE.LOG_TYPE_ERROR) == 0)
                return;

            logWriter_.OnWrite(ENUM_LOG_TYPE.LOG_TYPE_ERROR, string.Format(formatString, values));
        }

        public void WriteException(Exception e, string formatString, params object[] values)
        {
            if (logWriter_ == null)
                return;

            if ((writeLogInfo_ & (int)ENUM_LOG_TYPE.LOG_TYPE_EXCEPTION) == 0)
                return;

            logWriter_.OnWrite(ENUM_LOG_TYPE.LOG_TYPE_EXCEPTION, string.Format("{0}\n **[Exception]**{1}", 
                string.Format(formatString, values), e.ToString()));
        }
    }
}
