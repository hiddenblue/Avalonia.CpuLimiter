using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.CpuLimiter.Lang;
using Avalonia.Platform.Storage;

namespace Avalonia.CpuLimiter.Services;

public class FilesService : IFilesService
{
    private readonly IClassicDesktopStyleApplicationLifetime _desktop;

    public FilesService(IClassicDesktopStyleApplicationLifetime desktop)
    {
        _desktop = desktop;
    }

    public FilePickerFileType WinexeFileType { get; } = new("Windows exe")
    {
        Patterns = new[] { "*.exe" }
    };

    public async Task<IStorageFile?> OpenFilePickerAsync()
    {
        FilePickerOpenOptions options = new()
        {
            // Title = "Select executable file",
            Title = $@"{Resources.FilePickerTitle}",
            AllowMultiple = false
        };

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) options.FileTypeFilter = new[] { WinexeFileType };

        if (_desktop.MainWindow?.StorageProvider is { } storageProvider)
        {
            IReadOnlyList<IStorageFile>? files = await storageProvider.OpenFilePickerAsync(options);
            return files.Count >= 1 ? files[0] : null;
        }

        throw new NullReferenceException("Missing storageProvider instance.");
    }

    public async Task<IStorageFile?> SaveFilePickerAsync()
    {
        if (_desktop.MainWindow?.StorageProvider is { } storageProvider)
            return await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = $"{Resources.SaveFilePickerTitle}"
            });
        throw new NullReferenceException("Missing storageProvider instance.");
    }
}