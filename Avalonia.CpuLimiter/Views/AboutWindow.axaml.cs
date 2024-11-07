using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.CpuLimiter.Services;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

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

    private async void OnCloseButtonClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private async void OnCopyAndCloseButtonClicked(object? sender, RoutedEventArgs e)
    {
        IClipBoardService? clipBoardService = App.Current?.Services?.GetService<IClipBoardService>();
        if(clipBoardService is null)
            throw new NullReferenceException("Missing clip board service instance.");
        await clipBoardService.SetClipboardTextAsync(_aboutText);
        this.Close();
    }

    private readonly string _aboutText = $"Game Launcher: 1.0\n" + 
                                        $"Author: Hiddenblue\n" +
                                        $"Dotnet version: 8.0\n" +
                                        $"AvaloniaUI version: 11.20\n" +
                                        $"License: GPLV2";
    
    
}