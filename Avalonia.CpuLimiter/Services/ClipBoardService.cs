using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.CpuLimiter.Views;

namespace Avalonia.CpuLimiter.Services;

public class ClipBoardService : IClipBoardService
{

    public ClipBoardService(IClassicDesktopStyleApplicationLifetime desktop)
    {
        _desktop = desktop;
    }

    private readonly IClassicDesktopStyleApplicationLifetime _desktop;

    public  async Task SetClipboardTextAsync(string text)
    {
        try
        {
            if(_desktop.MainWindow is  MainWindow mainWindow  && mainWindow.Clipboard is {} clipboard)
                await clipboard.SetTextAsync(text);
            throw new InvalidOperationException("Cannot set clipboard text on this window.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<string?> GetClipboardTextAsync()
    {
        try
        {
            if (_desktop.MainWindow is MainWindow mainWindow && mainWindow.Clipboard is { } clipboard)
            {
                var text =  await clipboard.GetTextAsync();
                text = text!.Trim();
            
                // remove the quote symbol in Windows
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    text = text.TrimStart('\"').TrimEnd('\"');
                return text.Trim();
            }
            throw new InvalidOperationException("Cannot get clipboard text on this window.");
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    } 

}
