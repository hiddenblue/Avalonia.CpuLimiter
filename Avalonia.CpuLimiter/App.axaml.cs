using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.CpuLimiter.Services;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.CpuLimiter.Views;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.CpuLimiter.Models;
using Avalonia.Media;


namespace Avalonia.CpuLimiter
{
    public partial class App : Application
    {
        public override void Initialize()
        {
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
            services.AddSingleton<MainWindow>(sp =>
            {
                MainWindow mainWindow=  new()
                {
                    DataContext = sp.GetRequiredService<MainWindowViewModel>(),
                    RequestedThemeVariant = ConfigModel.ThemeVariantConfig,
                };
                mainWindow.MainBorder.Material.TintColor = ColorCollection[ConfigModel.ColorIndex].Color;
                return mainWindow;
            });
            
            services.AddTransient<SettingWindowViewModel>();
            services.AddTransient<SettingWindow>( _ =>
            {
                var settingWindow = new SettingWindow();
      
                settingWindow.SettingBorder.Material.TintColor = ColorCollection[ConfigModel.ColorIndex].Color;
                return settingWindow;
            });
            
            services.AddTransient<AboutWindow>(_ =>
            {
                var aboutWindow = new AboutWindow();
                aboutWindow.RequestedThemeVariant = ConfigModel.ThemeVariantConfig;
                aboutWindow.AboutBorder.Material.TintColor = ColorCollection[ConfigModel.ColorIndex].Color;
                // to do theme related
                return aboutWindow;
            });
            return services.BuildServiceProvider();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                //dependency injection and load config.json from file
                Services = ConfigureServices();
                ConfigModel = ConfigFileService.LoadConfig();
                
                if(!string.IsNullOrWhiteSpace(ConfigModel.StartupCultureConfig))
                    Lang.Resources.Culture = new CultureInfo(ConfigModel.StartupCultureConfig);
                
                try
                {
                    desktop.MainWindow = Services.GetService<MainWindow>();
                    _mainWindowViewModel = desktop.MainWindow.DataContext as MainWindowViewModel;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                desktop.ShutdownRequested += DesktopOnShutdownRequested;

                ExitApplication += OnExitApplicationTriggered;
            }
            base.OnFrameworkInitializationCompleted();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime normal)
            {
                // init and load from config.json
                LoadHistoryItemToMainVM();
            }

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
        
        public Collection<CustomColor> ColorCollection { get; } = new Collection<CustomColor>
        {
            new CustomColor("#e95815"),
            new CustomColor("#f1a100"),
            new CustomColor("#f0c400"),
            new CustomColor("#e7e542"),
        
            new CustomColor("#bbd53e"),
            new CustomColor("#4fb142"),
            new CustomColor("#068bce"),
            // new CustomColor("#014da1"),
        
            // new CustomColor("#192c92"),
            // new CustomColor("#522a8b"),
            new CustomColor("#b01e4f"),
            new CustomColor("#e83a17"),
        
            new CustomColor(Colors.Silver.ToString()),
            new CustomColor(Colors.Gray.ToString()),
            new CustomColor(Colors.Black.ToString()),
            new CustomColor(Colors.Aqua.ToString()),
            new CustomColor(Colors.SkyBlue.ToString()),
            new CustomColor(Colors.DeepSkyBlue.ToString()),
            new CustomColor(Colors.LightSkyBlue.ToString())
        };
    }
}