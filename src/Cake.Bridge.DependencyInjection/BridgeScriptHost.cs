using Cake.Core.Scripting;

namespace Cake.Bridge.DependencyInjection;

internal class BridgeScriptHost : ScriptHost
{
    private IExecutionStrategy Strategy { get; }
    private ICakeReportPrinter Reporter { get; }

    public override async Task<CakeReport> RunTargetAsync(string target)
    {
        Settings.SetTarget(target);
        var report = await Engine.RunTargetAsync(Context, Strategy, Settings);
        Reporter.Write(report);
        return report;
    }

    public override async Task<CakeReport> RunTargetsAsync(IEnumerable<string> targets)
    {
        Settings.SetTargets(targets);
        var report = await Engine.RunTargetAsync(Context, Strategy, Settings);
        Reporter.Write(report);
        return report;
    }

    public BridgeScriptHost(ICakeEngine engine, ICakeContext context, IExecutionStrategy strategy, ICakeReportPrinter reporter, ICakeArguments arguments)
        : base(engine, context)
    {
        Strategy = strategy;
        Reporter = reporter;
    }
}