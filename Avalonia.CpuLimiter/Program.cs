using Avalonia.ReactiveUI;
using System;
using Avalonia.Logging;
using Avalonia.Media;

namespace Avalonia.CpuLimiter
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            // if(!AdminRunner.IsRunAsAdmin())
            // {
            //     AdminRunner.RunElevated();
            //      Environment.Exit(0);
            // }
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                // .WithInterFont()
                .ConfigureFonts(manager =>
                {
                    manager.AddFontCollection(new HarmonyOSFontCollection());
                })
                .With(new FontManagerOptions(){
                    DefaultFamilyName = "fonts:HarmonyOS Sans#HarmonyOS Sans SC"
                })                
                .LogToTrace(LogEventLevel.Debug)
                .UseReactiveUI();
    }
}


