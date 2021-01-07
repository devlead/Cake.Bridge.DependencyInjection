using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.IO.NuGet;
using Cake.Core.Reflection;
using Cake.Core.Tooling;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Bridge.DependencyInjection
{
    public static class CakeCoreExtensions
    {
        public static IServiceCollection AddCakeCore(this IServiceCollection serviceCollection)
        {
            // Execution
            serviceCollection.AddSingleton<ICakeContext, CakeContext>();

            var cakeDataService = typeof(ICakeDataService)
                .Assembly
                .GetType("Cake.Core.CakeDataService")
                ?.GetConstructor(Type.EmptyTypes)
                ?.Invoke(Array.Empty<object>()) as ICakeDataService ?? throw new CakeException("Failed to resolve Cake.Core.CakeDataService");

            serviceCollection.AddSingleton<ICakeDataResolver>(cakeDataService);
            serviceCollection.AddSingleton<ICakeDataService>(cakeDataService);

            // Utilities
            serviceCollection.AddSingleton<ICakeConfiguration>(new CakeConfiguration(new Dictionary<string, string>()));
            serviceCollection.AddSingleton<ICakeArguments>(new CakeArguments(Array.Empty<string>().ToLookup(key => key)));

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
            serviceCollection.AddSingleton<ICakeLog, CakeMicrosoftExtensionsLogging>();

            return serviceCollection;
        }
    }
}
