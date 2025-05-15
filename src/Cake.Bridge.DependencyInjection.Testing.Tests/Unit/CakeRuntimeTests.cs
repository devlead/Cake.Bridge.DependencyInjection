using System.Runtime.Versioning;

namespace Cake.Bridge.DependencyInjection.Testing.Tests.Unit;

public class CakeRuntimeTests
{
    [Theory]
    [InlineData("1.0.0")]
    [InlineData("3.1.0")]
    [InlineData("8.0.1")]
    public async Task SetCakeVersion(string version)
    {
        // Given
        var (service, fake) = ServiceProviderFixture.GetRequiredService<ICakeRuntime, FakeRuntime>();

        // When
        fake.CakeVersion = new Version(version);

        // Then
        await Verify(service);
    }

    [Fact]
    public async Task SetTargetFramework()
    {
        // Given
        var (service, fake) = ServiceProviderFixture.GetRequiredService<ICakeRuntime, FakeRuntime>();
        var framework = new FrameworkName(".NETCoreApp", new Version("8.0.0"));

        // When
        fake.BuiltFramework = framework;

        // Then
        await Verify(service);
    }
} 