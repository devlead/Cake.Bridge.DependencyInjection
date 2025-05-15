
namespace Cake.Bridge.DependencyInjection.Testing.Tests.Services;
public class TestService(ICakeContext context)
{
    public ICakeContext Run() => context;
}
