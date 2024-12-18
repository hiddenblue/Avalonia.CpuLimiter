﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.CpuLimiter.Models;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Serilog;
using LogEventLevel = Avalonia.Logging.LogEventLevel;

namespace Avalonia.CpuLimiter;

internal sealed class Program
{
    private static FileStream _fileLock;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // temporary logger. close before running avalonia   
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(
                "Logs/log.txt",
                rollingInterval: RollingInterval.Month,
                retainedFileCountLimit: 31,
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.ff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Console(outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.ff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        if (!CheckProcessUnique())
        {
            Log.Information("duplicate process has already been started");
            Log.Information("Exiting...");
            return;
        }

        if (!AdminRunner.IsRunAsAdmin())
        {
            Log.Information("is not run as admin");
            Log.Information("try run as admin");
            Log.Information($@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}");
            AdminRunner.RunElevated();
            Environment.Exit(0);
        }

        Log.Information("Running with Administrator privileges");

        // load logging system;

        Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

        Log.CloseAndFlush();

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            // .WithInterFont()
            .ConfigureFonts(manager => { manager.AddFontCollection(new HarmonyOSFontCollection()); })
            .With(new FontManagerOptions
            {
                DefaultFamilyName = "fonts:HarmonyOS Sans#HarmonyOS Sans SC"
            })
            .LogToTrace(LogEventLevel.Information)
            .UseReactiveUI();
    }

    private static bool CheckProcessUnique()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // add a mutex to synchronize between multi processes to stop repeat launch
            bool mutexCreated;
            const string AppName = "Avalonia.CpuLimiter";

            Mutex mutex = new(false, AppName, out mutexCreated);
            if (!mutexCreated)
            {
                Log.Warning("You can only start one instance of Avalonia.CpuLimiter.");
                return false;
            }
            Log.Information("This is a unique instance of the Avalonia.CpuLimiter.");
            return true;
        }
        else
        {
            string lockPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AboutInfo.AppName,
                ".lock");

            if (!Directory.Exists(Path.GetDirectoryName(lockPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(lockPath));
            try
            {
                _fileLock = File.Open(lockPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                _fileLock.Lock(0, 0);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}