using Microsoft.Extensions.DependencyInjection;
using Cake.Bridge.DependencyInjection;
using Cake.Bridge.DependencyInjection.Example.Commands;
using Spectre.Console.Cli;
using Spectre.Cli.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection()
    .AddCakeCore();

serviceCollection.AddSingleton<Cake.Bridge.DependencyInjection.Example.Commands.Settings.CakeContextSettings>();

using var registrar = new DependencyInjectionRegistrar(serviceCollection);
var app = new CommandApp(registrar);

app.Configure(
    config =>
    {
        config.SetApplicationName("cbe");
        config.ValidateExamples();

        config.AddCommand<CakeContextCommand>("context")
                .WithDescription("Example testing just Cake context.")
                .WithExample(new[] { "context" });

        config.AddCommand<ScriptHostCommand>("host")
            .WithDescription("Example testing just Cake script host.")
            .WithExample(new[] { "host" });
    });

return await app.RunAsync(args);