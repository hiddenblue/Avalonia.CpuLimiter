using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System.Threading.Tasks;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.CpuLimiter.Models;
using Avalonia.CpuLimiter.Services;
using Avalonia.CpuLimiter.Views;
using ArgumentNullException = System.ArgumentNullException;

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

            // ChooseExeFileCommand = ReactiveCommand.CreateFromTask(ChooseExeFile);
            RunGameCommand = ReactiveCommand.Create(RunGame);
            OpenAboutWindowCommand = ReactiveCommand.CreateFromTask(OpenAboutWindowAsync);
            OpenProjWebsiteCommand = ReactiveCommand.CreateFromTask(OpenProjWebsiteAsync);
            OpenDocsCommand = ReactiveCommand.CreateFromTask(OpenDocsAsync);
            RemoveHistoryItemCommand = ReactiveCommand.CreateFromTask<HistoryItemViewModel>( item => RemoveHistoryItemAsync(item) );
            
            this.WhenAnyValue(x => x.CpuCoreCount)
                .Subscribe(x => Console.WriteLine($"CPU core count: {x}"));
            
            this.WhenAnyValue(x => x.GamePath)
                .Subscribe( x => Console.WriteLine($"Game path: {x}"));

            // this.WhenAnyValue(x => x.HistoryItems)
            //     .Subscribe( x => SortHistoryItems(x));

            if (Design.IsDesignMode)
            {
                HistoryItems.Add(new HistoryItemViewModel(new HistoryItem()
                {
                    CPUCoreUsed = 1,
                    LastUsed = new DateTime(2018, 9, 30),
                    
                    Path = "~/App_Data/CpuCoreHistory.json"
                }));
                HistoryItems.Add(new HistoryItemViewModel(new HistoryItem()
                {
                    CPUCoreUsed = 2,
                    LastUsed = new DateTime(2018, 9, 30),
                    
                    Path = "~/App_Data/CpuCoreHistory.json"
                }));
                HistoryItems.Add(new HistoryItemViewModel(new HistoryItem()
                {
                    CPUCoreUsed = 3,
                    LastUsed = new DateTime(2018, 9, 30),
                    
                    Path = "~/App_Data/CpuCoreHistory.json"
                }));
                GamePath = "~/App_Data/CpuCoreHistory.json";


            }
        }

        public void RunGame()
        {
            // Path Validation
            if (string.IsNullOrWhiteSpace(GamePath))
                throw new ArgumentNullException(nameof(GamePath), $@"Path: '{GamePath}' cannot be null or empty.");
                
            else if (!File.Exists(GamePath) &&  !Directory.Exists(GamePath))
                throw new FileNotFoundException(nameof(GamePath),$@"Path: '{GamePath}' does not exist.");
            else
                AdminRunner.RunAsAdmin(4, GamePath);
        }

        // public ICommand ChooseExeFileCommand { get; }

        public ICommand RunGameCommand { get; }

        // private string _gamePath =  "D:\\prototype\\Prototype\\prototypef.exe";
        private string? _gamePath;

        [Required]
        public string? GamePath
        {
            get => _gamePath;

            set
            {

                    Console.WriteLine($@"Game path: '{value}'");
                    this.RaiseAndSetIfChanged(ref _gamePath, value);
            }
        }

        private async Task AddHistoryItemAsync(HistoryItemViewModel historyItem)
        {
            IEnumerable<string?> list = HistoryItems.Select(x => x.Path);
            if (!list.Contains(historyItem.Path))
            {
                // determine whether the item was already in HistoryItems
                HistoryItemViewModel.SortHistoryItems(HistoryItems);

                if (HistoryItems.Count() > 4)
                {
                    var items = HistoryItems.Skip(0).Take(4).ToList();
                    HistoryItems.Clear();
                    
                    foreach (var item in items)
                    {
                        HistoryItems.Add(item);
                    }
                }
                // the new item is always the first place
                HistoryItems.Insert(0, historyItem);
                SelectedComboboxIndex = 0;
                GamePath = HistoryItems[0].Path;
                
            }
            else
            {
                Console.WriteLine($@"History item: {historyItem.Path} already exists");
                
                // todo refresh the date of history item
            }
        }

        public async Task ChooseExeFile()
        {
            try
            {
                var fileService = App.Current?.Services?.GetService<IFilesService>();

                if (fileService is null)
                    throw new NullReferenceException("Missing File Service instance.");

                var file = await fileService.OpenFileAsync();
                if (file != null)
                {
                    GamePath = file.Path.LocalPath;
                
                    AddHistoryItemAsync(new HistoryItemViewModel()
                    {
                        CPUCoreUsed = CpuCoreCount,
                        LastUsed = DateTime.Now,
                        Path = GamePath
                    }); 
                }

                // extension judgement
                if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !GamePath.EndsWith(".exe"))
                    throw new PlatformNotSupportedException($"File extension: {Path.GetExtension(GamePath)} is not supported on windows");
            }
            catch (Exception e)
            {

                Console.WriteLine(e);

            }
        }
        
        // Exit command
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
        
        // combobox 

        public ObservableCollection<HistoryItemViewModel> HistoryItems
        {
            get;
        } = new();
        
        public bool CanLaunchProgram()
        {
            var value = GamePath;
            // Path Validation
            if (string.IsNullOrWhiteSpace(value))
                return false;
                
            if (!File.Exists(value) && !Directory.Exists(value))
                return false;
            
            return true;
        }

        
        private void AddHistoryItem()
        {
            // 这里的逻辑可能还要改一下
            // 需要查找，重复判断，修改
            // 感觉可以用linq语法？
            // HistoryItems.Add();
            
        }
        
        public ICommand RemoveHistoryItemCommand { get; }

        private async Task RemoveHistoryItemAsync(HistoryItemViewModel historyItem)
        {
           HistoryItems.Remove(historyItem); 
        }

        public async Task PrintLevel(object o)
        {
            Console.WriteLine(o);
        }
        
        private int _screenWidth;
        public int ScreenWidth
        {
            get
            {
                if (string.IsNullOrWhiteSpace(GamePath))
                    return 400;
                else return 3 * GamePath.Length; 
            }
            
            set => this.RaiseAndSetIfChanged(ref _screenWidth, value);
        }

        private int _selectedComboboxIndex;

        public int SelectedComboboxIndex
        {
            get => _selectedComboboxIndex;

            set
            {
                Console.WriteLine(value);
                this.RaiseAndSetIfChanged(ref _selectedComboboxIndex, value);
            }
        }

    }
}
