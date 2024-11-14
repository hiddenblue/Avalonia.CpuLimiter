using System.Threading.Tasks;
using Avalonia.CpuLimiter.Models;

namespace Avalonia.CpuLimiter.Services;

public interface IConfigService
{
    public Task SaveConfigAsync(MyConfigModel configModel);
    
    public Task<MyConfigModel> LoadConfigAsync();
    
}

