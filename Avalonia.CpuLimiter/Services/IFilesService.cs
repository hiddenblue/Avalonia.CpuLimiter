using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Avalonia.CpuLimiter.Services;

public interface IFilesService
{
    public Task<IStorageFile?> OpenFilePickerAsync();

    public Task<IStorageFile?> SaveFilePickerAsync();
}