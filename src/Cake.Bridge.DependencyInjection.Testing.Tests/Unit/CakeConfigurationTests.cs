using Cake.Core.Configuration;

namespace Cake.Bridge.DependencyInjection.Testing.Tests.Unit;

public class CakeConfigurationTests
{
    [Theory]
    [InlineData("foo", "bar")]
    [InlineData("fizz", "buzz")]
    public async Task SetAndGetValue(string key, string value)
    {
        // Given
        var (service, fake) = ServiceProviderFixture.GetRequiredService<ICakeConfiguration, FakeConfiguration>();

        // When
        fake.SetValue(key, value);
        var result = new {
            Key = key,
            Value = value,
            Result = service.GetValue(key)
        };

        // Then
        await Verify(result);
    }
}
