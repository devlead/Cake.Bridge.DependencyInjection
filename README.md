# Cake Bridge DependencyInjection

Provides helpers for providing Cake context using Microsoft DependencyInjection, letting you use Cake Core/Common/Addins abstractions and aliases.

## Usage

### Obtain

The assembly is published at [nuget.org/packages/Cake.Bridge.DependencyInjection](https://www.nuget.org/packages/Cake.Bridge.DependencyInjection).

#### .NET CLI

```bash
dotnet add package Cake.Bridge.DependencyInjection --version 0.1.0-alpha0001
```

#### PackageReference
```xml
<PackageReference Include="Cake.Bridge.DependencyInjection" Version="0.1.0-alpha0001" />
```

### Register

```csharp
using using Cake.Bridge.DependencyInjection;
...
serviceCollection
    .AddCakeCore();
```

### Use

```csharp
using Cake.Core;

public MyClass
{
    public ICakeContext CakeContext { get; }

    public MyClass(ICakeContext cakeContext)
    {
        CakeContext = cakeContext;
    }
}
```

### Example

A example console application can be found at [src/Cake.Bridge.DependencyInjection.Example](src/Cake.Bridge.DependencyInjection.Example).
