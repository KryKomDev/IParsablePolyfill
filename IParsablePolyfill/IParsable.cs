// IParsable Polyfill 
// Copyright (c) KryKom 2026

#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using Implyzer;

namespace IParsablePolyfill;

// ReSharper disable once PartialTypeWithSinglePart

/// <summary>
/// Defines a mechanism for parsing a string to a value. Works as a replacement for
/// <see cref="IParsable{TSelf}"/>.
/// </summary>
/// <typeparam name="TSelf">The type that implements this interface.</typeparam>
[StaticAbstract("TryParse", typeof(TryParseF<>), "TSelf", "T")]
[StaticVirtual("TryParse", typeof(TryParse<>), "TSelf", "T", DefaultType = typeof(ParsableDefaultImplementations))]
[StaticVirtual("Parse",    typeof(ParseF<>),   "TSelf", "T", DefaultType = typeof(ParsableDefaultImplementations))]
[StaticVirtual("Parse",    typeof(Parse<>),    "TSelf", "T", DefaultType = typeof(ParsableDefaultImplementations))]
public partial interface IParsable<TSelf> where TSelf : IParsable<TSelf>?;

/// <summary>Tries to parse a string into a value.</summary>
/// <param name="s">The string to parse.</param>
/// <param name="provider">An object that provides culture-specific formatting information about <paramref name="s" />.</param>
/// <param name="result">On return, contains the result of successfully parsing <paramref name="s" /> or an undefined value on failure.</param>
/// <returns><c>true</c> if <paramref name="s" /> was successfully parsed; otherwise, <c>false</c>.</returns>
public delegate bool TryParseF<T>([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(returnValue: false)] out T result);

/// <summary>Tries to parse a string into a value.</summary>
/// <param name="s">The string to parse.</param>
/// <param name="result">On return, contains the result of successfully parsing <paramref name="s" /> or an undefined value on failure.</param>
/// <returns><c>true</c> if <paramref name="s" /> was successfully parsed; otherwise, <c>false</c>.</returns>
public delegate bool TryParse<T>([NotNullWhen(true)] string? s, [MaybeNullWhen(returnValue: false)] out T result);

/// <summary>Parses a string into a value.</summary>
/// <param name="s">The string to parse.</param>
/// <param name="provider">An object that provides culture-specific formatting information about <paramref name="s" />.</param>
/// <returns>The result of parsing <paramref name="s" />.</returns>
/// <exception cref="ArgumentNullException"><paramref name="s" /> is <c>null</c>.</exception>
/// <exception cref="FormatException"><paramref name="s" /> is not in the correct format.</exception>
/// <exception cref="OverflowException"><paramref name="s" /> is not representable by <typeparamref name="T" />.</exception>
public delegate T ParseF<T>(string? s, IFormatProvider? provider);

/// <summary>Parses a string into a value.</summary>
/// <param name="s">The string to parse.</param>
/// <returns>The result of parsing <paramref name="s" />.</returns>
/// <exception cref="ArgumentNullException"><paramref name="s" /> is <c>null</c>.</exception>
/// <exception cref="FormatException"><paramref name="s" /> is not in the correct format.</exception>
/// <exception cref="OverflowException"><paramref name="s" /> is not representable by <typeparamref name="T" />.</exception>
public delegate T Parse<T>(string? s);

public static class ParsableDefaultImplementations {

    public static bool TryParse<TSelf>(string? s, [MaybeNullWhen(false)] out TSelf result) {
        return IParsable.TryParse(s, null, out result);
    }
    
    public static TSelf Parse<TSelf>(string s, IFormatProvider? provider) {
        if (s == null)
            throw new ArgumentNullException(nameof(s));

        return IParsable.TryParse<TSelf>(s, provider, out var result)
            ? result!
            : throw new FormatException($"Invalid {typeof(TSelf).Name} format: '{s}'");
    }

    public static TSelf Parse<TSelf>(string s) {
        return Parse<TSelf>(s, null);
    }
}