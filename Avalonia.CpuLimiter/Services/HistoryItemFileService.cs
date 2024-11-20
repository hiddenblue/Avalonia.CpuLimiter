using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.CpuLimiter.Models;

namespace Avalonia.CpuLimiter.Services;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(IEnumerable<HistoryItem>))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}

public class HistoryItemFileService : IHistoryItemFileService
{
    private readonly string _historyDataPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AboutInfo.AppName,
            "history.json");

    // this option is conflicting with Sourcegenerator
    private JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };


    // the save config service should with save code in App.axaml.cs
    public async Task SaveHistoryToFileAsync(IEnumerable<HistoryItem>? historyItems)
    {
        if (!Directory.Exists(Path.GetDirectoryName(_historyDataPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(_historyDataPath)!);

        App.Current.logger.Information("Saving history to file SaveHistoryToFileAsync");

        try
        {
            await using (FileStream fs = File.Open(_historyDataPath, FileMode.Create))
            {
                await JsonSerializer.SerializeAsync(fs, historyItems,
                    SourceGenerationContext.Default.IEnumerableHistoryItem!);
            }
        }
        catch (Exception e)
        {
            App.Current.logger.Error("Failed to save history to file");
            App.Current.logger.Error(e.ToString());
            throw;
        }
    }


    // this load config service should work with load code in App.axaml.cs

    public async Task<IEnumerable<HistoryItem>?> LoadHistoryFromFileAsync()
    {
        try
        {
            if (!File.Exists(_historyDataPath))
                return null;

            await using (FileStream fs = File.OpenRead(_historyDataPath))
            {
                return await JsonSerializer.DeserializeAsync<IEnumerable<HistoryItem>>(fs,
                    SourceGenerationContext.Default.IEnumerableHistoryItem!);
            }
        }
        catch (Exception e) when (e is FileNotFoundException or DirectoryNotFoundException)
        {
            App.Current?.logger.Error(e.ToString());
            return null;
        }
    }
}