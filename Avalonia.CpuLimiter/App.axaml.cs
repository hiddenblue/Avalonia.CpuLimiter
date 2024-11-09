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
using Avalonia.CpuLimiter.Models;


namespace Avalonia.CpuLimiter
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }


        private readonly MainWindowViewModel _mainWindowViewModel = new MainWindowViewModel();

        public override async void OnFrameworkInitializationCompleted()
        {
            Lang.Resources.Culture = new CultureInfo("zh-hans-cn");
            
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = _mainWindowViewModel
                };

                var services = new ServiceCollection();

                services.AddSingleton<IFilesService>(x => new FilesService(desktop.MainWindow));
                services.AddSingleton<IClipBoardService>(x => new ClipBoardService(desktop));
                _mainWindowViewModel.ClipBoardService = services.BuildServiceProvider().GetService<IClipBoardService>()!;

                Services = services.BuildServiceProvider();

                desktop.ShutdownRequested += DesktopOnShutdownRequested;

                ExitApplication += OnExitApplicationTriggered;
            }

            base.OnFrameworkInitializationCompleted();
            
            // init and load from config.json

            await InitMainWindowViewModel();
        }

        // services provider 
        // dependency injection 

        public new static App? Current => Application.Current as App;

        public IServiceProvider? Services { get; private set; }
        
        private bool _canClose;

        // save config to local json file
        // handler to handler the close app event is triggered.
        private async void DesktopOnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
        {
            e.Cancel = !_canClose;
            
            if(!_canClose)
            {
                IEnumerable<HistoryItem> itemToSave = _mainWindowViewModel.HistoryItems.Select(item => item.GetHistoryItem());

                await HistoryItemFileService.SaveHistoryToFileAsync(itemToSave);
                
                _canClose = true;

                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.Shutdown();
                }
            }
        }
        
        
        // load config from json

        private async Task InitMainWindowViewModel()
        {
            IEnumerable<HistoryItem>? loadItem = await HistoryItemFileService.LoadHistoryFromFileAsync();

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