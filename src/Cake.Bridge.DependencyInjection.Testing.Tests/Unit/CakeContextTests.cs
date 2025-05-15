using Cake.Core.Configuration;

namespace Cake.Bridge.DependencyInjection.Testing.Tests.Unit;

public class CakeContextTests
{
    [Fact]
    public async Task Service()
    {
        // Given
        var service = ServiceProviderFixture.GetRequiredService<TestService>(
            services => services.AddSingleton<TestService>()
            );

        // When
        var result = service.Run();

        // Then
        await Verify(result);
    }
}
