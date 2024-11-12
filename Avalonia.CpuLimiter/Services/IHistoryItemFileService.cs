using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.CpuLimiter.Models;

namespace Avalonia.CpuLimiter.Services;

public interface IHistoryItemFileService
{
    public Task SaveHistoryToFileAsync(IEnumerable<HistoryItem>? historyItems);

    public Task<IEnumerable<HistoryItem>?> LoadHistoryFromFileAsync();
}