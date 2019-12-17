using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitVent.Common.Extensions;
using log4net;
using log4net.Core;
using log4net.Config;
using System.IO;
using System.Configuration;

namespace iMFAS.Services.Logger
{
    /// <summary>
    /// This is used for log from different parts of the application. 
    /// This has several log level lile Debug, Info, Warn, Error, Fatal.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LoggerService<T>
    {
        #region variables
        private readonly ILog logger;
        #endregion


        #region constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public LoggerService()
        {
            /*Read configuration at the first call of this class */
            if (LogManager.GetCurrentLoggers().Length == 0)
            {
                string logConfigSubFolder = ConfigurationManager.AppSettings["LoggerConfigSubFolderLocation"];
                if(string.IsNullOrWhiteSpace(logConfigSubFolder))
                    logConfigSubFolder="\\bin\\";
                XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + logConfigSubFolder + "log4net.xml"));
            }
            logger = LogManager.GetLogger(typeof(T));
        }
        #endregion


        #region public methods
        /// <summary>
        /// This will log the information according to the message object which has the message, level and exceptions
        /// </summary>
        /// <param name="message">message object</param>
        public void Log(LogMessage message)
        {
            Log(message.LogLevel, message.JoinMessage(), message.Exception);
        }


        /// <summary>
        /// This is used for log information according to the provided log level and message
        /// </summary>
        /// <param name="logLevel">Level of the log</param>
        /// <param name="message">The log message as string</param>
        public void Log(EnumLogLevel logLevel, String message)
        {
            Log(logLevel, message, null);
        }

        /// <summary>
        /// This is used for log information according to the provided log level, message and exception
        /// </summary>
        /// <param name="logLevel">Level of the log</param>
        /// <param name="message">The log message as string</param>
        /// <param name="exception">The exception which need to write</param>
        public void Log(EnumLogLevel logLevel, String message, Exception exception)
        {
            switch (Enum.GetName(typeof(EnumLogLevel), logLevel))
            {
                case "Info":
                    if (exception == null)
                        logger.Info(message);
                    else
                        logger.Info(message, exception);
                    break;
                case "Debug":
                    if (exception == null)
                        logger.Debug(message);
                    else
                        logger.Debug(message, exception);
                    break;
                case "Warn":
                    if (exception == null)
                        logger.Warn(message);
                    else
                        logger.Warn(message, exception);
                    break;
                case "Error":
                    if (exception == null)
                        logger.Error(message);
                    else
                        logger.Error(message, exception);
                    break;
                case "Fatal":
                    if (exception == null)
                        logger.Fatal(message);
                    else
                        logger.Fatal(message, exception);
                    break;
                case "InitInfo":
                    Level initLevel=LogManager.GetRepository().LevelMap["INIT"];
                    if (exception == null)
                        logger.Logger.Log(typeof(T).DeclaringType, initLevel, message, null);
                    else
                        logger.Logger.Log(typeof(T).DeclaringType, initLevel, message, exception);
                    break;
                case "SqlInfo":
                    Level sqlLevel = LogManager.GetRepository().LevelMap["SQL"];
                    if (exception == null)
                        logger.Logger.Log(typeof(T), sqlLevel, message, null);
                    else
                        logger.Logger.Log(typeof(T), sqlLevel, message, exception);
                    break;
                case "CacheInfo":
                    Level cacheLevel = LogManager.GetRepository().LevelMap["CACHE"];
                    if (exception == null)
                        logger.Logger.Log(typeof(T), cacheLevel, message, null);
                    else
                        logger.Logger.Log(typeof(T), cacheLevel, message, exception);
                    break;
                default:
                    var defaultLevel = EnumLogLevel.Warn;
                    
                    logger.Error("Logging not defined for log level " + logLevel + "; defaulting to log level " + defaultLevel.ToString());
                    Log(defaultLevel, message, exception);
                    break;
            }
        }

        #endregion
    }//class


    /// <summary>
    /// This is data container class for log message. 
    /// This has the property for log leveol, message and exception.
    /// </summary>
    public class LogMessage : SortedDictionary<String, Object>
    {
        #region properties
        public EnumLogLevel LogLevel { get; set; }
        public Exception Exception { get; set; }
        #endregion


        #region constructor
        public LogMessage(EnumLogLevel logLevel, Exception exception = null)
        {
            LogLevel = logLevel;
            Exception = exception;
        }
        #endregion


        #region public helper methods
        public String JoinMessage()
        {
            return String.Join(", ", this.Select(kvp => kvp.Key + " = " + kvp.Value));
        }

        public LogMessage Clone(EnumLogLevel logLevel, Exception exception = null)
        {
            var copy = new LogMessage(logLevel, exception ?? this.Exception);

            copy.AddAll(this);

            return copy;
        }
        #endregion

    }//class
}//ns
