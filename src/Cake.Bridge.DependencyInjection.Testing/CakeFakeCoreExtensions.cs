using Cake.Core.Configuration;
using Cake.Core.IO.NuGet;
using Cake.Core.Reflection;
using Cake.Core.Scripting;
using Cake.Core.Tooling;
using System.Text;


namespace Cake.Bridge.DependencyInjection.Testing;

/// <summary>
/// Contains extension methods for configuring fake Cake core services
/// </summary>
public static class CakeFakeCoreExtensions
{
    /// <summary>
    /// Delegate for configuring a service instance
    /// </summary>
    /// <typeparam name="TService">The type of service to configure</typeparam>
    /// <param name="service">The service instance to configure</param>
    public delegate void Configure<TService>(TService service);

    /// <summary>
    /// Adds a singleton service configuration to the service collection
    /// </summary>
    /// <typeparam name="TService">The type of service to configure</typeparam>
    /// <param name="services">The service collection to add to</param>
    /// <param name="configure">The configuration action to register</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddConfigureSingleton<TService>(
        this IServiceCollection services,
        Configure<TService> configure
        )
        => services.AddSingleton(configure);
    

    /// <summary>
    /// Adds a singleton service that can be configured during registration and through dependency injection
    /// </summary>
    /// <typeparam name="TService">The type of service to register</typeparam>
    /// <param name="serviceCollection">The service collection to add to</param>
    /// <param name="serviceInstance">The service instance to register</param>
    /// <param name="configure">Optional configuration action</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddConfiguredSingleton<TService>(
        this IServiceCollection serviceCollection,
        TService serviceInstance,
        Configure<TService>? configure = null
    ) where TService : class
    {
        configure?.Invoke(serviceInstance);
        return serviceCollection.AddSingleton(provider =>
        {
            
            var configurations = provider.GetService<Configure<TService>[]>() ?? [];
            foreach (var configure in configurations)
            {
                configure(serviceInstance);
            }
            return serviceInstance;
        });
    }

    /// <summary>
    /// Adds fake implementations of Cake core services to the service collection
    /// </summary>
    /// <param name="serviceCollection">The service collection to add the fake services to</param>
    /// <param name="configureConfiguration">Optional configuration for FakeConfiguration</param>
    /// <param name="configureEnvironment">Optional configuration for FakeEnvironment</param>
    /// <param name="configureFileSystem">Optional configuration for FakeFileSystem</param>
    /// <param name="configureLog">Optional configuration for FakeLog</param>
    /// <param name="configureConsole">Optional configuration for FakeConsole</param>
    /// <param name="configureRuntime">Optional configuration for FakeRuntime</param>
    /// <param name="configurePlatform">Optional configuration for FakePlatform</param>
    /// <param name="configureArguments">Optional configuration for BridgeArguments</param>
    /// <param name="processRunnerFactory">Optional factory for creating process runners</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCakeCoreFakes(
        this IServiceCollection serviceCollection,
        Configure<FakeConfiguration>? configureConfiguration = null,
        Configure<FakeEnvironment>? configureEnvironment = null,
        Configure<FakeFileSystem>? configureFileSystem = null,
        Configure<FakeLog>? configureLog = null,
        Configure<FakeConsole>? configureConsole = null,
        Configure<FakeRuntime>? configureRuntime = null,
        Configure<FakePlatform>? configurePlatform = null,
        Configure<BridgeArguments>? configureArguments = null,
        ProcessRunnerFactory? processRunnerFactory = null
    )
    {
        // Fake Configuration
        var configuration = new FakeConfiguration();
        serviceCollection.AddConfiguredSingleton(configuration, configureConfiguration);

        // Fake Environment
        var environment = FakeEnvironment.CreateUnixEnvironment();
        serviceCollection.AddConfiguredSingleton(environment, configureEnvironment);

        // Fake FileSystem
        var fileSystem = new FakeFileSystem(environment);
        serviceCollection.AddConfiguredSingleton(fileSystem, configureFileSystem);

        // Fake Log
        var log = new FakeLog();
        serviceCollection.AddConfiguredSingleton(log, configureLog);

        // Fake Console
        var console = new FakeConsole();
        serviceCollection.AddConfiguredSingleton(console, configureConsole);

        // Fake Runtime
        var runtime = new FakeRuntime();
        serviceCollection.AddConfiguredSingleton(runtime, configureRuntime);

        // Fake Arguments
        var arguments = new BridgeArguments();
        serviceCollection.AddConfiguredSingleton(arguments, configureArguments);

        // Fake Platform
        var platform = new FakePlatform(PlatformFamily.Linux);
        serviceCollection.AddConfiguredSingleton(platform, configurePlatform);

        // Fake Process Runner
        processRunnerFactory ??= (filePath, settings) =>
        {
            var fakeProcess = new FakeProcess();
            if (!fileSystem.Exist(filePath))
            {
                fakeProcess.SetExitCode(1);
            }
            return fakeProcess;
        };

        serviceCollection.AddSingleton(provider =>
        {
            return new FakeProcessRunner(
                provider.GetService<ProcessRunnerFactory>() ?? processRunnerFactory
            );
        });

        // Converters
        serviceCollection.AddSingleton<DirectoryPathConverter>();
        serviceCollection.AddSingleton<FilePathConverter>();
        serviceCollection.AddSingleton<VerbosityConverter>();

        // Execution
        serviceCollection.AddSingleton<ICakeContext, CakeContext>();

        var cakeDataService = new CakeDataService();

        serviceCollection.AddSingleton<ICakeDataResolver>(cakeDataService);
        serviceCollection.AddSingleton<ICakeDataService>(cakeDataService);

        // Utilities
        serviceCollection.AddSingleton<ICakeArguments>(arguments);
        serviceCollection.AddSingleton<ICakeConfiguration>(configuration);

        // Environment
        serviceCollection.AddSingleton<ICakeEnvironment>(environment);
        serviceCollection.AddSingleton<ICakeRuntime>(runtime);
        serviceCollection.AddSingleton<ICakePlatform>(platform);

        // IO
        serviceCollection.AddSingleton<IFileSystem>(fileSystem);
        serviceCollection.AddSingleton<IGlobber, Globber>();
        serviceCollection.AddSingleton<IProcessRunner>(provider => provider.GetRequiredService<FakeProcessRunner>());
        serviceCollection.AddSingleton<INuGetToolResolver, NuGetToolResolver>();
        serviceCollection.AddSingleton<IRegistry, WindowsRegistry>();

        // Reflection
        serviceCollection.AddSingleton<IAssemblyLoader, AssemblyLoader>();
        serviceCollection.AddSingleton<IAssemblyVerifier, AssemblyVerifier>();

        // Tooling
        serviceCollection.AddSingleton<IToolRepository, ToolRepository>();
        serviceCollection.AddSingleton<IToolResolutionStrategy, ToolResolutionStrategy>();
        serviceCollection.AddSingleton<IToolLocator, ToolLocator>();

        // Logging
        serviceCollection.AddSingleton<IConsole>(console);
        serviceCollection.AddSingleton<ICakeLog>(log);

        // Scripting
        serviceCollection.AddSingleton<ICakeReportPrinter, CakeReportPrinter>();
        serviceCollection.AddSingleton<IExecutionStrategy, DefaultExecutionStrategy>();
        serviceCollection.AddSingleton<ICakeEngine, CakeEngine>();
        serviceCollection.AddSingleton<IScriptHost, BridgeScriptHost>();

        return serviceCollection;
    }
}