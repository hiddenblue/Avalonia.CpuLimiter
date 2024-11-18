using System;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.CpuLimiter.Models;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.CpuLimiter.Views;
using Avalonia.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Logger = Serilog.Core.Logger;

namespace Avalonia.CpuLimiter.Services;

    public static class ServiceCollectionExtension
    {
        public static ServiceProvider InitServices(this IServiceCollection services, IApplicationLifetime applicationLifetime, MyConfigModel configModel)
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
                MainWindow mainWindow=  new MainWindow(sp.GetRequiredService<ILogger>())
                {
                    DataContext = sp.GetRequiredService<MainWindowViewModel>(),
                    RequestedThemeVariant = configModel.ThemeVariantConfig,
                };
                mainWindow.MainBorder.Material.TintColor =colorCollection[configModel.ColorIndex].Color;
                return mainWindow;
            });
            
            services.AddTransient<SettingWindowViewModel>(
                sp => new SettingWindowViewModel(sp.GetRequiredService<ILogger>()));
            services.AddTransient<SettingWindow>( sp =>
            {
                var settingWindow = new SettingWindow(sp.GetRequiredService<ILogger>());
      
                settingWindow.SettingBorder.Material.TintColor = colorCollection[configModel.ColorIndex].Color;
                return settingWindow;
            });
            
            services.AddTransient<AboutWindow>(sp =>
            {
                AboutWindow aboutWindow = new AboutWindow(sp.GetRequiredService<ILogger>());
                aboutWindow.RequestedThemeVariant = configModel.ThemeVariantConfig;
                aboutWindow.AboutBorder.Material.TintColor = colorCollection[configModel.ColorIndex].Color;
                // to do theme related
                return aboutWindow;
            });
            return services.BuildServiceProvider();
        } 

        public static IConfiguration AddSerilogConfiguration(this IServiceCollection services)
        {
            var environment = Environment.GetEnvironmentVariable("AVALONIA_ENVIRONMENT") ??
#if DEBUG
                              "Development"
#else
                            "Production"
#endif
                ;
        
            // we read a configiration file from current directory
            IConfigurationRoot avalConfiguration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddSingleton<IConfiguration>(avalConfiguration);
            services.AddSingleton<ILogger, Logger>(_ => 
                new LoggerConfiguration()
                .ReadFrom.Configuration(avalConfiguration)
                .CreateLogger()
                );
            return avalConfiguration;
            
        }
    }