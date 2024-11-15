using System;
using Avalonia.Controls;
using Avalonia.CpuLimiter.Models;
using Avalonia.CpuLimiter.ViewModels;

namespace Avalonia.CpuLimiter.Views;

public partial class HistoryComboboxView : UserControl
{
    public HistoryComboboxView()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
        {
            this.DataContext = new HistoryItemViewModel(
                new HistoryItem()
                {
                    CPUCoreUsed = 1,
                    LastUsed = new DateTime(2018, 9, 30),

                    Path = "~/App_Data/CpuCoreHistory.json"
                });
        }
    }
    
    // private async void ResourcesChanged(object? sender, ResourcesChangedEventArgs e)
    // {
    //     HistoryComboBox.SelectedIndex = 0;
    //     // Console.WriteLine($"History ComboBox slec";
    //     Console.WriteLine($@"HistoryComboBox.SelectedIndex : {HistoryComboBox.SelectedIndex}");
    //     Console.WriteLine($@"HistoryComboBox.SelectedValue): {HistoryComboBox.SelectedValue}");
    //     Console.WriteLine($@"HistoryComboBox.SelectedItem): {HistoryComboBox.SelectedItem}");
    //     HistoryComboBox.SelectedIndex = 0;
    // }

}