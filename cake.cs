#:sdk Cake.Sdk@6.0.0
#:property IncludeAdditionalFiles=./build/*.cs


/*****************************
 * Setup
 *****************************/
Setup(
    static context => {
        // Install .NET Core Global tools.
        InstallTool("dotnet:https://api.nuget.org/v3/index.json?package=DPI&version=2025.11.5.295");
        InstallTool("dotnet:https://api.nuget.org/v3/index.json?package=GitVersion.Tool&version=6.5.0");
        var assertedVersions = context.GitVersion(new GitVersionSettings
            {
                OutputType = GitVersionOutput.Json
            });

        var branchName = assertedVersions.BranchName;
        var isMainBranch = StringComparer.OrdinalIgnoreCase.Equals("main", branchName);
        
        var buildDate = DateTime.UtcNow;
        var runNumber = GitHubActions.IsRunningOnGitHubActions
                            ? GitHubActions.Environment.Workflow.RunNumber
                            : 0;
      
        var suffix = runNumber == 0 
                       ? $"-{(short)((buildDate - buildDate.Date).TotalSeconds/3)}"
                       : string.Empty;

        var version = FormattableString
                          .Invariant($"{buildDate:yyyy.M.d}.{runNumber}{suffix}");

        context.Information("Building version {0} (Branch: {1}, IsMain: {2})",
            version,
            branchName,
            isMainBranch);

        var artifactsPath = context
                            .MakeAbsolute(context.Directory("./artifacts"));

        return new BuildData(
            version,
            isMainBranch,
            !context.IsRunningOnWindows(),
            "./src",
            context.MakeAbsolute(FilePath.FromString("./src/Cake.Bridge.DependencyInjection.Example/Cake.Bridge.DependencyInjection.Example.csproj")),
            new DotNetMSBuildSettings()
                .SetConfiguration("Release")
                .SetVersion(version)
                .WithProperty("Copyright", $"Mattias Karlsson © {DateTime.UtcNow.Year}")
                .WithProperty("Authors", "devlead")
                .WithProperty("Company", "devlead")
                .WithProperty("PackageLicenseExpression", "MIT")
                .WithProperty("PackageTags", "Cake;Build;DependencyInjection")
                .WithProperty("PackageDescription", "Provides helpers for providing Cake context using Microsoft DependencyInjection, letting you use Cake Core/Common/Addins abstractions and aliases.")
                .WithProperty("PackageIconUrl", "https://cdn.rawgit.com/cake-contrib/graphics/a5cf0f881c390650144b2243ae551d5b9f836196/png/cake-contrib-medium.png")
                .WithProperty("PackageIcon", "cake-contrib-medium.png")
                .WithProperty("PackageProjectUrl", "https://github.com/devlead/Cake.Bridge.DependencyInjection")
                .WithProperty("RepositoryUrl", "https://github.com/devlead/Cake.Bridge.DependencyInjection.git")
                .WithProperty("RepositoryType", "git")
                .WithProperty("ContinuousIntegrationBuild", GitHubActions.IsRunningOnGitHubActions ? "true" : "false")
                .WithProperty("EmbedUntrackedSources", "true"),
            artifactsPath,
            artifactsPath.Combine(version)
            );
    }
);

/*****************************
 * Tasks
 *****************************/
Task("Clean")
    .Does<BuildData>(
        static (context, data) => context.CleanDirectories(data.DirectoryPathsToClean)
    )
.Then("Restore")
    .Does<BuildData>(
        static (context, data) => context.DotNetRestore(
            data.ProjectRoot.FullPath,
            new DotNetRestoreSettings {
                MSBuildSettings = data.MSBuildSettings
            }
        )
    )
.Then("DPI")
    .Does<BuildData>(
        static (context, data) => Command(
                ["dpi", "dpi.exe"],
                new ProcessArgumentBuilder()
                    .Append("nuget")
                    .Append("--silent")
                    .AppendSwitchQuoted("--output", "table")
                    .Append(
                        (
                            !string.IsNullOrWhiteSpace(context.EnvironmentVariable("NuGetReportSettings_SharedKey"))
                            &&
                            !string.IsNullOrWhiteSpace(context.EnvironmentVariable("NuGetReportSettings_WorkspaceId"))
                        )
                            ? "report"
                            : "analyze"
                        )
                    .AppendSwitchQuoted("--buildversion", data.Version)
                
            )
    )
