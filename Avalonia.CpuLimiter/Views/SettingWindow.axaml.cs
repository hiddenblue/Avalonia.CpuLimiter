using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;

namespace Avalonia.CpuLimiter.Views;

public partial class SettingWindow : Window
{
    public SettingWindow()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
        {
            SettingBorder.Material.TintColor = Colors.SkyBlue;
        }
        
        if(Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            ResetStateToConfig();
        

        // this.WhenAnyValue(x => x.ColorSlider.Value)
        //     .Subscribe(RefreshThemeColor);
    }


    private void OnRefreshThemeColor(object sender, RangeBaseValueChangedEventArgs e)
    {
        int colorIndex = (int)e.NewValue;
        Console.WriteLine($@"selected color changed {CustomColors[colorIndex]}");

        SettingBorder.Material.TintColor = CustomColors[colorIndex].Color;
        if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            SettingWindowViewModel vm = DataContext as SettingWindowViewModel;
            vm.ColorDigit = colorIndex;
        }
        
    }


    private List<CustomColor> CustomColors = new List<CustomColor>()
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
    
    private void OnToggleThemeButtonClicked(object? sender, RoutedEventArgs e)
    {

        if (sender is ToggleButton toggleButton)
        {
            SettingWindowViewModel vm = DataContext as SettingWindowViewModel;
            if (toggleButton.IsChecked == true)
            {
                vm.ThemeVariant = ThemeVariant.Light;
                RequestedThemeVariant = ThemeVariant.Light;
                Console.WriteLine("configmodel theme variant is set to Light");
            }
            else
            {
                vm.ThemeVariant = ThemeVariant.Dark;
                RequestedThemeVariant = ThemeVariant.Dark;
                Console.WriteLine("configmodel theme variant is set to Dark");
            }
        }
    }

    private void  OnPointerWheelChanged(object sender, PointerWheelEventArgs e)
    {
        Console.WriteLine(e.Delta);
        // e.Delta.Y value is vary between [-2, 2]
        ColorSlider.Value += e.Delta.Y;
    }


    // triggered when actived
    private void ResetStateToConfig()
    {
        var tempConfig = App.Current.ConfigModel;
        
        
        
        if (tempConfig.StartupCultureConfig == "")
            StartupLanguageToggle.IsChecked = true;
        else
        {
            StartupLanguageToggle.IsChecked = false;
        }

        if (tempConfig.ThemeVariantConfig == ThemeVariant.Dark || tempConfig.ThemeVariantConfig == ThemeVariant.Default)
        {
            StartupThemeToggle.IsChecked = false;
            RequestedThemeVariant = ThemeVariant.Dark;
        }
        else
        {
            StartupThemeToggle.IsChecked = true;
            RequestedThemeVariant = ThemeVariant.Light;
        }


    }

    private void OnHistoryLimitSpinnerChanged(object sender, SpinEventArgs e)
    {
        var spinner = (Spinner)sender;

        if (spinner.Content is TextBox textBox)
        {
            if (int.TryParse(textBox.Text, out int limitValue))
            {
                if (e.Direction == SpinDirection.Increase)
                    limitValue += 1;
                else
                {
                    limitValue -= 1;
                }

                if (DataContext is SettingWindowViewModel vm)
                {
                    vm.HistoryLimit = limitValue;
                    App.Current.ConfigModel.HistoryLimit = limitValue;
                }
            }

        }
    }

}