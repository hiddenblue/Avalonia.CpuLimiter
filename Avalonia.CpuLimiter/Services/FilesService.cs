using Avalonia.Platform.Storage;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;


namespace Avalonia.CpuLimiter.Services
{
    public class FilesService : IFilesService
    {

        private readonly IClassicDesktopStyleApplicationLifetime _desktop;

        public FilesService(IClassicDesktopStyleApplicationLifetime desktop)
        {
            _desktop = desktop;

            
        }

        public async Task<IStorageFile?> OpenFilePickerAsync()
        {
            FilePickerOpenOptions options = new()
            {
                Title = "Select executable file",
                AllowMultiple = false,
            };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                options.FileTypeFilter = new FilePickerFileType[] { WinexeFileType };
            }

            if (_desktop.MainWindow?.StorageProvider is { } storageProvider)
            {
                var files = await storageProvider.OpenFilePickerAsync(options);
                return files.Count >= 1 ? files[0] : null;
            }
            throw new NullReferenceException("Missing storageProvider instance.");
        }

        public async Task<IStorageFile?> SaveFilePickerAsync()
        {
            if (_desktop.MainWindow?.StorageProvider is { } storageProvider)
            {
                return await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
                {
                    Title = "Save Text File"
                });
            }
            throw new NullReferenceException("Missing storageProvider instance.");
        }
        
        public FilePickerFileType WinexeFileType { get; } = new("Windows exe")
        {
            Patterns = new[] { "*.exe" },
        };
    }
}
