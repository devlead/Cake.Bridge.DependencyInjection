using Cake.Core.Configuration;

namespace Cake.Bridge.DependencyInjection.Testing.Tests.Unit;

public class CakeEnvironentTests
{
    [Theory]
    [InlineData("/root")]
    [InlineData("/home")]
    [InlineData("C:\\temp")]
    public async Task ApplicationRoot(string applicationRoot)
    {
        // Given
        var (service, fake) = ServiceProviderFixture.GetRequiredService<ICakeEnvironment, FakeEnvironment>();

        // When
        fake.ApplicationRoot = applicationRoot;

        // Then
        await Verify(service);
    }

    [Theory]
    [InlineData(PlatformFamily.Linux)]
    [InlineData(PlatformFamily.OSX)]
    [InlineData(PlatformFamily.Windows)]
    public async Task ChangeOperatingSystemFamily(PlatformFamily family)
    {
        // Given
        var (service, fake) = ServiceProviderFixture.GetRequiredService<ICakeEnvironment, FakeEnvironment>();

        // When
        fake.ChangeOperatingSystemFamily(family);

        // Then
        await Verify(service);
    }
}
