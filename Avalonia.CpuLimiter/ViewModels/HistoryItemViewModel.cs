using System;
using Avalonia.CpuLimiter.Models;
using ReactiveUI;

namespace Avalonia.CpuLimiter.ViewModels;

public class HistoryItemViewModel : ViewModelBase
{
    public HistoryItemViewModel()
    {
    }

    public HistoryItemViewModel(HistoryItem historyItem)
    {
        Path = historyItem.Path;
        LastUsed = historyItem.LastUsed;
        CPUCoreUsed = historyItem.CPUCoreUsed;
    }

    private string? _path;
    public string? Path
    {
        get => _path;
        set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    private DateTime? _lastUsed;
    public DateTime? LastUsed
    {
        get => _lastUsed;
        set => this.RaiseAndSetIfChanged(ref _lastUsed, value);
    }

    private int? _CPUCoreUsed;

    public int? CPUCoreUsed
    {
        get => _CPUCoreUsed;
        set => this.RaiseAndSetIfChanged(ref _CPUCoreUsed, value);
    }

    public HistoryItem GetHistoryItem()
    {
        return new HistoryItem()
        {
            Path = this.Path,
            CPUCoreUsed = this.CPUCoreUsed,
            LastUsed = this.LastUsed,
        };
    }









}