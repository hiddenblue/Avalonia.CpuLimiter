using Avalonia.Controls;
using ReactiveUI;

namespace Avalonia.CpuLimiter.ViewModels;

public enum StartupDecoration
{
    FluentTheme,
    SimpleTheme,
    // extend
}

public class SettingWindowViewModel : ViewModelBase
{
    public SettingWindowViewModel()
    {
        HistoryLimit = 5;


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

    
}

