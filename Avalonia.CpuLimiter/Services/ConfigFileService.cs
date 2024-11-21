using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.CpuLimiter.Models;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using LogEventLevel = Serilog.Events.LogEventLevel;

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
        // temporary logger. close before running avalonia   
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(
                "logs/logconfig.txt",
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.ff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Console(outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.ff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();


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
            Log.Information("try to save a config file, but it doesn't exist.");
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
            Log.Error("An error occured", e.Message);
            // when code above not working, recover the old config
            await File.WriteAllTextAsync(configPath, oldConfig);
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static async Task<MyConfigModel> LoadConfigAsync()
    {
        // temporary logger. close before running avalonia   
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(
                "logs/log.txt",
                rollingInterval: RollingInterval.Month,
                retainedFileCountLimit: 31,
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.ff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Console(outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.ff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

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
            Log.Error("Failed to load config file");
            await SaveConfigAsync(GetDefaultConfigModel());
            Log.Information("Create a default config file");
            return GetDefaultConfigModel();
        }
        catch (FileNotFoundException)
        {
            await SaveConfigAsync(GetDefaultConfigModel());
            Log.Information("Create a default config file");
            Log.Warning("No config file found, using default config");
            return GetDefaultConfigModel();
        }
        catch (NullReferenceException)
        {
            await SaveConfigAsync(GetDefaultConfigModel());
            Log.Information("The config file could not be loaded");
            Log.Warning("No config file found, using default config");
            return GetDefaultConfigModel();
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            Log.Error("Failed to load config file exception");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }


    public static MyConfigModel LoadConfig()
    {
        // temporary logger. close before running avalonia   
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(
                "Logs/log.txt",
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.ff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Console(outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.ff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

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
            Log.Error("Failed to load config file");
            SaveConfigAsync(GetDefaultConfigModel());
            Log.Information("Create a default config file");
            return GetDefaultConfigModel();
        }
        catch (FileNotFoundException)
        {
            SaveConfigAsync(GetDefaultConfigModel());
            Log.Information("Create a default config file");
            Log.Warning("No config file found, using default config");
            return GetDefaultConfigModel();
        }
        catch (NullReferenceException)
        {
            SaveConfigAsync(GetDefaultConfigModel());
            Log.Information("The config file could not be loaded");
            Log.Warning("No config file found, using default config");
            return GetDefaultConfigModel();
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            Log.Error("Failed to load config file exception");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}