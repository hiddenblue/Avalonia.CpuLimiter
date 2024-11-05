using System;
using System.ComponentModel.DataAnnotations;

namespace Avalonia.CpuLimiter.Models;

public class HistoryItem
{
    public HistoryItem()
    {
    }
    
    public HistoryItem(string path, DateTime lastUsed, int count)
    {

        Path = path;
        LastUsed = lastUsed;
        CPUCoreUsed = count;
    }
    
    public string? Path { get; set; }
    
    public DateTime? LastUsed { get; set; }
    
    public int? CPUCoreUsed { get; set; }

    public override bool Equals(object? obj)
    {
        return Path == (obj as HistoryItem)?.Path;
    }

    public override int GetHashCode()
    {
        return (Path, LastUsed, CPUCoreUsed).GetHashCode();
    }
}