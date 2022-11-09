using System.ComponentModel;
using Cake.Core;
using Cake.Core.IO;
using Cake.Bridge.DependencyInjection.Example.Commands.Validation;
using Cake.Core.Diagnostics;
using Spectre.Console.Cli;

namespace Cake.Bridge.DependencyInjection.Example.Commands.Settings;

// ReSharper disable once ClassNeverInstantiated.Global
public class CakeContextSettings : CommandSettings
{
    public ICakeContext Context { get; }

    [CommandArgument(0, "[SourcePath]")]
    [Description("Specifies source path to perform example command on, if not specified working direcory used.")]
    [TypeConverter(typeof(DirectoryPathConverter))]
    [ValidatePath]
    public DirectoryPath SourcePath { get; set; }

    [CommandOption("-v|--verbosity")]
    [Description("Specifies the amount of information to be displayed.")]
    [TypeConverter(typeof(VerbosityConverter))]
    [DefaultValue(Verbosity.Normal)]
    public Verbosity Verbosity
    {
        get => Context.Log.Verbosity;
        set => Context.Log.Verbosity = value;
    }

    public CakeContextSettings(ICakeContext context)
    {
        Context = context;
        SourcePath = context.Environment.WorkingDirectory;
    }
}