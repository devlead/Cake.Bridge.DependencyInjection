using Cake.Core;

namespace Cake.Bridge.DependencyInjection.Testing.Tests.Unit;

public class CakePlatformTests
{
    [Theory]
    [InlineData(PlatformFamily.Linux)]
    [InlineData(PlatformFamily.OSX)]
    [InlineData(PlatformFamily.Windows)]
    public async Task SetPlatformFamily(PlatformFamily family)
    {
        // Given
        var (service, fake) = ServiceProviderFixture.GetRequiredService<ICakePlatform, FakePlatform>();

        // When
        fake.Family = family;

        // Then
        await Verify(service);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task SetIs64Bit(bool is64Bit)
    {
        // Given
        var (service, fake) = ServiceProviderFixture.GetRequiredService<ICakePlatform, FakePlatform>();

        // When
        fake.Is64Bit = is64Bit;

        // Then
        await Verify(service);
    }
} 