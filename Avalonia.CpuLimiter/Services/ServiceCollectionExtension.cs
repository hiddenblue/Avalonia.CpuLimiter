using System;
using System.Collections.ObjectModel;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.CpuLimiter.Models;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.CpuLimiter.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Logger = Serilog.Core.Logger;

namespace Avalonia.CpuLimiter.Services;

public static class ServiceCollectionExtension
{
    public static ServiceProvider InitServices(this IServiceCollection services,
        IApplicationLifetime applicationLifetime, MyConfigModel configModel)
    {
        if (applicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            throw new PlatformNotSupportedException();
        Collection<CustomColor> colorCollection = App.Current.ColorCollection;

        services.AddSingleton<IFilesService, FilesService>(_ => new FilesService(desktop));
        services.AddSingleton<IClipBoardService, ClipBoardService>(_ => new ClipBoardService(desktop));
        services.AddSingleton<IHistoryItemFileService, HistoryItemFileService>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>(sp =>
        {
            MainWindow mainWindow = new(sp.GetRequiredService<ILogger>())
            {
                DataContext = sp.GetRequiredService<MainWindowViewModel>(),
                RequestedThemeVariant = configModel.ThemeVariantConfig
            };
            mainWindow.MainBorder.Material.TintColor = colorCollection[configModel.ColorIndex].Color;
            return mainWindow;
        });

        services.AddTransient<SettingWindowViewModel>(
            sp => new SettingWindowViewModel(sp.GetRequiredService<ILogger>()));
        services.AddTransient<SettingWindow>(sp =>
        {
            SettingWindow settingWindow = new(sp.GetRequiredService<ILogger>());

            settingWindow.SettingBorder.Material.TintColor = colorCollection[configModel.ColorIndex].Color;
            return settingWindow;
        });


        services.AddTransient<AboutWindow>(sp =>
        {
            AboutWindow aboutWindow = new(sp.GetRequiredService<ILogger>());
            aboutWindow.RequestedThemeVariant = configModel.ThemeVariantConfig;
            aboutWindow.AboutBorder.Material.TintColor = colorCollection[configModel.ColorIndex].Color;
            // to do theme related
            return aboutWindow;
        });
        Console.WriteLine("all dependency injection injected");
        return services.BuildServiceProvider();
    }

    public static void AddSerilogService(this IServiceCollection services, MyConfigModel configModel)
    {
        string environment = Environment.GetEnvironmentVariable("AVALONIA_ENVIRONMENT") ??
#if DEBUG
                             "Development";
#else
                            "Production";
#endif
        Console.WriteLine($"Environment: {environment}");


        SerilogConfig localSerilogConfig = configModel.serilogConfig;
        LoggerConfiguration loggerConfiguration = new LoggerConfiguration().MinimumLevel.Verbose();
        loggerConfiguration.Destructure.ByTransforming<object>(obj => obj?.ToString());


        // loggerConfiguration.Enrich()

        // abandon a minimum level config item

        if (localSerilogConfig.WriteToConsole)
            loggerConfiguration.WriteTo.Console(localSerilogConfig.ConsoleRestrictEventLevel,
                "{Timestamp:yyyy-MM-dd HH:mm:ss.ff} [{Level:u3}] {Message:lj}{NewLine}{Exception}");

        if (localSerilogConfig.WriteToFile && !string.IsNullOrWhiteSpace(localSerilogConfig.LogPath))
            loggerConfiguration.WriteTo.File(
                restrictedToMinimumLevel: localSerilogConfig.FileRestrictEventLevel,
                path: localSerilogConfig.LogPath,
                retainedFileCountLimit: localSerilogConfig.retainedFileCountLimit,
                rollingInterval: localSerilogConfig.RollingInterval,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.ff} [{Level:u3}] {Message:lj}{NewLine}{Exception}");


        services.AddSingleton<ILogger, Logger>(_ =>
            loggerConfiguration.CreateLogger());
    }
}