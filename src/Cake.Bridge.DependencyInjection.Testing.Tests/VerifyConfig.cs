
using System.Runtime.CompilerServices;
using DiffEngine;

namespace Cake.Bridge.DependencyInjection.Testing.Tests;

public static class VerifyConfig
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.UseTextDiffFormat(TextDiffFormat.Compact);
        VerifierSettings.InitializePlugins();
        VerifierSettings.DontIgnoreEmptyCollections();
        VerifierSettings.IgnoreStackTrace();
        VerifierSettings.AddExtraSettings(settings =>
        {
            settings.DefaultValueHandling = Argon.DefaultValueHandling.Include;
        });
    }
}
