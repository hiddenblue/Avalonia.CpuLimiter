using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.CpuLimiter.Models;

namespace Avalonia.CpuLimiter.Services;

public class HistoryItemFileService
{
    private static string _configPath = 
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Avalonia.CpuLimiter", "history.json");

    
    // the save config service should with save code in App.axaml.cs
    public static async Task SaveHistoryToFileAsync(IEnumerable<HistoryItem>? historyItems)
    {
        if(!Directory.Exists(Path.GetDirectoryName(_configPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(_configPath)!);

        using (FileStream fs = File.Open(_configPath, FileMode.Create))
        {
            await JsonSerializer.SerializeAsync(fs, historyItems, _jsonSerializerOptions);
        }
    }
    
    
    // this load config service should work with load code in App.axaml.cs

    public static async Task<IEnumerable<HistoryItem>?> LoadHistoryFromFileAsync()
    {
        try
        {
            using (FileStream fs = File.OpenRead(_configPath))
            {
                return await JsonSerializer.DeserializeAsync<IEnumerable<HistoryItem>>(fs ,_jsonSerializerOptions);
            }
        }
        catch (Exception e) when (e is FileNotFoundException or DirectoryNotFoundException)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    private static JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };

}