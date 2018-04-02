using System;
using NLog;
using Rebus.Logging;
using LogLevel = NLog.LogLevel;

namespace Rebus.NLog.NLog
{
    class NLogLoggerFactory : AbstractRebusLoggerFactory
    {
        protected override ILog GetLogger(Type type)
        {
            return new NLogLogger(LogManager.GetLogger(type.FullName), this);
        }

        class NLogLogger : ILog
        {
            readonly Logger _logger;
            readonly NLogLoggerFactory _loggerFactory;

            public NLogLogger(Logger logger, NLogLoggerFactory loggerFactory)
            {
                _logger = logger;
                _loggerFactory = loggerFactory;
            }

            public void Debug(string message, params object[] objs)
            {
                Log(LogLevel.Debug, null, message, objs);
            }

            public void Info(string message, params object[] objs)
            {
                Log(LogLevel.Info, null, message, objs);
            }

            public void Warn(string message, params object[] objs)
            {
                Log(LogLevel.Warn, null, message, objs);
            }

            public void Warn(Exception exception, string message, params object[] objs)
            {
                Log(LogLevel.Warn, exception, message, objs);
            }

            public void Error(string message, params object[] objs)
            {
                Log(LogLevel.Error, null, message, objs);
            }

            public void Error(Exception exception, string message, params object[] objs)
            {
                Log(LogLevel.Error, exception, message, objs);
            }

            private void Log(LogLevel logLevel, Exception exception, string message, object[] objs)
            {
                // Skip RenderString if no one is listening
                if (_logger.IsEnabled(logLevel))
                {
                    // Allow NLog callsite to work, by providing typeof(NLogLogger)
                    _logger.Log(typeof(NLogLogger), LogEventInfo.Create(logLevel, _logger.Name, exception, null, _loggerFactory.RenderString(message, objs)));
                }
            }
        }
    }
}
