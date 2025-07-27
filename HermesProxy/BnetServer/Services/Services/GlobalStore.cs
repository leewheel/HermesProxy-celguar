using System;
using System.Collections.Generic;
using HermesProxy.World;

namespace BNetServer.Services;

public class GlobalStore
{
    private static readonly Lazy<GlobalStore> _instance = new(() => new GlobalStore());

    public static GlobalStore Instance => _instance.Value;

    private GlobalStore()
    {
        // private constructor to enforce singleton
    }

    public Dictionary<string, byte[]> CharacterData { get; } = new();
}