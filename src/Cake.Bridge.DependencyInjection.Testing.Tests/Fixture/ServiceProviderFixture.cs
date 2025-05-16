public static partial class ServiceProviderFixture
{
    static partial void InitServiceProvider(IServiceCollection services)
    {
        services.AddCakeCoreFakes(
           configureEnvironment: env =>
           {
               env.SetEnvironmentVariable("KEY", "VALUE");
           }
        );
    }
}
