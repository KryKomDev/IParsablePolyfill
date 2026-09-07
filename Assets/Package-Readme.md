# IParsablePolyfill

A backward-compatible `IParsable<TSelf>` polyfill for .NET down to .NET Standard 2.0, powered by [Implyzer](https://github.com/KryKomDev/Implyzer).

## About

`IParsable<TSelf>` was introduced in .NET 7 (C# 11) alongside `static abstract` interface members, providing a standardized contract for parsing strings into strongly-typed objects. However, projects targeting earlier .NET runtimes (.NET Standard 2.0/2.1, .NET Core, .NET 5, or .NET 6) cannot use this interface or write generic string parsing algorithms.

**IParsablePolyfill** backports `IParsable<TSelf>` to all modern and legacy .NET runtimes down to **.NET Standard 2.0**. Powered by [Implyzer](https://github.com/KryKomDev/Implyzer), it utilizes native `static abstract` members on .NET 7+ with zero overhead, while automatically generating high-performance static dispatch fallback tables on older frameworks.

Additionally, standard BCL types (`int`, `double`, `DateTime`, `Guid`, `Uri`, `IPAddress`, `BigInteger`, etc.) are pre-registered, enabling consistent parsing across all targets.

---

## Features

- **Broad Framework Support**: Seamlessly targets `.NET Standard 2.0`, `.NET Standard 2.1`, `.NET 5.0`, `.NET 6.0`, `.NET 7.0`, `.NET 8.0`, `.NET 9.0`, and `.NET 10.0`.
- **Pre-Registered BCL Types**: All standard parsable types in the Base Class Library are registered and ready to parse out of the box.
- **Companion Helper Class**: Call `IParsable.Parse<T>(s)` and `IParsable.TryParse<T>(s, out var result)` anywhere without boilerplate.
- **Static Virtual Default Implementations**: Custom types only need to implement `TryParse`; the `Parse` method is automatically provided and throws an informative `FormatException` on failure.
- **Reflection & Dynamic Invocation**: Non-generic `IParsable.Parse(Type, string)` and `IParsable.TryParse(Type, string, out object?)` overloads make serializers, configuration binders, and CLI frameworks effortless.
- **Zero Configuration**: Simply reference the package and start parsing.

---

## Supported Target Frameworks

| Target Framework | Native `static abstract` | Dispatch Mode |
| :--- | :--- | :--- |
| **.NET 7.0 – 10.0+** | Yes (C# 11+) | Direct native static interface call |
| **.NET Standard 2.0 / 2.1** | No | Implyzer static routing dispatch |
| **.NET 5.0 / 6.0** | No | Implyzer static routing dispatch |

---

## Installation

### .csproj

```xml
<PackageReference Include="IParsablePolyfill" Version="*" />
```

### .NET CLI

```bash
dotnet add package IParsablePolyfill
```

### Package Manager Console

```powershell
NuGet\Install-Package IParsablePolyfill
```

### C# File-Based Scripting

```csharp
#:package IParsablePolyfill
```

---

## Usage

### 1. Parsing Standard BCL Types

```csharp
using System;
using System.Globalization;
using IParsablePolyfill;

// Generic Parse
int count = IParsable.Parse<int>("42", CultureInfo.InvariantCulture);
double price = IParsable.Parse<double>("19.99", CultureInfo.InvariantCulture);
Guid id = IParsable.Parse<Guid>("d3b07384-d113-4f01-9b16-92c25df60e22", null);

// Without format provider
int port = IParsable.Parse<int>("8080");

// TryParse
if (IParsable.TryParse<int>("123", CultureInfo.InvariantCulture, out var result))
{
    Console.WriteLine($"Parsed integer: {result}");
}
```

### 2. Implementing `IParsable<TSelf>` on Custom Types

```csharp
using System;
using System.Diagnostics.CodeAnalysis;
using IParsablePolyfill;

public readonly struct Point : IParsable<Point>
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y) => (X, Y) = (x, y);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Point result)
    {
        if (s != null)
        {
            var parts = s.Split(',');
            if (parts.Length == 2 &&
                int.TryParse(parts[0].Trim(), NumberStyles.Integer, provider, out var x) &&
                int.TryParse(parts[1].Trim(), NumberStyles.Integer, provider, out var y))
            {
                result = new Point(x, y);
                return true;
            }
        }

        result = default;
        return false;
    }

    public override string ToString() => $"({X}, {Y})";
}

// Usage: Parse is provided automatically via default implementation!
var pt = IParsable.Parse<Point>("10, 20");
```

### 3. Non-Generic / Dynamic Parsing (Type-Based)

```csharp
Type targetType = typeof(Point);
string input = "100, 200";

if (IParsable.TryParse(targetType, input, null, out object? obj))
{
    Point point = (Point)obj!;
}

object parsed = IParsable.Parse(targetType, "50, 75", null);
```

---

## Pre-Registered BCL Types

`IParsablePolyfill` comes with out-of-the-box support for common BCL types across all target frameworks:

| Category | Types | Target Framework Note |
| :--- | :--- | :--- |
| **Signed Integers** | `sbyte`, `short`, `int`, `long`, `nint` (`IntPtr`), `Int128` | `Int128` available on .NET 7+ |
| **Unsigned Integers** | `byte`, `ushort`, `uint`, `ulong`, `nuint` (`UIntPtr`), `UInt128` | `UInt128` available on .NET 7+ |
| **Arbitrary Precision** | `BigInteger` | All supported frameworks |
| **Floating-Point & Math** | `float`, `double`, `decimal`, `Complex`, `Half`, `NFloat` | `Half` on .NET 5+; `NFloat` on .NET 7+ |
| **Booleans & Chars** | `bool`, `char` | All supported frameworks |
| **Date & Time** | `DateTime`, `DateTimeOffset`, `TimeSpan`, `DateOnly`, `TimeOnly` | `DateOnly` & `TimeOnly` on .NET 6+ |
| **Identifiers & URIs** | `Guid`, `Uri`, `Version` | All supported frameworks |
| **Networking** | `IPAddress`, `IPEndPoint`, `IPNetwork` | `IPNetwork` on .NET 8+ |

---

## License

This project is licensed under the [Mozilla Public License Version 2.0 (MPL-2.0)](https://github.com/KryKomDev/IParsablePolyfill/blob/main/LICENSE.md).
