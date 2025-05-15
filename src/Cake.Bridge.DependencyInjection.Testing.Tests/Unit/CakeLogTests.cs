using Cake.Core.Diagnostics;

namespace Cake.Bridge.DependencyInjection.Testing.Tests.Unit;

public class CakeLogTests
{
    [Theory]
    [InlineData(Verbosity.Quiet)]
    [InlineData(Verbosity.Minimal)]
    [InlineData(Verbosity.Normal)]
    [InlineData(Verbosity.Verbose)]
    [InlineData(Verbosity.Diagnostic)]
    public async Task SetVerbosity(Verbosity verbosity)
    {
        // Given
        var (service, fake) = ServiceProviderFixture.GetRequiredService<ICakeLog, FakeLog>();

        // When
        fake.Verbosity = verbosity;

        // Then
        await Verify(service);
    }

    [Theory]
    [InlineData(LogLevel.Error, "Error message")]
    [InlineData(LogLevel.Warning, "Warning message")]
    [InlineData(LogLevel.Information, "Info message")]
    [InlineData(LogLevel.Verbose, "Verbose message")]
    [InlineData(LogLevel.Debug, "Debug message")]
    public async Task WriteLogMessage(LogLevel logLevel, string message)
    {
        // Given
        var (service, fake) = ServiceProviderFixture.GetRequiredService<ICakeLog, FakeLog>();

        // When
        service.Write(Verbosity.Diagnostic, logLevel, message);

        // Then
        await Verify(new { LogLevel = logLevel, Message = message, Service = service });
    }
} 