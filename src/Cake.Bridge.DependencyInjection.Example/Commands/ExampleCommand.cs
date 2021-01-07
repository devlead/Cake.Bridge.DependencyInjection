using Microsoft.Extensions.Logging;
using Cake.Common.IO;
using Cake.Core.IO;
using Cake.Bridge.DependencyInjection.Example.Commands.Settings;
using Spectre.Console.Cli;

namespace Cake.Bridge.DependencyInjection.Example.Commands
{
    public class ExampleCommand : Command<ExampleSettings>
    {
        public override int Execute(CommandContext context, ExampleSettings settings)
        {
            var absoluteSourcePath = settings.Context.MakeAbsolute(settings.SourcePath);
            var direcory = settings.Context.FileSystem.GetDirectory(absoluteSourcePath);

            settings.Logger.LogInformation(
                " Directory of {SourcePath}",
                absoluteSourcePath
                );

            foreach (var directoryPath in direcory.GetDirectories("*.*", SearchScope.Current))
            {
                settings.Logger.LogInformation(
                    "<DIR>\t{DirectoryName}",
                    directoryPath.Path.GetDirectoryName()
                    );
            }
            foreach (var file in direcory.GetFiles("*.*", SearchScope.Current))
            {
                settings.Logger.LogInformation(
                    "{Length}\t{Filename}",
                    file.Length,
                    file.Path.GetFilename()
                    );
            }
            return 0;
        }
    }
}