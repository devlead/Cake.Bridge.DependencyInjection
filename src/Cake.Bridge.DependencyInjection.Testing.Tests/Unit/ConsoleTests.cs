namespace Cake.Bridge.DependencyInjection.Testing.Tests.Unit;

public class ConsoleTests
{
    [Theory]
    [InlineData("Hello World")]
    [InlineData("Test Message")]
    public async Task WriteLineTest(string message)
    {
        // Given
        var (service, fake) = ServiceProviderFixture.GetRequiredService<IConsole, FakeConsole>();

        // When
        service.WriteLine(message);

        // Then
        await Verify(fake);
    }

    [Theory]
    [InlineData("Error Message", ConsoleColor.Red)]
    [InlineData("Warning Message", ConsoleColor.Yellow)]
    [InlineData("Info Message", ConsoleColor.Green)]
    public async Task WriteLineWithForegroundColor(string message, ConsoleColor color)
    {
        // Given
        var (service, fake) = ServiceProviderFixture.GetRequiredService<IConsole, FakeConsole>();

        // When
        service.ForegroundColor = color;
        service.WriteLine(message);

        // Then
        await Verify(fake);
    }
} 