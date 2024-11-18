using Avalonia.ReactiveUI;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia.CpuLimiter.Models;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.LinuxFramebuffer;
using Microsoft.Extensions.Configuration;
using Serilog;
using Logger = Serilog.Core.Logger;

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

            if(!AdminRunner.IsRunAsAdmin())
            {
                Console.WriteLine("is not run as admin");
                Console.WriteLine("try run as admin");
                // AdminRunner.RunElevated();
                // Environment.Exit(0);
            }
            
            // load logging system;
            
            

            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
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
                .LogToTrace(LogEventLevel.Information)
                .UseReactiveUI();
    }
}


