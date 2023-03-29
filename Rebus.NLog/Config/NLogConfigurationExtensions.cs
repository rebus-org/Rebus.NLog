using System;
using System.Threading.Tasks;
using NLog;
using Rebus.Extensions;
using Rebus.Messages;
using Rebus.NLog;
using Rebus.Pipeline;

namespace Rebus.Config;

/// <summary>
/// Configuration extensions for setting up logging with NLog
/// </summary>
public static class NLogConfigurationExtensions
{
    /// <summary>
    /// Configures Rebus to use NLog for all of its internal logging, getting its loggers by calling logger <see cref="LogManager.GetLogger(string)"/>.
    /// If <paramref name="correlationIdPropertyName"/> is different from NULL (default: "correlationid"), an incoming pipeline step will be installed
    /// which will establish a property via <see cref="ScopeContext.PushProperty{TValue}"/> with the correlation ID of the message currently being handled.
    /// The scope property can be included in output by adding <code>${scopeproperty:&lt;correlationIdPropertyName&gt;}</code> to a layaout, e.g. like
    /// <code>${level}|${scopeproperty:correlationId}|${message}</code>
    /// </summary>
    public static void NLog(this RebusLoggingConfigurer configurer, string correlationIdPropertyName = "correlationId")
    {
        if (configurer == null) throw new ArgumentNullException(nameof(configurer));

        if (correlationIdPropertyName != null)
        {
            configurer.Decorate<IPipeline>(c =>
            {
                var pipeline = c.Get<IPipeline>();
                var step = new NLogContextStep(correlationIdPropertyName);
                return new PipelineStepConcatenator(pipeline)
                    .OnReceive(step, PipelineAbsolutePosition.Front);
            });
        }

        configurer.Use(new NLogLoggerFactory());
    }

    class NLogContextStep : IIncomingStep
    {
        readonly string _propertyName;

        public NLogContextStep(string propertyName)
        {
            _propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            var correlationid = context.Load<TransportMessage>()?.Headers.GetValueOrNull(Headers.CorrelationId);

            using var _ = correlationid != null ? ScopeContext.PushProperty(_propertyName, correlationid) : null;

            await next();
        }
    }
}
