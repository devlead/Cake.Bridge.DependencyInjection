using System.Threading.Tasks;
using Cake.Bridge.DependencyInjection.Example.Commands.Settings;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Core.Scripting;
using Spectre.Console.Cli;

namespace Cake.Bridge.DependencyInjection.Example.Commands
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ScriptHostCommand : AsyncCommand<ScriptHostSettings>
    {
        private IScriptHost ScriptHost { get; }
        private BridgeArguments BridgeArguments { get; }

        public override async Task<int> ExecuteAsync(CommandContext context, ScriptHostSettings settings)
        {
            if (settings.Exclusive)
            {
                ScriptHost.Settings.UseExclusiveTarget();
            }

            ScriptHost.Context.Log.Verbosity = settings.Verbosity;

            BridgeArguments.SetArguments(context.Remaining.Parsed);

            var hello = ScriptHost.Task(nameof(Hello))
                .Does(Hello);

            var world = ScriptHost.Task(nameof(World))
                .IsDependentOn(hello)
                .Does(World);

            ScriptHost.Task("Default")
                .IsDependentOn(world);

            await ScriptHost.RunTargetAsync(settings.Target);

            return 0;
        }

        private static void Hello(ICakeContext context)
        {
            context.Information("{0}", context.GetCallerInfo().MemberName);
        }

        private static void World(ICakeContext context)
        {
            context.Information("{0}", context.GetCallerInfo().MemberName);
        }

        public ScriptHostCommand(IScriptHost scriptHost, BridgeArguments bridgeArguments)
        {
            ScriptHost = scriptHost;
            BridgeArguments = bridgeArguments;
        }
    }
}
