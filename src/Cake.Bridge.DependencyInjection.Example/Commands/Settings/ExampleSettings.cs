using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Cake.Core;
using Cake.Core.IO;
using Cake.Bridge.DependencyInjection.Helper;
using Cake.Bridge.DependencyInjection.Example.Commands.Validation;
using Spectre.Console.Cli;

namespace Cake.Bridge.DependencyInjection.Example.Commands.Settings
{
    public class ExampleSettings : CommandSettings
    {
        public ICakeContext Context { get; }
        public ILogger Logger { get; }

        [CommandArgument(0, "[SourcePath]")]
        [Description("Specifies source path to perform example command on, if not specified working direcory used.")]
        [TypeConverter(typeof(DirectoryPathConverter))]
        [ValidatePath()]
        public DirectoryPath SourcePath { get; set; }

        public ExampleSettings(ICakeContext context, ILogger<ExampleSettings> logger)
        {
            Context = context;
            Logger = logger;
            SourcePath = context.Environment.WorkingDirectory;
        }
    }
}