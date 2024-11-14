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

public class HistoryItemFileService : IHistoryItemFileService
{
    public HistoryItemFileService()
    {
        
    }

    
    private readonly string _historyDataPath = 
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AboutInfo.AppName, "history.json");

    
    // the save config service should with save code in App.axaml.cs
    public  async Task SaveHistoryToFileAsync(IEnumerable<HistoryItem>? historyItems)
    {
        if(!Directory.Exists(Path.GetDirectoryName(_historyDataPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(_historyDataPath)!);

        await using (FileStream fs = File.Open(_historyDataPath, FileMode.Create))
        {
            await JsonSerializer.SerializeAsync(fs, historyItems, SourceGenerationContext.Default.IEnumerableHistoryItem!);
        }
    }
    
    
    // this load config service should work with load code in App.axaml.cs

    public  async Task<IEnumerable<HistoryItem>?> LoadHistoryFromFileAsync()
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
    private  JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };

}