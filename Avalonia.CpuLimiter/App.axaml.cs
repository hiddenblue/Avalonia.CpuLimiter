using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.CpuLimiter.Services;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.CpuLimiter.Views;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.CpuLimiter.Config;
using Avalonia.CpuLimiter.Models;


namespace Avalonia.CpuLimiter
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            //dependency injection and load config.json from file
            Services = ConfigureServices();
            ConfigModel = ConfigFileService.LoadConfigAsync();
            AvaloniaXamlLoader.Load(this);
        }
        
        public MyConfigModel ConfigModel { get; set; } = null!;
        public new static App? Current => Application.Current as App; 
        
        private MainWindowViewModel _mainWindowViewModel;
        
        public ServiceProvider? Services { get; private set; }


        private ServiceProvider ConfigureServices()
        {
            if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
                throw new PlatformNotSupportedException();
            var services = new ServiceCollection();
            services.AddSingleton<IFilesService, FilesService>(_ => new FilesService(desktop));
            services.AddSingleton<IClipBoardService, ClipBoardService>(_ => new ClipBoardService(desktop));
            services.AddSingleton<IHistoryItemFileService, HistoryItemFileService>();
            
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainWindow>(sp => new MainWindow(){ DataContext = sp.GetRequiredService<MainWindowViewModel>()});
            services.AddSingleton<SettingWindowViewModel>();
            services.AddSingleton<SettingWindow>( _ =>
            {
                var settingWindow = new SettingWindow()
                {
                    RequestedThemeVariant = ConfigModel.ThemeVariantConfig,
                    DataContext = Services.GetRequiredService<SettingWindowViewModel>()
                };
                settingWindow.SettingBorder.Material.TintColor = ConfigModel.UserColor.Color;
                return settingWindow;
            });
            
            services.AddTransient<AboutWindow>(_ =>
            {
                var aboutWindow = new AboutWindow();
                aboutWindow.RequestedThemeVariant = ConfigModel.ThemeVariantConfig;
                aboutWindow.AboutBorder.Material.TintColor = ConfigModel.UserColor.Color;
                // to do theme related
                return aboutWindow;
            });
            return services.BuildServiceProvider();
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            if(!string.IsNullOrWhiteSpace(ConfigModel.StartupCultureConfig))
                Lang.Resources.Culture = new CultureInfo(ConfigModel.StartupCultureConfig);
            
            if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = Services!.GetService<MainWindow>();
                _mainWindowViewModel = Services.GetRequiredService<MainWindowViewModel>();
            
                desktop.ShutdownRequested += DesktopOnShutdownRequested;

                ExitApplication += OnExitApplicationTriggered;
            }
            base.OnFrameworkInitializationCompleted();
            
            // init and load from config.json
            await LoadHistoryItemToMainVM();
        }

        private bool _canClose;

        // save config to local json file
        // handler to handler the close app event is triggered.
        private async void DesktopOnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
        {
            e.Cancel = !_canClose;
            
            if(!_canClose)
            {
                IEnumerable<HistoryItem> itemToSave = _mainWindowViewModel.HistoryItems.Select(item => item.GetHistoryItem());
                var historyItemFileService = Services!.GetService<IHistoryItemFileService>();
                await historyItemFileService.SaveHistoryToFileAsync(itemToSave);
                
                _canClose = true;

                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.Shutdown();
                }
            }
        }
        
        
        // load data from history.json
        private async Task LoadHistoryItemToMainVM()
        {
            var historyItemFileService = Services!.GetRequiredService<IHistoryItemFileService>();
            IEnumerable<HistoryItem>? loadItem = await historyItemFileService.LoadHistoryFromFileAsync();

            if (loadItem != null && loadItem.Count() > 0)
            {
                foreach (HistoryItem item in loadItem)
                {
                    _mainWindowViewModel.HistoryItems.Add(new HistoryItemViewModel(item));
                }
                
                await HistoryItemViewModel.RemoveDuplicatedHistoryItemAsync(_mainWindowViewModel.HistoryItems);
                
                _mainWindowViewModel.ResetIndexAndItems();
            }
        }
        
        public async void OnExitButtonClicked(object? sender, EventArgs eventArgs)
        {
            ExitApplication?.Invoke(this, EventArgs.Empty);
        }

        public  void OnExitApplicationTriggered(object? sender, EventArgs eventArgs)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Console.WriteLine("The application is exiting.");
                desktop.TryShutdown();
            } 
        }

        // the program calling this Event to exit;
        // App.Current.ExitApplication.invoke
        public event EventHandler? ExitApplication;
    }
}