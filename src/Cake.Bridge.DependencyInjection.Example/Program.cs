using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Cake.Bridge.DependencyInjection;
using Cake.Bridge.DependencyInjection.Example.Commands;
using Spectre.Console.Cli;
using Spectre.Cli.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection()
    .AddLogging(configure =>
        configure
            .AddSimpleConsole(opts => {
                opts.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
            }))
    .AddCakeCore();

using var registrar = new DependencyInjectionRegistrar(serviceCollection);
var app = new CommandApp(registrar);

app.Configure(
    config =>
    {
        config.SetApplicationName("cbe");
        config.ValidateExamples();
        config.AddCommand<ExampleCommand>("example")
                .WithDescription("Example command")
                .WithExample(new[] { "example" });
    });

return await app.RunAsync(args);