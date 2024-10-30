using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System.Threading.Tasks;
using System.Windows.Input;
using System;
using Avalonia.CpuLimiter.Models;
using Avalonia.CpuLimiter.Services;

namespace Avalonia.CpuLimiter.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            if(!AdminRunner.IsRunAsAdmin())
            {
                AdminRunner.RunElevated();
                Environment.Exit(0);
            }
            ChooseExeFileCommand = ReactiveCommand.CreateFromTask(ChooseExeFile);
            RunGameCommand = ReactiveCommand.Create(RunGame);

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
                    GamePath = file.Path.ToString();           

            }
            catch (Exception e)
            {

                Console.WriteLine(e);

            }
        }
    }
}
