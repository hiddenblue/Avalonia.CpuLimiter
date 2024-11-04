﻿using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace Avalonia.CpuLimiter.Services
{
    public class FilesService : IFilesService
    {

        private readonly Window _target;

        public FilesService(Window target)
        {
            _target = target;
        }

        public async Task<IStorageFile?> OpenFileAsync()
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


            var files = await _target.StorageProvider.OpenFilePickerAsync(options);
            return files.Count >= 1 ? files[0] : null;
        }

        public async Task<IStorageFile?> SaveFileAsync()
        {
            return await _target.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = "Save Text File"
            });
        }
        
        public static FilePickerFileType WinexeFileType { get; } = new("Windows exe")
        {
            Patterns = new[] { "*.exe" },
        };
    }
}
