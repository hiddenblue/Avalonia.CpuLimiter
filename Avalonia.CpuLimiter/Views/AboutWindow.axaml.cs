using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.CpuLimiter.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
    }

    // Add singleton AboutWindow
    public static AboutWindow Instance;
    
    
    public static AboutWindow GetInstance()
    {
        Instance = new AboutWindow();
        return Instance;
    }
}