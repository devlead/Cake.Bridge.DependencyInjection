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
        long 
            fileCount = 0,
            fileBytes = 0,
            directoryCount = 0;
        foreach (var directoryPath in directory.GetDirectories("*.*", SearchScope.Current))
        {
            directoryCount++;
            settings.Context.Information(
                "{0}\t<DIR>\t\t{1}",
                directoryPath.CreationTimeUtc?.ToString("yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                directoryPath.Path.GetDirectoryName()
                );
        }

        foreach (var file in directory.GetFiles("*.*", SearchScope.Current))
        {
            fileCount++;
            fileBytes += file.Length;
            settings.Context.Information(
                "{0}\t\t{1}\t{2}",
                file.CreationTimeUtc?.ToString("yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                file.Length,
                file.Path.GetFilename()
                );
        }

        settings.Context.Information(
            "\t\t{0:N0} File(s)\t{1:N0} bytes",
            fileCount,
            fileBytes
            );
        settings.Context.Information(
            "\t\t{0:N0} Dir(s)",
            directoryCount
            );

        return 0;
    }
}