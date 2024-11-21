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
            DataContext = new HistoryItemViewModel(
                new HistoryItem
                {
                    CPUCoreUsed = 1,
                    LastUsed = new DateTime(2018, 9, 30),

                    Path = "~/App_Data/CpuCoreHistory.json"
                });
    }
}