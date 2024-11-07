using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;

namespace Avalonia.CpuLimiter.Services;

public class ClipBoardService : IClipBoardService
{

    public ClipBoardService(IApplicationLifetime app)
    {
        if (app is not IClassicDesktopStyleApplicationLifetime desktop|| desktop.MainWindow?.Clipboard is not  {} provider)
            throw new NullReferenceException("Missing Clipboard instance"); 
        _provider = provider;
    }

    private readonly IClipboard _provider;

    public  async Task SetClipboardTextAsync(string text)
    {
        try
        {
            await _provider.SetTextAsync(text);
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
            var text =  await _provider.GetTextAsync();
            text = text.Trim();
            // remove the quote symbol
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                text = text.TrimStart('\"').TrimEnd('\"');
            text = text.Trim();
            return text;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    } 

}
