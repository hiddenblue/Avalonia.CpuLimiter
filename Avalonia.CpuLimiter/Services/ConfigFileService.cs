using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.CpuLimiter.Models;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Avalonia.CpuLimiter.Services;

public class ConfigFileService
{
    private static readonly ILogger _logger = App.Current.Services.GetRequiredService<ILogger>();

    private static string configPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AboutInfo.AppName,
            "config.json");

    public static MyConfigModel GetDefaultConfigModel()
    {
        return new MyConfigModel
        {
            aboutInfo = new AboutInfo(),
            HistoryLimit = 5,
            ColorIndex = 0,
            StartupDecorationConfig = StartupDecoration.Fluent,
            // default culture is empty.
            StartupCultureConfig = "",
            ThemeVariantConfig = ThemeVariant.Dark,
            serilogConfig = new SerilogConfig
            {
                // MinimumLevelConfiguration = 
                ConsoleRestrictEventLevel = LogEventLevel.Verbose,
                FileRestrictEventLevel = LogEventLevel.Debug,
                LogPath = "./Logs/log.txt",
                retainedFileCountLimit = 31,
                RollingInterval = RollingInterval.Day,
                WriteToFile = true,
                WriteToConsole = true
            }
        };
    }

    public static async Task SaveConfigAsync(MyConfigModel configModel)
    {
        if (!Directory.Exists(Path.GetDirectoryName(configPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(configPath) ?? throw new InvalidOperationException());

        string oldConfig;

        if (File.Exists(configPath))
        {
            // backup the previous config
            oldConfig = await File.ReadAllTextAsync(configPath);
        }
        else
        {
            _logger.Information("try to save a config file, but it doesn't exist.");
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
            _logger.Error("An error occured", e.Message);
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
                using (FileStream fs = File.Open(configPath, FileMode.Open))
                {
                    MyConfigModel configModel =
                        JsonSerializer.Deserialize(fs,
                            MyConfigModelContext.Default.MyConfigModel);
                    if (configModel != null)
                    {
                        // convert the Themevalue read from json
                        if (configModel.ThemeVariantConfig.Key.ToString() == "Dark")
                            configModel.ThemeVariantConfig = ThemeVariant.Dark;
                        else if (configModel.ThemeVariantConfig.Key.ToString() == "Light")
                            configModel.ThemeVariantConfig = ThemeVariant.Light;
                        else
                            configModel.ThemeVariantConfig = ThemeVariant.Default;
                        return configModel;
                    }

                    throw new NullReferenceException("the file content was null");
                }

            throw new FileNotFoundException("The config file was not found.");
        }
        catch (JsonException)
        {
            _logger.Error("Failed to load config file");
            await SaveConfigAsync(GetDefaultConfigModel());
            _logger.Information("Create a default config file");
            return GetDefaultConfigModel();
        }
        catch (FileNotFoundException)
        {
            await SaveConfigAsync(GetDefaultConfigModel());
            _logger.Information("Create a default config file");
            _logger.Warning("No config file found, using default config");
            return GetDefaultConfigModel();
        }
        catch (NullReferenceException)
        {
            await SaveConfigAsync(GetDefaultConfigModel());
            _logger.Information("The config file could not be loaded");
            _logger.Warning("No config file found, using default config");
            return GetDefaultConfigModel();
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
            _logger.Error("Failed to load config file exception");
            throw;
        }
    }


    public static MyConfigModel LoadConfig()
    {
        try
        {
            if (Path.Exists(configPath))
                using (FileStream fs = File.Open(configPath, FileMode.Open))
                {
                    MyConfigModel configModel =
                        JsonSerializer.Deserialize(fs,
                            MyConfigModelContext.Default.MyConfigModel);
                    if (configModel != null)
                    {
                        // convert the Themevalue read from json
                        if (configModel.ThemeVariantConfig.Key.ToString() == "Dark")
                            configModel.ThemeVariantConfig = ThemeVariant.Dark;
                        else if (configModel.ThemeVariantConfig.Key.ToString() == "Light")
                            configModel.ThemeVariantConfig = ThemeVariant.Light;
                        else
                            configModel.ThemeVariantConfig = ThemeVariant.Default;
                        return configModel;
                    }

                    throw new NullReferenceException("the file content was null");
                }

            throw new FileNotFoundException("The config file was not found.");
        }
        catch (JsonException)
        {
            _logger.Error("Failed to load config file");
            SaveConfigAsync(GetDefaultConfigModel());
            _logger.Information("Create a default config file");
            return GetDefaultConfigModel();
        }
        catch (FileNotFoundException)
        {
            SaveConfigAsync(GetDefaultConfigModel());
            _logger.Information("Create a default config file");
            _logger.Warning("No config file found, using default config");
            return GetDefaultConfigModel();
        }
        catch (NullReferenceException)
        {
            SaveConfigAsync(GetDefaultConfigModel());
            _logger.Information("The config file could not be loaded");
            _logger.Warning("No config file found, using default config");
            return GetDefaultConfigModel();
        }
        catch (Exception e)
        {
            _logger.Error(e.ToString());
            _logger.Error("Failed to load config file exception");
            throw;
        }
    }
}