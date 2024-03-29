﻿using Cake.Core.Configuration;
using Cake.Core.IO.NuGet;
using Cake.Core.Reflection;
using Cake.Core.Scripting;
using Cake.Core.Tooling;

namespace Cake.Bridge.DependencyInjection;

public static class CakeCoreExtensions
{
    public static IServiceCollection AddCakeCore(
        this IServiceCollection serviceCollection,
        IDictionary<string, string> cakeConfiguration = null
        )
    {
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
        var arguments = new BridgeArguments();
        serviceCollection.AddSingleton(arguments);
        serviceCollection.AddSingleton<ICakeArguments>(arguments);
        serviceCollection.AddSingleton<ICakeConfiguration>(new CakeConfiguration(cakeConfiguration ?? new Dictionary<string, string>()));

        // Environment
        serviceCollection.AddSingleton<ICakeEnvironment, CakeEnvironment>();
        serviceCollection.AddSingleton<ICakeRuntime, CakeRuntime>();
        serviceCollection.AddSingleton<ICakePlatform, CakePlatform>();

        // IO
        serviceCollection.AddSingleton<IFileSystem, FileSystem>();
        serviceCollection.AddSingleton<IGlobber, Globber>();
        serviceCollection.AddSingleton<IProcessRunner, ProcessRunner>();
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
        serviceCollection.AddSingleton<IConsole, CakeConsole>();
        serviceCollection.AddSingleton<ICakeLog, CakeBuildLog>();

        // Scripting
        serviceCollection.AddSingleton<ICakeReportPrinter, CakeReportPrinter>();
        serviceCollection.AddSingleton<IExecutionStrategy, DefaultExecutionStrategy>();
        serviceCollection.AddSingleton<ICakeEngine, CakeEngine>();
        serviceCollection.AddSingleton<IScriptHost, BridgeScriptHost>();

        return serviceCollection;
    }
}