.Then("Build")
    .Does<BuildData>(
        static (context, data) => context.DotNetBuild(
            data.ProjectRoot.FullPath,
            new DotNetBuildSettings {
                NoRestore = true,
                MSBuildSettings = data.MSBuildSettings
            }
        )
    )
.Then("Test")
    .DoesForEach<BuildData, FilePath>(
        GetFiles("./src/**/*.Tests.csproj"),
        static (data, item, context) => context.DotNetTest(
            item.FullPath,
            new DotNetTestSettings {
                NoRestore = true,
                NoBuild = true,
                MSBuildSettings = data.MSBuildSettings,
                PathType = DotNetTestPathType.Project
            }
        )
    )
.Then("Integration-Test")
    .Default()
    .DoesForEach<BuildData, (string Framework, string Command)>(
        static (data, context)
            =>  (
                    from framework in context
                                        .XmlPeek(
                                            data.IntegrationTestProject.FullPath,
                                            "/Project/PropertyGroup/TargetFrameworks"
                                        )
                                        .Split(';', StringSplitOptions.TrimEntries)
                    from command in new []{ "context", "host" }
                    select (
                        framework,
                        command
                    )
                ),
        static (data, item, context)
            => {
            context.Information("Testing {0}...", item);
            context.DotNetRun(
                data.IntegrationTestProject.FullPath,
                new ProcessArgumentBuilder()
                    .Append(item.Command),
                new DotNetRunSettings {
                    Framework = item.Framework,
                    NoWorkingDirectory = true,
                    NoRestore = true
                }
            );
            context.Information("Tested {0}.", item);
        }
    )
.Then("Pack")
    .Does<BuildData>(
        static (context, data) => context.DotNetPack(
            data.ProjectRoot.FullPath,
            new DotNetPackSettings {
                NoBuild = true,
                NoRestore = true,
                OutputDirectory = data.NuGetOutputPath,
                MSBuildSettings = data.MSBuildSettings
            }
        )
    )
.Then("Upload-Artifacts")
    .WithCriteria<BuildData>( (context, data) => data.ShouldPushGitHubPackages())
    .Does<BuildData>(
        static (context, data) => GitHubActions
            .Commands
            .UploadArtifact(data.ArtifactsPath, "artifacts")
    )
.Then("Push-GitHub-Packages")
    .WithCriteria<BuildData>( (context, data) => data.ShouldPushGitHubPackages())
    .DoesForEach<BuildData, FilePath>(
        static (data, context)
            => context.GetFiles(data.NuGetOutputPath.FullPath + "/*.nupkg"),
        static (data, item, context)
            => context.DotNetNuGetPush(
                item.FullPath,
            new DotNetNuGetPushSettings
            {
                Source = data.GitHubNuGetSource,
                ApiKey = data.GitHubNuGetApiKey
            }
        )
    )
.Then("Push-NuGet-Packages")
    .WithCriteria<BuildData>( (context, data) => data.ShouldPushNuGetPackages())
    .DoesForEach<BuildData, FilePath>(
        static (data, context)
            => context.GetFiles(data.NuGetOutputPath.FullPath + "/*.nupkg"),
        static (data, item, context)
            => context.DotNetNuGetPush(
                item.FullPath,
                new DotNetNuGetPushSettings
                {
                    Source = data.NuGetSource,
                    ApiKey = data.NuGetApiKey
                }
        )
    )
.Then("Create-GitHub-Release")
    .WithCriteria<BuildData>( (context, data) => data.ShouldPushNuGetPackages())
    .Does<BuildData>(
        static (context, data) => context
            .Command(
                new CommandSettings {
                    ToolName = "GitHub CLI",
                    ToolExecutableNames = new []{ "gh.exe", "gh" },
                    EnvironmentVariables = { { "GH_TOKEN", data.GitHubNuGetApiKey } }
                },
                new ProcessArgumentBuilder()
                    .Append("release")
                    .Append("create")
                    .Append(data.Version)
                    .AppendSwitchQuoted("--title", data.Version)
                    .Append("--generate-notes")
                    .Append(string.Join(
                        ' ',
                        context
                            .GetFiles(data.NuGetOutputPath.FullPath + "/*.nupkg")
                            .Select(path => path.FullPath.Quote())
                        ))

            )
    )
.Then("GitHub-Actions")
.Run();
