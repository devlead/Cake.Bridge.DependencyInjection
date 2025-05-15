
using System.Runtime.CompilerServices;
using VerifyTests.DiffPlex;

namespace Cake.Bridge.DependencyInjection.Testing.Tests;

public static class VerifyConfig
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifyDiffPlex.Initialize(OutputType.Compact);
        VerifierSettings.InitializePlugins();
        VerifierSettings.DontIgnoreEmptyCollections();
        VerifierSettings.IgnoreStackTrace();
        VerifierSettings.AddExtraSettings(settings =>
        {
            settings.DefaultValueHandling = Argon.DefaultValueHandling.Include;
        });
    }
}
