using System;
using Cake.Core.Diagnostics;
using Microsoft.Extensions.Logging;
using LogLevel = Cake.Core.Diagnostics.LogLevel;

namespace Cake.Bridge.DependencyInjection
{
    public class CakeMicrosoftExtensionsLogging : ICakeLog
    {
        private ILogger Logger { get; }


        public CakeMicrosoftExtensionsLogging(ILogger<CakeMicrosoftExtensionsLogging> logger)
        {
            Logger = logger;
        }

        public void Write(Verbosity verbosity, LogLevel level, string format, params object[] args)
        {
            if (verbosity > Verbosity)
            {
                return;
            }

            switch(level)
            {
                case LogLevel.Fatal:
                    Logger.LogCritical(format, args);
                    break;
                case LogLevel.Error:
                    Logger.LogError(format, args);
                    break;
                case LogLevel.Warning:
                    Logger.LogWarning(format, args);
                    break;
                case LogLevel.Information:
                    Logger.LogInformation(format, args);
                    break;
                case LogLevel.Verbose:
                    Logger.LogTrace(format, args);
                    break;
                case LogLevel.Debug:
                    Logger.LogDebug(format, args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public Verbosity Verbosity { get; set; }
    }
}
