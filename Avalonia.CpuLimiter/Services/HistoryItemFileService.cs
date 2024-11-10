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
[JsonSerializable(typeof(MyConfigModel))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
    
}

public class HistoryItemFileService
{
    private static string _historyDataPath = 
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Avalonia.CpuLimiter", "history.json");

    
    // the save config service should with save code in App.axaml.cs
    public static async Task SaveHistoryToFileAsync(IEnumerable<HistoryItem>? historyItems)
    {
        if(!Directory.Exists(Path.GetDirectoryName(_historyDataPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(_historyDataPath)!);

        await using (FileStream fs = File.Open(_historyDataPath, FileMode.Create))
        {
            await JsonSerializer.SerializeAsync(fs, historyItems, SourceGenerationContext.Default.IEnumerableHistoryItem!);
        }
    }
    
    
    // this load config service should work with load code in App.axaml.cs

    public static async Task<IEnumerable<HistoryItem>?> LoadHistoryFromFileAsync()
    {
        try
        {
            if(!File.Exists(_historyDataPath))
                return null;

            await using (FileStream fs = File.OpenRead(_historyDataPath))
            {
                return await JsonSerializer.DeserializeAsync<IEnumerable<HistoryItem>>(fs , SourceGenerationContext.Default.IEnumerableHistoryItem!);
            }
        }
        catch (Exception e) when (e is FileNotFoundException or DirectoryNotFoundException)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    // this option is conflicting with Sourcegenerator
    private static JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };

}