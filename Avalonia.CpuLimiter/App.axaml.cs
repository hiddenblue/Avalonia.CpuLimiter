using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.CpuLimiter.Models;
using Avalonia.CpuLimiter.Services;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.CpuLimiter.Views;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Avalonia.CpuLimiter;

public class App : Application
{
    private bool _canClose;

    private DateTime? _lastTrayIconClicked;

    private Func<Task> _launchProgramFromMWVM;

    private MainWindowViewModel _mainWindowViewModel;

    private Func<Task> _openFileFromMWVM;

    private Func<Task> _OpenSettingFromMWVM;

    public MyConfigModel ConfigModel { get; set; }
    public new static App? Current => Application.Current as App;

    public string UserName
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return Environment.UserName;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX
                ) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                return Environment.GetEnvironmentVariable("HOME").Replace("/home/", "");

            return null;
        }
    }

    public ServiceProvider? Services { get; private set; }

    public ILogger logger { get; set; }

    public Collection<CustomColor> ColorCollection { get; } = new()
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

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            //dependency injection and load config.json from file
            // Services.Add
            ServiceCollection services = new();
            ConfigModel = ConfigFileService.LoadConfig();

            services.AddSerilogService(ConfigModel);

            Services = services.InitServices(ApplicationLifetime, ConfigModel);
            logger = Services.GetRequiredService<ILogger>();
            logger.Information("The DI system has been initialized.");
            logger.Information("The logger has been initialized. Start Logging");

            if (!string.IsNullOrWhiteSpace(ConfigModel.StartupCultureConfig))
            {
                logger.Information("The startup culture is {CultureInfo}", ConfigModel.StartupCultureConfig);
                Lang.Resources.Culture = new CultureInfo(ConfigModel.StartupCultureConfig);
            }

            try
            {
                logger.Information("the program MainWindow is running.");
                desktop.MainWindow = Services.GetService<MainWindow>();
                _mainWindowViewModel = desktop.MainWindow.DataContext as MainWindowViewModel;

                // assign a lot delegate to mainwindow
                _openFileFromMWVM += _mainWindowViewModel.ChooseExeFile;
                _launchProgramFromMWVM += _mainWindowViewModel.RunGame;
                _OpenSettingFromMWVM += _mainWindowViewModel.OpenSettingWindow;
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
            // init and load from config.json
            LoadHistoryItemToMainVM();
    }

    // save config to local json file
    // handler to handler the close app event is triggered.
    private async void DesktopOnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        e.Cancel = !_canClose;

        if (!_canClose)
        {
            IEnumerable<HistoryItem> itemToSave =
                _mainWindowViewModel.HistoryItems.Select(item => item.GetHistoryItem());
            IHistoryItemFileService? historyItemFileService = Services!.GetService<IHistoryItemFileService>();
            await historyItemFileService.SaveHistoryToFileAsync(itemToSave);

            _canClose = true;

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) desktop.Shutdown();
        }
    }


    // load data from history.json
    private async Task LoadHistoryItemToMainVM()
    {
        IHistoryItemFileService historyItemFileService = Services!.GetRequiredService<IHistoryItemFileService>();
        IEnumerable<HistoryItem>? loadItem = await historyItemFileService.LoadHistoryFromFileAsync();

        if (loadItem != null && loadItem.Count() > 0)
        {
            foreach (HistoryItem item in loadItem)
                _mainWindowViewModel.HistoryItems.Add(new HistoryItemViewModel(item));

            await HistoryItemViewModel.RemoveDuplicatedHistoryItemAsync(_mainWindowViewModel.HistoryItems);

            _mainWindowViewModel.ResetIndexAndItems();
        }
    }

    private async void OnExitButtonClicked(object? sender, EventArgs eventArgs)
    {
        ExitApplication?.Invoke(this, EventArgs.Empty);
    }

    public async void OnExitApplicationTriggered(object? sender, EventArgs eventArgs)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            logger.Information("The application is exiting.");
            desktop.TryShutdown();
        }
    }

    // the program calling this Event to exit;
    // App.Current.ExitApplication.invoke
    public event EventHandler? ExitApplication;

    private async void OnOpenFileButtonClicked(object? sender, EventArgs eventArgs)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            if (desktop.MainWindow != null)
                desktop.MainWindow.WindowState = WindowState.Normal;
        await _openFileFromMWVM();
    }


    private async void OnRestoreButtonClicked(object? sender, EventArgs eventArgs)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
            desktop.MainWindow.Show();
    }

    private async void OnLaunchProgramButtonClicked(object? sender, EventArgs eventArgs)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
            _launchProgramFromMWVM();
    }

    private async void OnOpenSettingButtonClicked(object? sender, EventArgs eventArgs)
    {
        await _OpenSettingFromMWVM();
    }

    private async void OnTrayIconClicked(object? sender, EventArgs eventArgs)
    {
        if (_lastTrayIconClicked == null)
        {
            _lastTrayIconClicked = DateTime.Now;
            return;
        }

        if (DateTime.Now.Subtract((DateTime)_lastTrayIconClicked).TotalMilliseconds > 600)
        {
            _lastTrayIconClicked = DateTime.Now;
        }
        else
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
                desktop.MainWindow.Show();
            _lastTrayIconClicked = null;
        }
    }
}