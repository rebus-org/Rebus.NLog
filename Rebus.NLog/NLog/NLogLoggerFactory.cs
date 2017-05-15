using System;
using NLog;
using Rebus.Logging;

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
                _logger.Debug(_loggerFactory.RenderString(message, objs));
            }

            public void Info(string message, params object[] objs)
            {
                _logger.Info(_loggerFactory.RenderString(message, objs));
            }

            public void Warn(string message, params object[] objs)
            {
                _logger.Warn(_loggerFactory.RenderString(message, objs));
            }

            public void Warn(Exception exception, string message, params object[] objs)
            {
                _logger.Warn(exception, _loggerFactory.RenderString(message, objs));
            }

            public void Error(string message, params object[] objs)
            {
                _logger.Error(_loggerFactory.RenderString(message, objs));
            }

            public void Error(Exception exception, string message, params object[] objs)
            {
                _logger.Error(exception, _loggerFactory.RenderString(message, objs));
            }
        }
    }
}
