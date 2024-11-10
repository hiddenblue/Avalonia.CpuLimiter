using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using ReactiveUI;

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

        // this.WhenAnyValue(x => x.ColorSlider.Value)
        //     .Subscribe(RefreshThemeColor);
    }


    private void OnRefreshThemeColor(object sender, RangeBaseValueChangedEventArgs e)
    {
        int colorIndex = (int)e.NewValue;

        Border.Material.TintColor = CustomColors[colorIndex].Color;
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
        if (RequestedThemeVariant == ThemeVariant.Dark)
            RequestedThemeVariant = ThemeVariant.Light;
        else
        {
            RequestedThemeVariant = ThemeVariant.Dark;
        }
    }

}