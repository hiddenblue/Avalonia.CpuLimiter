using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.Media;
using Avalonia.Styling;
using Serilog;
using Serilog.Events;

namespace Avalonia.CpuLimiter.Models;

// pay attention to the JsonElement. the dynamic type need it
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(MyConfigModel))]
[JsonSerializable(typeof(ThemeVariant))]
[JsonSerializable(typeof(CustomColor))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(Color))]
[JsonSerializable(typeof(string))]
internal partial class MyConfigModelContext : JsonSerializerContext
{
}

public class MyConfigModel
{
    public MyConfigModel()
    {
    }

    public MyConfigModel(MyConfigModel myConfig)
    {
        HistoryLimit = myConfig.HistoryLimit;
        ColorIndex = myConfig.ColorIndex;
        StartupDecorationConfig = myConfig.StartupDecorationConfig;
        StartupCultureConfig = myConfig.StartupCultureConfig;
        ThemeVariantConfig = myConfig.ThemeVariantConfig;
        aboutInfo = new AboutInfo();
        serilogConfig = myConfig.serilogConfig;
    }

    public int HistoryLimit { get; set; }

    public int ColorIndex { get; set; }

    public StartupDecoration StartupDecorationConfig { get; set; }

    public string StartupCultureConfig { get; set; }

    public ThemeVariant ThemeVariantConfig { get; set; }

    public AboutInfo aboutInfo { get; set; }

    // public Collection<CustomColor> CustomColorCollection { get; set; }

    public SerilogConfig serilogConfig { get; set; }
}

public class AboutInfo
{
    public static string AppName { get; } = "Avalonia.CpuLimiter";
    public static string AppVersion { get; } = "1.0";
    public static string Author { get; } = "Hiddenblue";
    public static string DotnetVersion { get; } = "8.0";
    public static string AvaloniaUiVersion { get; } = "11.20";
    public static string License { get; } = "GPL-2.0";
}

public enum StartupDecoration
{
    Fluent,
    Simple
}

public class SerilogConfig
{
    // public LoggerMinimumLevelConfiguration MinimumLevelConfiguration { get; init; }

    public bool WriteToConsole { get; init; }

    public LogEventLevel ConsoleRestrictEventLevel { get; init; }

    public bool WriteToFile { get; init; }

    public LogEventLevel FileRestrictEventLevel { get; init; }

    public string LogPath { get; init; }

    public RollingInterval RollingInterval { get; init; }

    public int retainedFileCountLimit { get; init; }
}