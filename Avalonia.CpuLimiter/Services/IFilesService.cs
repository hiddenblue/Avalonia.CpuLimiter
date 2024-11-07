using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.CpuLimiter.Services
{
    public interface IFilesService
    {
        public Task<IStorageFile?> OpenFilePickerAsync();

        public Task<IStorageFile?> SaveFilePickerAsync();
    }
}
