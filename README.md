# Rebus.NLog

[![install from nuget](https://img.shields.io/nuget/v/Rebus.NLog.svg?style=flat-square)](https://www.nuget.org/packages/Rebus.NLog)

Provides an NLog logger integration for [Rebus](https://github.com/rebus-org/Rebus).

![](https://raw.githubusercontent.com/rebus-org/Rebus/master/artwork/little_rebusbus2_copy-200x200.png)

## How to use it

When you configure Rebus by calling the ordinary configuration spell

```csharp
Configure.With(yourFavoriteContainerAdapter)
	.Transport(t => t.UseYourFavoriteTransport("with-this-queue"))
	.Start();
```

Rebus will default to log things to the console, using colors to differentiate between the four different log levels used.

You can make Rebus use NLog like this:

```csharp
Configure.With(yourFavoriteContainerAdapter)
	.Logging(l => l.NLog())
	.Transport(t => t.UseYourFavoriteTransport("with-this-queue"))
	.Start();
```

and that's basically it :)

Please check out [the NLog sige](http://nlog-project.org/) for details on how to configure NLog.

---


