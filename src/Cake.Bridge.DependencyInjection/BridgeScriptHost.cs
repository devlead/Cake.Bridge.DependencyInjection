using Cake.Core.Scripting;

namespace Cake.Bridge.DependencyInjection;

internal class BridgeScriptHost(ICakeEngine engine, ICakeContext context, IExecutionStrategy strategy, ICakeReportPrinter reporter)
    : ScriptHost(engine, context)
{

    public override async Task<CakeReport> RunTargetAsync(string target)
    {
        Settings.SetTarget(target);
        var report = await Engine.RunTargetAsync(Context, strategy, Settings);
        reporter.Write(report);
        return report;
    }

    public override async Task<CakeReport> RunTargetsAsync(IEnumerable<string> targets)
    {
        Settings.SetTargets(targets);
        var report = await Engine.RunTargetAsync(Context, strategy, Settings);
        reporter.Write(report);
        return report;
    }
}