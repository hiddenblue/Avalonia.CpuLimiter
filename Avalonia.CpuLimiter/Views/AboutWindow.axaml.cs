using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.CpuLimiter.Models;
using Avalonia.CpuLimiter.Services;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Avalonia.CpuLimiter.Views;

public partial class AboutWindow : Window, ILinuxScreen
{
    private readonly string _aboutText = $"{AboutInfo.AppName}: {AboutInfo.AppVersion}\n" +
                                         $"Author: {AboutInfo.Author}\n" +
                                         $"Dotnet version: {AboutInfo.DotnetVersion}\n" +
                                         $"AvaloniaUI version: {AboutInfo.AvaloniaUiVersion}\n" +
                                         $"License: {AboutInfo.License}";

    private readonly ILogger _logger;

    public AboutWindow(ILogger logger)
    {
        _logger = logger;
        InitializeComponent();
        AppName.Text = AboutInfo.AppName;
        AppVersion.Text = AboutInfo.AppVersion;
        AuthorName.Text = AboutInfo.Author;
        DotnetVersion.Text = AboutInfo.DotnetVersion;
        AvaloniaUIVersion.Text = AboutInfo.AvaloniaUiVersion;
        License.Text = AboutInfo.License;

        AddScreenWidth();
    }

    /***************************************/

    /*****************************************/

    public void AddScreenWidth()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            _logger.Information("mainwindow width {Width}, height {Height}", Width, Height);
            Width *= 1.1;
            _logger.Information("Width: {Width}", Width);
        }
    }

    private async void OnCloseButtonClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void OnCopyAndCloseButtonClicked(object? sender, RoutedEventArgs e)
    {
        IClipBoardService? clipBoardService = App.Current?.Services?.GetService<IClipBoardService>();
        if (clipBoardService is null)
            throw new NullReferenceException("Missing clip board service instance.");
        await clipBoardService.SetClipboardTextAsync(_aboutText);
        Close();
    }
}