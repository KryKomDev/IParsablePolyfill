// IParsable Polyfill 
// Copyright (c) KryKom 2026

#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP3_0_OR_GREATER

// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class NotNullWhenAttribute : Attribute {
    public bool ReturnValue { get; }
    public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;
}

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class MaybeNullWhenAttribute : Attribute {
    public bool ReturnValue { get; }
    public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;
}

#endif
