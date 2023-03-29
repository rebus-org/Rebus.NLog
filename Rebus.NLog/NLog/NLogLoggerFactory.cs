using System;
using NLog;
using Rebus.Logging;

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
            _logger.Debug(message, objs);
        }

        public void Info(string message, params object[] objs)
        {
            if (!_logger.IsInfoEnabled) return;
            _logger.Info(message, objs);
        }

        public void Warn(string message, params object[] objs)
        {
            if (!_logger.IsWarnEnabled) return;
            _logger.Warn(message, objs);
        }

        public void Warn(Exception exception, string message, params object[] objs)
        {
            if (!_logger.IsWarnEnabled) return;
            _logger.Warn(exception, message, objs);
        }

        public void Error(string message, params object[] objs)
        {
            if (!_logger.IsErrorEnabled) return;
            _logger.Error(message, objs);
        }

        public void Error(Exception exception, string message, params object[] objs)
        {
            if (!_logger.IsErrorEnabled) return;
            _logger.Error(exception, message, objs);
        }
    }
}