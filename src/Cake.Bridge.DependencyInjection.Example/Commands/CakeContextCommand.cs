using Cake.Common.IO;
using Cake.Core.IO;
using Cake.Bridge.DependencyInjection.Example.Commands.Settings;
using Cake.Common.Diagnostics;
using Spectre.Console.Cli;

namespace Cake.Bridge.DependencyInjection.Example.Commands;

// ReSharper disable once ClassNeverInstantiated.Global
public class CakeContextCommand : Command<CakeContextSettings>
{
    public override int Execute(CommandContext context, CakeContextSettings settings, CancellationToken token)
    {
        var absoluteSourcePath = settings.Context.MakeAbsolute(settings.SourcePath);
        var directory = settings.Context.FileSystem.GetDirectory(absoluteSourcePath);

        settings.Context.Information(
            " Directory of {0}",
            absoluteSourcePath
            );

        foreach (var directoryPath in directory.GetDirectories("*.*", SearchScope.Current))
        {
            settings.Context.Information(
                "\t<DIR>\t{0}",
                directoryPath.Path.GetDirectoryName()
                );
        }

        foreach (var file in directory.GetFiles("*.*", SearchScope.Current))
        {
            settings.Context.Information(
                "\t{0}\t{1}",
                file.Length,
                file.Path.GetFilename()
                );
        }

        return 0;
    }
}