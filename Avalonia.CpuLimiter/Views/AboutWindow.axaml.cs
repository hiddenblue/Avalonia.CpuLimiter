using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.CpuLimiter.Models;
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
        AppName.Text = AboutInfo.AppName;
        AppVersion.Text = AboutInfo.AppVersion;
        AuthorName.Text = AboutInfo.Author;
        DotnetVersion.Text = AboutInfo.DotnetVersion;
        AvaloniaUIVersion.Text = AboutInfo.AvaloniaUiVersion;
        License.Text = AboutInfo.License;
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

    private readonly string _aboutText = $"{AboutInfo.AppName}: {AboutInfo.AppVersion}\n" + 
                                        $"Author: {AboutInfo.Author}\n" +
                                        $"Dotnet version: {AboutInfo.DotnetVersion}\n" +
                                        $"AvaloniaUI version: {AboutInfo.AvaloniaUiVersion}\n" +
                                        $"License: {AboutInfo.License}";

    /***************************************/

        /*****************************************/

}