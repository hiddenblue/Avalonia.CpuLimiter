using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.CpuLimiter.Models;
using Avalonia.Media;
using Avalonia.Styling;
using ReactiveUI;

namespace Avalonia.CpuLimiter.ViewModels;



public class SettingWindowViewModel : ViewModelBase
{

    public SettingWindowViewModel()
    {
        if (App.Current.ConfigModel is MyConfigModel configModel)
        {
            HistoryLimit = configModel.HistoryLimit;
            // ColorDigital = configModel.
            // StartupCulture = configModel.StartupCultureConfig;
            // ThemeVariant = configModel.ThemeVariantConfig;
        }
    }


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
    
    private int _colorDigital = 4;

    public int ColorDigital
    {
        get => _colorDigital;
        set => this.RaiseAndSetIfChanged(ref _colorDigital, value);
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




    
}

public class CustomColor
{
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
