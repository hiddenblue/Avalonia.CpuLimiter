using System.Threading.Tasks;

namespace Avalonia.CpuLimiter.Services;

public interface IClipBoardService
{
    public Task<string?> GetClipboardTextAsync();
    
    public Task SetClipboardTextAsync(string text);
}