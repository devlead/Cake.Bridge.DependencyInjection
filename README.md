# Cake Bridge DependencyInjection

Provides helpers for providing Cake context using Microsoft DependencyInjection, letting you use Cake Core/Common/Addins abstractions and aliases.

## Usage

### Obtain

The assembly is published at [nuget.org/packages/Cake.Bridge.DependencyInjection](https://www.nuget.org/packages/Cake.Bridge.DependencyInjection).

#### .NET CLI

```bash
dotnet add package Cake.Bridge.DependencyInjection
```

#### PackageReference
```xml
<PackageReference Include="Cake.Bridge.DependencyInjection" Version="0.4.0" />
```

### Register

```csharp
using Cake.Bridge.DependencyInjection;
...
serviceCollection
    .AddCakeCore();
```

### Use

Once registered you can now via dependency injection access majority [Cake.Core](https://cakebuild.net/api/Cake.Core/#InterfaceTypes) interfaces with ease, i.e:

| Type         | Description |
|--------------|-------------|
| [ICakeContext](https://cakebuild.net/api/Cake.Core/ICakeContext/) | Gives access to Cake built-in and addin aliases, and most Cake abstractions. |
| [IScriptHost](https://cakebuild.net/api/Cake.Core.Scripting/IScriptHost/) | Gives access to script runner. |
| [ICakeLog](https://cakebuild.net/api/Cake.Core.Diagnostics/ICakeLog/) | Cake logging implementation. |
| [IFileSystem](https://cakebuild.net/api/Cake.Core.IO/IFileSystem/) | Cake file system abstraction. |

### Example

```csharp
var serviceCollection = new ServiceCollection()
    .AddCakeCore();

var serviceProvider = serviceCollection.BuildServiceProvider();

var scriptHost = serviceProvider.GetRequiredService<IScriptHost>();

scriptHost.Task("Hello")
    .Does(ctx => ctx.Information("Hello"));

scriptHost.Task("World")
    .IsDependentOn("Hello")
    .Does(ctx => ctx.Information("World"));

await scriptHost.RunTargetAsync("World");
```

will output

```powershell
========================================
Hello
========================================
Hello

========================================
World
========================================
World

Task                          Duration
--------------------------------------------------
Hello                         00:00:00.0226275
World                         00:00:00.0002682
--------------------------------------------------
Total:                        00:00:00.0228957
```

A full example console application using [Spectre.Console](https://www.nuget.org/packages/Spectre.Console) demonstrating usage of both [ICakeContext](https://cakebuild.net/api/Cake.Core/ICakeContext/) and [IScriptHost](https://cakebuild.net/api/Cake.Core.Scripting/IScriptHost/) can be found in this repository at [src/Cake.Bridge.DependencyInjection.Example](src/Cake.Bridge.DependencyInjection.Example).
