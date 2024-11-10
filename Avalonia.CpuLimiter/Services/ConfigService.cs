using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.CpuLimiter.Models;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.Media;
using Avalonia.Styling;

namespace Avalonia.CpuLimiter.Services;

public class ConfigService : IConfigService
{
    public ConfigService(string configFileName)
    {
       this.ConfigFileName = configFileName; 
    }

    public string ConfigFileName;

    private string configPath
    {
        get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AboutInfo.AppName,
            ConfigFileName);
    }

    private Color DefaultUserColor = Colors.Black;

    public MyConfigModel GetDefaultConfigModel()
    {
        return new MyConfigModel()
        {
            AboutInfo = new AboutInfo(),
            HistoryLimit = 5,
            UserColor = new CustomColor(DefaultUserColor),
            StartupDecorationConfig = StartupDecoration.FluentTheme,
            StartupCultureConfig =  "Default",
            ThemeVariantConfig = ThemeVariant.Default
         
        };
    }

    public async Task SaveConfigAsync(MyConfigModel configModel)
    {
       if(!Directory.Exists(Path.GetDirectoryName(configPath)))
           Directory.CreateDirectory(Path.GetDirectoryName(configPath));

       await using (FileStream fs = File.Open(configPath, FileMode.Create))
       {
           await JsonSerializer.SerializeAsync<MyConfigModel>(fs, configModel, SourceGenerationContext.Default.MyConfigModel);
       }
    }

    public async Task<MyConfigModel> LoadConfigAsync()
    {
        try
        {
            if (Path.Exists(configPath))
            {
                await using (FileStream fs = File.Open(configPath, FileMode.Open))
                {
                    return await JsonSerializer.DeserializeAsync<MyConfigModel?>(fs, SourceGenerationContext.Default.MyConfigModel);
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