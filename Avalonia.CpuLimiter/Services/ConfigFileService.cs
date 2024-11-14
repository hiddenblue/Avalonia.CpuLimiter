using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.CpuLimiter.Models;
using Avalonia.CpuLimiter.Services;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.Media;
using Avalonia.Styling;

namespace Avalonia.CpuLimiter.Config;

public class ConfigFileService
{

    private static string configPath
    {
        get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AboutInfo.AppName,
            "config.json");
    }

    public static MyConfigModel GetDefaultConfigModel()
    {
        return new MyConfigModel()
        {
            AboutInfo = new AboutInfo(),
            HistoryLimit = 5,
            UserColor = new CustomColor(Colors.Black),
            StartupDecorationConfig = StartupDecoration.Fluent,
            // default culture is empty.
            StartupCultureConfig =  "",
            ThemeVariantConfig = ThemeVariant.Default
        };
    }

    public static async Task SaveConfigAsync(MyConfigModel configModel)
    {
       if(!Directory.Exists(Path.GetDirectoryName(configPath)))
           Directory.CreateDirectory(path: Path.GetDirectoryName(configPath) ?? throw new InvalidOperationException());

       await using (FileStream fs = File.Open(configPath, FileMode.Create))
       {
           await JsonSerializer.SerializeAsync<MyConfigModel>(fs, configModel, SourceGenerationContext.Default.MyConfigModel);
       }
    }

    // public static async Task<MyConfigModel> LoadConfigAsync()
    // {
    //     try
    //     {
    //         if (Path.Exists(configPath))
    //         {
    //             await using (FileStream fs = File.Open(configPath, FileMode.Open))
    //             {
    //                 return (await JsonSerializer.DeserializeAsync<MyConfigModel>(fs, SourceGenerationContext.Default.MyConfigModel))!;
    //             }
    //         }
    //         Console.WriteLine(@"No config file found, using default config");
    //         return await GetDefaultConfigModel();
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         Console.WriteLine(@"Failed to load config file exception");
    //         throw;
    //     }
    // }
    
    
    public static MyConfigModel LoadConfigAsync()
    {
        try
        {
            if (Path.Exists(configPath))
            {
                using (FileStream fs = File.Open(configPath, FileMode.Open))
                {
                    return ( JsonSerializer.Deserialize<MyConfigModel>(fs, SourceGenerationContext.Default.MyConfigModel))!;
                }
            }
            Console.WriteLine(@"No config file found, using default config");
            return GetDefaultConfigModel();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine(@"Failed to load config file exception");
            throw;
        }
    }
}