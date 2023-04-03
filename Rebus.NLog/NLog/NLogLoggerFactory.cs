using System;
using NLog;
using Rebus.Logging;
using LogLevel = NLog.LogLevel;

namespace Rebus.NLog;

class NLogLoggerFactory : AbstractRebusLoggerFactory
{
    protected override ILog GetLogger(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        return new NLogLogger(LogManager.GetLogger(type.FullName));
    }

    class NLogLogger : ILog
    {
        readonly Logger _logger;

        public NLogLogger(Logger logger) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public void Debug(string message, params object[] objs)
        {
            if (!_logger.IsDebugEnabled) return;
            Log(LogLevel.Debug, null, message, objs);
        }

        public void Info(string message, params object[] objs)
        {
            if (!_logger.IsInfoEnabled) return;
            Log(LogLevel.Info, null, message, objs);
        }

        public void Warn(string message, params object[] objs)
        {
            if (!_logger.IsWarnEnabled) return;
            Log(LogLevel.Warn, null, message, objs);
        }

        public void Warn(Exception exception, string message, params object[] objs)
        {
            if (!_logger.IsWarnEnabled) return;
            Log(LogLevel.Warn, exception, message, objs);
        }

        public void Error(string message, params object[] objs)
        {
            if (!_logger.IsErrorEnabled) return;
            Log(LogLevel.Error, null, message, objs);
        }

        public void Error(Exception exception, string message, params object[] objs)
        {
            if (!_logger.IsErrorEnabled) return;
            Log(LogLevel.Error, exception, message, objs);
        }

        void Log(LogLevel logLevel, Exception exception, string message, object[] objs) => _logger.Log(typeof(NLogLogger), LogEventInfo.Create(logLevel, _logger.Name, exception, null, message, objs));
    }
}