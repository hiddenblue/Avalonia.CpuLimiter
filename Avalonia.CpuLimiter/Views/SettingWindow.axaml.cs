using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Avalonia.CpuLimiter.Views;

public partial class SettingWindow : Window
{
    public SettingWindow()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
        {
            Border.Material.TintColor = Colors.SkyBlue;
        }
    }
}