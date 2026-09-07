#!/usr/bin/env dotnet

#:project IParsablePolyfill/IParsablePolyfill.csproj

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using IParsablePolyfill;

Console.WriteLine("=================================================");
Console.WriteLine("       IParsable Polyfill Demonstration          ");
Console.WriteLine("=================================================\n");

// -----------------------------------------------------------------
// 1. Parsing BCL / External Types (Registered via [StaticRegister])
// -----------------------------------------------------------------
Console.WriteLine("--- 1. External / BCL Types ---");

// Generic TryParse with int
if (IParsable.TryParse<int>("42", CultureInfo.InvariantCulture, out var intResult)) {
    Console.WriteLine($"[Generic TryParse<int>]     Parsed successfully: {intResult}");
}

// Generic Parse with double
var doubleResult = IParsable.Parse<double>("3.14159", CultureInfo.InvariantCulture);
Console.WriteLine($"[Generic Parse<double>]    Parsed successfully: {doubleResult}");

// Generic Parse with Guid
var guidResult = IParsable.Parse<Guid>("d3b07384-d113-4f01-9b16-92c25df60e22", null);
Console.WriteLine($"[Generic Parse<Guid>]      Parsed successfully: {guidResult}");

// Generic Parse with DateTime
var dateResult = IParsable.Parse<DateTime>("2026-09-06T19:00:00", CultureInfo.InvariantCulture);
Console.WriteLine($"[Generic Parse<DateTime>]  Parsed successfully: {dateResult:yyyy-MM-dd HH:mm:ss}\n");

// -----------------------------------------------------------------
// 2. Custom Type Implementing IParsable<TSelf>
// -----------------------------------------------------------------
Console.WriteLine("--- 2. Custom Types Implementing IParsable<TSelf> ---");

// Point implements TryParse; Parse is automatically provided via ParsableDefaultImplementations
var point = IParsable.Parse<Point>("15, 30", null);
Console.WriteLine($"[Generic Parse<Point>]     Parsed: {point}");

if (IParsable.TryParse<Point>("invalid_point", null, out var invalidPoint))
    Console.WriteLine($"Parsed: {invalidPoint}");
else
    Console.WriteLine("[Generic TryParse<Point>]  Gracefully failed as expected on invalid input.");

// ComplexNumber overrides both Parse and TryParse
var complex = IParsable.Parse<ComplexNumber>("3+4i", null);
Console.WriteLine($"[Generic Parse<Complex>]   Parsed: {complex}\n");

// -----------------------------------------------------------------
// 3. Dynamic / Non-Generic (Type-Based) Invocation
// -----------------------------------------------------------------
Console.WriteLine("--- 3. Type-Based (Non-Generic) Invocation ---");
Type[]   typesToParse = [typeof(int), typeof(double), typeof(Guid), typeof(Point)];
string[] sampleInputs = ["100", "99.99", "a0b1c2d3-e4f5-6789-abcd-ef0123456789", "50, 75"];

for (int i = 0; i < typesToParse.Length; i++) {
    var targetType = typesToParse[i];
    var input      = sampleInputs[i];

    if (IParsable.TryParse(targetType, input, CultureInfo.InvariantCulture, out var objResult)) {
        Console.WriteLine($"[Non-generic TryParse]     {targetType.Name,-15} -> {objResult}");
    }
}

var parsedObj = IParsable.Parse(typeof(Point), "200, 400", null);
Console.WriteLine($"[Non-generic Parse]        Point           -> {parsedObj}\n");

// -----------------------------------------------------------------
// 4. Default Implementation & Error Handling
// -----------------------------------------------------------------
Console.WriteLine("--- 4. Default Implementation FormatException ---");

try {
    IParsable.Parse<Point>("not a coordinate", null);
}
catch (FormatException ex) {
    Console.WriteLine($"[Expected Exception] Caught FormatException: {ex.Message}");
}

Console.WriteLine("\nDemo completed successfully!");

// =================================================================
// Custom Type Declarations
// =================================================================

/// <summary>
/// A 2D Point type that only implements TryParse.
/// Parse is automatically handled by Implyzer's static virtual default implementation.
/// </summary>
public readonly struct Point : IParsablePolyfill.IParsable<Point> {
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y) {
        X = x;
        Y = y;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Point result) {
        if (s != null) {
            var parts = s.Split(',');

            if (parts.Length == 2                                                        &&
                int.TryParse(parts[0].Trim(), NumberStyles.Integer, provider, out var x) &&
                int.TryParse(parts[1].Trim(), NumberStyles.Integer, provider, out var y)) {
                result = new Point(x, y);

                return true;
            }
        }

        result = default;

        return false;
    }

    public override string ToString() => $"Point({X}, {Y})";
}

/// <summary>
/// A custom type that provides both TryParse and an explicit Parse override.
/// </summary>
public readonly struct ComplexNumber : IParsablePolyfill.IParsable<ComplexNumber> {
    public double Real      { get; }
    public double Imaginary { get; }

    public ComplexNumber(double real, double imaginary) {
        Real      = real;
        Imaginary = imaginary;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ComplexNumber result) {
        if (!string.IsNullOrWhiteSpace(s)) {
            var trimmed   = s.TrimEnd('i');
            var signIndex = trimmed.LastIndexOfAny(['+', '-']);

            if (signIndex > 0                                                                     &&
                double.TryParse(trimmed[..signIndex], NumberStyles.Float, provider, out var real) &&
                double.TryParse(trimmed[signIndex..], NumberStyles.Float, provider, out var imag))
            {
                result = new ComplexNumber(real, imag);

                return true;
            }
        }

        result = default;

        return false;
    }

    public static ComplexNumber Parse(string? s, IFormatProvider? provider) {
        if (TryParse(s, provider, out var result))
            return result;

        throw new FormatException($"Invalid ComplexNumber string: '{s}'");
    }

    public override string ToString() => $"{Real} + {Imaginary}i";
}