using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.CpuLimiter.Models;
using ReactiveUI;

namespace Avalonia.CpuLimiter.ViewModels;

public class HistoryItemViewModel : ViewModelBase
{
    private int? _CPUCoreUsed;

    private DateTime? _lastUsed;

    private string? _path;

    public HistoryItemViewModel()
    {
    }

    public HistoryItemViewModel(HistoryItem historyItem)
    {
        Path = historyItem.Path;
        LastUsed = historyItem.LastUsed;
        CPUCoreUsed = historyItem.CPUCoreUsed;
    }

    public string? Path
    {
        get => _path;
        set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    public DateTime? LastUsed
    {
        get => _lastUsed;
        set => this.RaiseAndSetIfChanged(ref _lastUsed, value);
    }

    public int? CPUCoreUsed
    {
        get => _CPUCoreUsed;
        set => this.RaiseAndSetIfChanged(ref _CPUCoreUsed, value);
    }

    public HistoryItem GetHistoryItem()
    {
        return new HistoryItem
        {
            Path = Path,
            CPUCoreUsed = CPUCoreUsed,
            LastUsed = LastUsed
        };
    }

    public override bool Equals(object? obj)
    {
        // call the base item equal function
        if (obj is HistoryItemViewModel other)
            return Path == other.Path && LastUsed == other.LastUsed && CPUCoreUsed == other.CPUCoreUsed;
        return false;
    }

    public override int GetHashCode()
    {
        return (Path, CPUCoreUsed, LastUsed).GetHashCode();
    }


    public static async Task RemoveDuplicatedHistoryItemAsync(ObservableCollection<HistoryItemViewModel> historyItems)
    {
        List<HistoryItemViewModel> distinctItems = historyItems.Distinct().ToList();
        historyItems.Clear();
        for (var i = 0; i < distinctItems.Count; i++) historyItems.Add(distinctItems[i]);
    }

    public static async Task SortHistoryItems(ObservableCollection<HistoryItemViewModel> historyItems)
    {
        List<HistoryItemViewModel>? tempList = historyItems.OrderByDescending(x => x.LastUsed).ToList();
        historyItems.Clear();
        for (var i = 0; i < tempList.Count(); i++) historyItems.Add(tempList[i]);
    }

    public override string ToString()
    {
        return GetHistoryItem().ToString();
    }
}