using System.ComponentModel;
using Cake.Bridge.DependencyInjection.Example.Helper;
using Cake.Core.Diagnostics;
using Spectre.Console.Cli;

namespace Cake.Bridge.DependencyInjection.Example.Commands.Settings
{
    public class ScriptHostSettings : CommandSettings
    {
        [CommandOption("-e|--exclusive")]
        [Description("Execute a single task without any dependencies.")]
        public bool Exclusive { get; set; }

        [CommandOption("-v|--verbosity")]
        [Description("Specifies the amount of information to be displayed.")]
        [TypeConverter(typeof(VerbosityConverter))]
        [DefaultValue(Verbosity.Normal)]
        public Verbosity Verbosity { get; set; }

        [CommandOption("-t|--target")]
        [Description("Target to execute.")]
        [DefaultValue("Default")]
        public string Target { get; set; }
    }
}
