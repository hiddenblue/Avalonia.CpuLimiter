using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.CpuLimiter.Models;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.Media;
using Avalonia.Styling;

namespace Avalonia.CpuLimiter.Services;

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
            aboutInfo = new AboutInfo(),
            HistoryLimit = 5,
            ColorIndex = 0,
            StartupDecorationConfig = StartupDecoration.Fluent,
            // default culture is empty.
            StartupCultureConfig =  "",
            ThemeVariantConfig = ThemeVariant.Dark
        };
    }

    public static async Task SaveConfigAsync(MyConfigModel configModel)
    {
        if (!Directory.Exists(Path.GetDirectoryName(configPath)))
        {
            Directory.CreateDirectory(path: Path.GetDirectoryName(configPath) ?? throw new InvalidOperationException());
        }

        string oldConfig;
        
        if (File.Exists(configPath))
        {
            // backup the previous config
            oldConfig = await File.ReadAllTextAsync(configPath);
        }
        else
        {
            Console.WriteLine("try to save a config file, but it doesn't exist.");
            oldConfig = string.Empty;
        }
        
        try
        {
            await using (FileStream fs = File.Open(configPath, FileMode.Create))
            {
                await JsonSerializer.SerializeAsync(fs, configModel, MyConfigModelContext.Default.MyConfigModel);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // when code above not working, recover the old config
            await File.WriteAllTextAsync(configPath, oldConfig);
            throw;
        }




    }

    public static async Task<MyConfigModel> LoadConfigAsync()
    {
        try
        {
            if (Path.Exists(configPath))
            {
                using (FileStream fs = File.Open(configPath, FileMode.Open))
                {
                    MyConfigModel configModel =
                        JsonSerializer.Deserialize(fs,
                            MyConfigModelContext.Default.MyConfigModel);
                    if (configModel != null)
                    {
                        // convert the Themevalue read from json
                        if(configModel.ThemeVariantConfig.Key.ToString() == "Dark")
                            configModel.ThemeVariantConfig = ThemeVariant.Dark;
                        else if(configModel.ThemeVariantConfig.Key.ToString() == "Light")
                            configModel.ThemeVariantConfig = ThemeVariant.Light;
                        else
                        {
                            configModel.ThemeVariantConfig = ThemeVariant.Default;
                        }
                        return configModel;
                    }
                    else
                        throw new NullReferenceException("the file content was null");

                }
            }

            throw new FileNotFoundException("The config file was not found.");
        }
        catch (System.Text.Json.JsonException)
        {
            Console.WriteLine("Failed to load config file");
            SaveConfigAsync(GetDefaultConfigModel());
            Console.WriteLine($"Create a default config file");
            return GetDefaultConfigModel();
        }
        catch (FileNotFoundException)
        {
            SaveConfigAsync(GetDefaultConfigModel());
            Console.WriteLine($"Create a default config file");
            Console.WriteLine(@"No config file found, using default config");
            return GetDefaultConfigModel();
        }
        catch (NullReferenceException)
        {
            SaveConfigAsync(GetDefaultConfigModel());
            Console.WriteLine($"The config file could not be loaded");
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
    
    
    public static MyConfigModel LoadConfig()
    {
        try
        {
            if (Path.Exists(configPath))
            {
                using (FileStream fs = File.Open(configPath, FileMode.Open))
                {
                    MyConfigModel configModel =
                        JsonSerializer.Deserialize(fs,
                            MyConfigModelContext.Default.MyConfigModel);
                    if (configModel != null)
                    {
                        // convert the Themevalue read from json
                        if(configModel.ThemeVariantConfig.Key.ToString() == "Dark")
                            configModel.ThemeVariantConfig = ThemeVariant.Dark;
                        else if(configModel.ThemeVariantConfig.Key.ToString() == "Light")
                            configModel.ThemeVariantConfig = ThemeVariant.Light;
                        else
                        {
                            configModel.ThemeVariantConfig = ThemeVariant.Default;
                        }
                        return configModel;
                    }
                    else
                        throw new NullReferenceException("the file content was null");

                }
            }

            throw new FileNotFoundException("The config file was not found.");
        }
        catch (System.Text.Json.JsonException)
        {
            Console.WriteLine("Failed to load config file");
            SaveConfigAsync(GetDefaultConfigModel());
            Console.WriteLine($"Create a default config file");
            return GetDefaultConfigModel();
        }
        catch (FileNotFoundException)
        {
            SaveConfigAsync(GetDefaultConfigModel());
            Console.WriteLine($"Create a default config file");
            Console.WriteLine(@"No config file found, using default config");
            return GetDefaultConfigModel();
        }
        catch (NullReferenceException)
        {
            SaveConfigAsync(GetDefaultConfigModel());
            Console.WriteLine($"The config file could not be loaded");
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