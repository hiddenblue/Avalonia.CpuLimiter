using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System.Threading.Tasks;
using System.Windows.Input;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.CpuLimiter.Models;
using Avalonia.CpuLimiter.Services;
using Avalonia.CpuLimiter.Views;

namespace Avalonia.CpuLimiter.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            // if (!Design.IsDesignMode)
            // {
            //     if(!AdminRunner.IsRunAsAdmin())
            //     {
            //         AdminRunner.RunElevated();
            //         Environment.Exit(0);
            //     }
            // }


            ChooseExeFileCommand = ReactiveCommand.CreateFromTask(ChooseExeFile);
            RunGameCommand = ReactiveCommand.Create(RunGame);
            ExitProgramCommand = ReactiveCommand.Create(ExitProgram);
            OpenAboutWindowCommand = ReactiveCommand.CreateFromTask(OpenAboutWindowAsync);
            OpenProjWebsiteCommand = ReactiveCommand.CreateFromTask(OpenProjWebsiteAsync);
            OpenDocsCommand = ReactiveCommand.CreateFromTask(OpenDocsAsync);
            this.WhenAnyValue(x => x.CpuCoreCount)
                .Subscribe(x => Console.WriteLine($"CPU core count: {x}"));
        }

        public void RunGame() => AdminRunner.RunAsAdmin(4, GamePath);


        public ICommand ChooseExeFileCommand { get; }

        public ICommand RunGameCommand { get; }

        private string _gamePath =  "D:\\prototype\\Prototype\\prototypef.exe";

        public string GamePath
        {
            get => _gamePath;

            set
            {
                this.RaiseAndSetIfChanged(ref _gamePath, value);
            }
        }


        private async Task ChooseExeFile()
        {
            try
            {
                var fileService = App.Current?.Services?.GetService<IFilesService>();

                if (fileService is null)
                    throw new NullReferenceException("Missing File Service instance.");

                var file = await fileService.OpenFileAsync();
                if (file != null)
                    GamePath = file.Path.AbsolutePath;
                
                // extension judgement
                if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !GamePath.EndsWith(".exe"))
                    throw new PlatformNotSupportedException("File extension is not supported on windows");
                
            }
            catch (Exception e)
            {

                Console.WriteLine(e);

            }
        }
        
        // Exit command
        
        public ICommand ExitProgramCommand { get; }

        private void ExitProgram()
        {
           Environment.Exit(0); 
        }
        
        //slider cpu core

        private int _CpuCoreCount = 4;

        public int CpuCoreCount
        {
            get => _CpuCoreCount;
            
            set => this.RaiseAndSetIfChanged(ref _CpuCoreCount, value);
        }
        
        // about button
        
        public ICommand OpenAboutWindowCommand { get; }
        
        public Interaction<object, object> ShowAboutDialog { get; }

        public async Task OpenAboutWindowAsync()
        {
            await MainWindow.DoOpenAboutWindowAsync();
        }
        
        // project website button
        
        public ICommand OpenProjWebsiteCommand { get; }

        private async Task OpenProjWebsiteAsync()
        {
            string url = "https://github.com/hiddenblue/Avalonia.CpuLimiter";
            
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        
        public ICommand OpenDocsCommand { get; }

        private async Task OpenDocsAsync()
        {
            string url = "https://github.com/hiddenblue/Avalonia.CpuLimiter";
            
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
                    
            });
        }
    }
}
