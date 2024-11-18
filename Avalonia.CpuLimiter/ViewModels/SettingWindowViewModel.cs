using System;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.CpuLimiter.Models;
using Avalonia.CpuLimiter.Services;
using Avalonia.Media;
using Avalonia.Styling;
using ReactiveUI;
using Serilog;

namespace Avalonia.CpuLimiter.ViewModels;



public class SettingWindowViewModel : ViewModelBase
{
    //dummy constructor
    public SettingWindowViewModel()
    {
        if (!Design.IsDesignMode)
        {
            // ColorDigital = configModel.
            _startupCulture = App.Current.ConfigModel.StartupCultureConfig;
            _themeVariant = App.Current?.ConfigModel.ThemeVariantConfig;
            _historyLimit = App.Current.ConfigModel.HistoryLimit;
            _colorDigit = App.Current.ConfigModel.ColorIndex;
        }
        else
        {
            _themeVariant = ThemeVariant.Dark;
            _historyLimit = 4;
        }

        SaveSettingsCommand= ReactiveCommand.CreateFromTask(() => SaveSettings());
        ChangeStartupThemeCommand = ReactiveCommand.CreateFromTask<bool>(ChangeStartupTheme);
        ChangeAppCultureCommand = ReactiveCommand.CreateFromTask<bool>(ChangeAppCulture);
        this.WhenAnyValue(vm => vm.HistoryLimit)
            .Subscribe(Console.WriteLine);
        this.WhenAnyValue(vm => vm.ColorDigit )
            .Subscribe(Console.WriteLine);
    }

    public SettingWindowViewModel(ILogger logger)
    {
        this._logger = logger;
        
        // ColorDigital = configModel.
        _startupCulture = App.Current.ConfigModel.StartupCultureConfig;
        _themeVariant = App.Current?.ConfigModel.ThemeVariantConfig;
        _historyLimit = App.Current.ConfigModel.HistoryLimit;
        _colorDigit = App.Current.ConfigModel.ColorIndex;

        SaveSettingsCommand= ReactiveCommand.CreateFromTask(() => SaveSettings());
        ChangeStartupThemeCommand = ReactiveCommand.CreateFromTask<bool>(ChangeStartupTheme);
        ChangeAppCultureCommand = ReactiveCommand.CreateFromTask<bool>(ChangeAppCulture);
        this.WhenAnyValue(vm => vm.HistoryLimit)
            .Subscribe(Console.WriteLine);
        this.WhenAnyValue(vm => vm.ColorDigit )
            .Subscribe(Console.WriteLine);
        
        
    }
    
    private ILogger _logger;


    private int _historyLimit;
    public int HistoryLimit
    {
        get => _historyLimit;
        set => this.RaiseAndSetIfChanged(ref _historyLimit, value);
    }
    
    private StartupDecoration _startupDecoration;

    public StartupDecoration StartupDecoration
    {
        get => _startupDecoration;
        set => this.RaiseAndSetIfChanged(ref _startupDecoration, value);
    }
    
    private bool _isDarkTheme;

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set => this.RaiseAndSetIfChanged(ref _isDarkTheme, value);
    }

    private int _colorDigit;

    public int ColorDigit
    {
        get => _colorDigit;
        set => this.RaiseAndSetIfChanged(ref _colorDigit, value);
    }

    private string _startupCulture;
    public string StartupCulture
    {
        get => _startupCulture;
        
        set => this.RaiseAndSetIfChanged(ref _startupCulture, value);
    }
    
    private ThemeVariant _themeVariant;

    public ThemeVariant ThemeVariant
    {
        get => _themeVariant;
        set => this.RaiseAndSetIfChanged(ref _themeVariant, value);
    }
    
    public ObservableCollection<CustomColor> ColorCollection { get; } = new ObservableCollection<CustomColor>
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

    public ICommand ChangeStartupThemeCommand { get; }
    // True is Dark False is Light
    private async Task ChangeStartupTheme(bool isChecked)
    {
        Console.WriteLine(isChecked);
        if (isChecked)
            this.ThemeVariant = ThemeVariant.Light;
        else
            this.ThemeVariant = ThemeVariant.Dark;
    }

    public ICommand SaveSettingsCommand { get; }

    private async Task SaveSettings()
    {
        Console.WriteLine("Saving settings");
        App.Current.ConfigModel.ThemeVariantConfig = this.ThemeVariant;
        App.Current.ConfigModel.StartupCultureConfig = this.StartupCulture;
        App.Current.ConfigModel.ColorIndex = ColorDigit;
        await ConfigFileService.SaveConfigAsync(App.Current.ConfigModel);
        Console.WriteLine($@"{this.ThemeVariant}, {this.StartupCulture}, {this.ColorCollection[ColorDigit]}");
    }

    public ICommand ChangeAppCultureCommand { get; }

    private async Task ChangeAppCulture(bool isChecked)
    {
        Console.WriteLine(isChecked);
        if (isChecked)
        {
            this.StartupCulture = "";
        }
        else
        {
            this.StartupCulture = "zh-hans-cn";
        }
    }

    
    //current we use static culture resources, have to restart to switch another culture.
    




    
}

public class CustomColor
{
    public CustomColor()
    {
        
    }
    public string Hex { get; }
    public Color Color { get; }

    public CustomColor(string hex)
    {
        Hex = hex;
        Color = Color.Parse(hex);
    }

    public CustomColor(Color color)
    {
        Hex = color.ToString();
        Color = color;
    }

}
