namespace Cake.Bridge.DependencyInjection;

public class BridgeArguments : ICakeArguments
{
    private Dictionary<string, ICollection<string>> Arguments { get; set; } = [];

    bool ICakeArguments.HasArgument(string name)
        => Arguments.ContainsKey(name);

    ICollection<string> ICakeArguments.GetArguments(string name)
        => Arguments.TryGetValue(name, out var values)
            ? values
            : Array.Empty<string>();

    public void SetArguments(ILookup<string, string> arguments)
    {
        Arguments = arguments.ToDictionary(
            key => key.Key,
            value => (ICollection<string>) value.ToArray()
        );
    }

    public IDictionary<string, ICollection<string>> GetArguments()
        => Arguments;
}