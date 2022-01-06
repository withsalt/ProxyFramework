using NLog;
using System;

namespace ProxyFramework
{
    public class Log
    {
        private static readonly NLog.Logger logger = null;

        private static readonly object _locker = new object();

        static Log()
        {
            if (logger == null)
            {
                lock (_locker)
                {
                    if (logger == null)
                    {
                        logger = LogManager.GetLogger("ProxyFramework");
                    }
                }
            }
        }

        /// <summary>
        /// 普通日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Info(string message, Exception exception = null)
        {
            if (exception == null)
                logger.Info($"[{DateTime.Now}]{message}");
            else
                logger.Info(exception, $"[{DateTime.Now}]{message}");
        }

        /// <summary>
        /// 告警日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Warn(string message, Exception exception = null)
        {
            if (exception == null)
                logger.Warn($"[{DateTime.Now}]{message}");
            else
                logger.Warn(exception, $"[{DateTime.Now}]{message}");
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Error(string message, Exception exception = null)
        {
            if (exception == null)
                logger.Error($"[{DateTime.Now}]{message}");
            else
                logger.Error(exception, $"[{DateTime.Now}]{message}");
        }
    }
}
