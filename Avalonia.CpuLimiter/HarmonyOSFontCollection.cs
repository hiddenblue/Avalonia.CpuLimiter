using System;
using Avalonia.Media.Fonts;

namespace Avalonia.CpuLimiter;

public sealed class HarmonyOSFontCollection: EmbeddedFontCollection
{
    public HarmonyOSFontCollection() : base(
        new Uri("fonts:HarmonyOS Sans", UriKind.Absolute),
        new Uri("avares://Avalonia.CpuLimiter/Assets/Fonts", UriKind.Absolute))
    {
        
    }
}