using System;
using System.Net;
using System.Numerics;
using Implyzer;

// ReSharper disable BuiltInTypeReferenceStyle

[assembly: StaticRegister(
    typeof(SByte),
    typeof(Byte),
    typeof(Int16),
    typeof(UInt16),
    typeof(Int32),
    typeof(UInt32),
    typeof(Int64),
    typeof(UInt64), 
#if NET7_0_OR_GREATER
    typeof(Int128),    
    typeof(UInt128),
    typeof(System.Runtime.InteropServices.NFloat),    
#endif
    typeof(BigInteger), 
    typeof(IntPtr),
    typeof(UIntPtr),
#if NET5_0_OR_GREATER
    typeof(Half),    
#endif
    typeof(Single),
    typeof(Double),
    typeof(Decimal),
    typeof(Complex),
    typeof(Boolean),
    typeof(Char),
    typeof(DateTime),
    typeof(DateTimeOffset), 
#if NET6_0_OR_GREATER
    typeof(DateOnly),
    typeof(TimeOnly),
#endif    
    typeof(TimeSpan),
    typeof(Guid),
    typeof(Version),
    typeof(Uri),
    typeof(IPAddress),
    typeof(IPEndPoint), 
#if NET8_0_OR_GREATER
    typeof(IPNetwork),
#endif
    TargetInterface = typeof(IParsablePolyfill.IParsable<>),
    Strict = false
)]