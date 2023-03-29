using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Messages;
using Rebus.Tests.Contracts;
using Rebus.Tests.Contracts.Utilities;
using Rebus.Transport.InMem;
// ReSharper disable AccessToDisposedClosure

namespace Rebus.NLog.Tests;

[TestFixture]
public class ContextVariableWorks : FixtureBase
{
    [Test]
    public async Task IncludesCorrelationIdInTheThreeLoggedLines()
    {
        // ${basedir}/logs/logfile.log

        var logFilePath = Path.Combine(AppContext.BaseDirectory, "logs", "logfile.log");

        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath);
        }

        using var activator = new BuiltinHandlerActivator();
        using var counter = new SharedCounter(1);

        var logger = LogManager.GetLogger("test");

        activator.Handle<string>(async _ =>
        {
            logger.Info("1");

            await Task.Delay(100);

            logger.Info("2");

            await Task.Delay(100);

            logger.Info("3");

            counter.Decrement();
        });

        Configure.With(Using(activator))
            .Logging(l => l.NLog(correlationIdPropertyName: "rebus-correlation-id"))
            .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "test"))
            .Start();

        var headers = new Dictionary<string, string>
        {
            {Headers.CorrelationId, "known-correlation-id" }
        };

        await activator.Bus.SendLocal("hej med dig min ven!!!", headers);

        counter.WaitForResetEvent();

        await WaitForFile(logFilePath);

        LogManager.Flush();
        LogManager.Shutdown();

        var loggedLines = File.ReadAllLines(logFilePath).ToList();

        AssertLineIsThere(loggedLines, "1|known-correlation-id");
        AssertLineIsThere(loggedLines, "2|known-correlation-id");
        AssertLineIsThere(loggedLines, "3|known-correlation-id");
    }

    static async Task WaitForFile(string logFilePath)
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var cancellationToken = cancellationTokenSource.Token;

        try
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (File.Exists(logFilePath)) return;

                await Task.Delay(millisecondsDelay: 200, cancellationToken);
            }
        }
        catch
        {
            throw new TimeoutException($"The file '{logFilePath}' did not appear within 10 s");
        }
    }

    static void AssertLineIsThere(List<string> loggedLines, string expectedLine)
    {
        Assert.That(loggedLines.Any(l => l.Contains(expectedLine)), Is.True,
            $@"The expected log line '{expectedLine}' was not present:

This is what I found:
---------------------------------------------------------------
{string.Join(Environment.NewLine, loggedLines)}
---------------------------------------------------------------

");
    }
}