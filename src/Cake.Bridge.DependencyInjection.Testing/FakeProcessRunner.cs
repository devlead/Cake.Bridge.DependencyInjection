namespace Cake.Bridge.DependencyInjection.Testing;

/// <summary>
/// A fake implementation of <see cref="IProcessRunner"/> for testing
/// </summary>
/// <param name="processRunnerFactory">Factory for creating process instances</param>
public class FakeProcessRunner(ProcessRunnerFactory processRunnerFactory) : IProcessRunner
{
    private readonly ProcessRunnerFactory _factory = processRunnerFactory;
    private readonly List<IProcess> _processes = [];
    
    /// <summary>
    /// Gets the list of processes that have been started
    /// </summary>
    public IReadOnlyList<IProcess> Results => _processes;

    /// <inheritdoc/>
    public IProcess Start(FilePath filePath, ProcessSettings settings)
    {
        var process = _factory(filePath, settings);

        _processes.Add(process);

        return process;
    }
}

/// <summary>
/// Delegate for creating process instances
/// </summary>
/// <param name="filePath">The path to the process executable</param>
/// <param name="settings">The process settings</param>
/// <returns>A new process instance</returns>
public delegate IProcess ProcessRunnerFactory(FilePath filePath, ProcessSettings settings);