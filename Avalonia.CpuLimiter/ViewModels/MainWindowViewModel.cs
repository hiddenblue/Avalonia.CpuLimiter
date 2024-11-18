using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System.Threading.Tasks;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.CpuLimiter.Models;
using Avalonia.CpuLimiter.Services;
using Avalonia.CpuLimiter.Views;
using Avalonia.Platform.Storage;
using Serilog;
using ArgumentNullException = System.ArgumentNullException;

namespace Avalonia.CpuLimiter.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // just for preview don't use it.
        public MainWindowViewModel()
        {
            // design mode non-parameter constructor
            // ChooseExeFileCommand = ReactiveCommand.CreateFromTask(ChooseExeFile);
            RunGameCommand = ReactiveCommand.CreateFromTask(RunGame, canSave);
            RemoveHistoryItemCommand = ReactiveCommand.CreateFromTask<HistoryItemViewModel>( item => RemoveHistoryItemAsync(item) );
            SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, canSave);
            
            // open setting window with local config model
            InteractionSettingWindow  = new Interaction<SettingWindowViewModel, MyConfigModel?>();
            OpenSettingWindowCommand = ReactiveCommand.CreateFromTask(OpenSettingWindow);
            
            this.WhenAnyValue(x => x.CpuCoreCount)
                .Subscribe(x => _logger.Debug($@"CPU core count: {x}"));
            
            this.WhenAnyValue(x => x.GamePath)
                .Subscribe( x => _logger.Debug($@"Game path: {x}"));
        
            this.WhenAnyValue(x => x.SelectedComboboxIndex)
                .Subscribe(x =>
                {
                    _logger.Debug($@"change Selected combobox index: {x}");
                    // refresh gamepath when selectedindex is not -1
                    if(HistoryItems.Count > 0 ? true : false)
                        GamePath = HistoryItems[SelectedComboboxIndex]?.Path;
                });
        
            if (Design.IsDesignMode)
            {
                HistoryItems.Add(new HistoryItemViewModel(new HistoryItem()
                {
                    CPUCoreUsed = 1,
                    LastUsed = new DateTime(2018, 9, 30),
                    
                    Path = "~/App_Data/CpuCoreHistory.json"
                }));
                GamePath = "~/App_Data/CpuCoreHistory.json";
            }
        }
        
        // real constructor
        public MainWindowViewModel(IHistoryItemFileService historyItemFileService, IClipBoardService clipBoardService,
            IFilesService filesService, ILogger logger)
        {
            this._historyItemFileService = historyItemFileService;
            this._clipBoardService = clipBoardService;
            this._filesService = filesService;
            this._logger = logger;
            
            ChooseExeFileCommand = ReactiveCommand.CreateFromTask(ChooseExeFile,
                this.WhenAnyValue(vm => vm.HistoryItems.Count, count => count < this.HistoryLimit));
            
            RunGameCommand = ReactiveCommand.CreateFromTask(RunGame, canSave);
            
            RemoveHistoryItemCommand = ReactiveCommand.CreateFromTask<HistoryItemViewModel>( RemoveHistoryItemAsync );
            SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, canSave);
            
            // open setting window with local config model
            InteractionSettingWindow  = new Interaction<SettingWindowViewModel, MyConfigModel?>();
            
            OpenSettingWindowCommand = ReactiveCommand.CreateFromTask(OpenSettingWindow);
            
            this.WhenAnyValue(x => x.CpuCoreCount)
                .Subscribe(x => _logger.Debug($@"CPU core count: {x}"));
            
            this.WhenAnyValue(x => x.GamePath)
                .Subscribe( x => _logger.Debug($@"Game path: {x}"));

            this.WhenAnyValue(x => x.SelectedComboboxIndex)
                .Subscribe(x =>
                {
                    _logger.Debug($@"change Selected combobox index: {x}");
                    // refresh gamepath when selectedindex is not -1
                    if(HistoryItems.Count > 0 ? true : false)
                        GamePath = HistoryItems[SelectedComboboxIndex]?.Path;
                });
            this.HistoryLimit = App.Current.ConfigModel.HistoryLimit;
            _logger.Debug($@"vm startup history limit.{this.HistoryLimit}");

            if (Design.IsDesignMode)
            {
                HistoryItems.Add(new HistoryItemViewModel(new HistoryItem()
                {
                    CPUCoreUsed = 1,
                    LastUsed = new DateTime(2018, 9, 30),
                    
                    Path = "~/App_Data/CpuCoreHistory.json"
                }));
                GamePath = "~/App_Data/CpuCoreHistory.json";
            }
            
        }
        
        private IHistoryItemFileService _historyItemFileService;
        private IClipBoardService _clipBoardService;
        private IFilesService _filesService;
        private ILogger _logger;

        public ICommand SaveCommand {get;}

        private IObservable<bool> canSave => this.WhenAnyValue(x =>x.HistoryItems.Count, count => count > 0);
        private async Task SaveAsync()
        {
            IEnumerable<HistoryItem> itemToSave = HistoryItems.Select(item => item.GetHistoryItem());
            await _historyItemFileService.SaveHistoryToFileAsync(itemToSave);
        }

        public async Task RunGame()
        {
            // Path Validation
            if (string.IsNullOrWhiteSpace(GamePath))
                throw new ArgumentNullException(nameof(GamePath), $@"Path: '{GamePath}' cannot be null or empty.");
                
            else if (!File.Exists(GamePath) &&  !Directory.Exists(GamePath))
                throw new FileNotFoundException(nameof(GamePath),$@"Path: '{GamePath}' does not exist.");
            _logger.Debug(GamePath);
            AdminRunner.RunAsAdmin(CpuCoreCount, GamePath);
        }

        public ICommand ChooseExeFileCommand { get; }

        public ICommand RunGameCommand { get; }

        // private string _gamePath =  "D:\\prototype\\Prototype\\prototypef.exe";
        private string? _gamePath;

        [Required]
        public string? GamePath
        {
            get => _gamePath;

            set
            {
                _logger.Debug($@"Game path: '{value}'");
                this.RaiseAndSetIfChanged(ref _gamePath, value);
            }
        }
        
        public int HistoryLimit { get; set; }

        private async Task AddHistoryItemAsync(HistoryItemViewModel historyItem)
        {
            IEnumerable<string?> list = HistoryItems.Select(x => x.Path);
            if (!list.Contains(historyItem.Path))
            {
                // determine whether the item was already in HistoryItems
                await HistoryItemViewModel.SortHistoryItems(HistoryItems);

                if (HistoryItems.Count() > HistoryLimit)
                {
                    var items = HistoryItems.Skip(0).Take(HistoryLimit).ToList();
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
                _logger.Debug($@"History item: {historyItem.Path} already exists");
                
                // todo refresh the date of history item
            }
        }

        public async Task ChooseExeFile()
        {
            try
            {
                IFilesService? fileService = App.Current?.Services?.GetService<IFilesService>();

                if (fileService is null)
                    throw new NullReferenceException("Missing File Service instance.");

                IStorageFile? file = await fileService.OpenFilePickerAsync();
                if (file != null)
                {
                    var tempPath = file.Path.LocalPath;
                
                    await AddHistoryItemAsync(new HistoryItemViewModel()
                    {
                        CPUCoreUsed = CpuCoreCount,
                        LastUsed = DateTime.Now,
                        Path = tempPath
                    });

                    if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
                        desktop.MainWindow is MainWindow mainWindow)
                        mainWindow.HistoryComboBox.SelectedIndex = 0;
                    
                    // extension judgement
                    if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !tempPath.EndsWith(".exe"))
                        throw new PlatformNotSupportedException($"File extension: {Path.GetExtension(GamePath)} is not supported on windows");
                }
            }
            catch (Exception e)
            {
                _logger.Debug(e.ToString());
            }
        }

        public int HostCpuCoreCount => Environment.ProcessorCount;
        
        //slider cpu core

        private int _CpuCoreCount = 4;

        public int CpuCoreCount
        {
            get => _CpuCoreCount;
            set => this.RaiseAndSetIfChanged(ref _CpuCoreCount, value);
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

        public ICommand RemoveHistoryItemCommand { get; }

        private async Task RemoveHistoryItemAsync(HistoryItemViewModel historyItem)
        { 
            _logger.Debug("{@historyItem)}", historyItem);
            _logger.Debug("{HistoryItems.Contains(historyItem)}", HistoryItems.Contains(historyItem));

            try
            {
                var index = HistoryItems.IndexOf(historyItem);
                HistoryItems.RemoveAt(index);
                await HistoryItemViewModel.SortHistoryItems(HistoryItems);
                _logger.Debug($@"Removed history item: {historyItem.Path}");
                if(HistoryItems.Count == 0) GamePath = null;

            }
            catch (Exception e)
            {
                _logger.Debug("{@e}", e);
                _logger.Debug($@"Failed to remove history item: {historyItem.Path}");
            }
        }
        
        private async Task RemoveHistoryItemAsync(int index)
        { 
            _logger.Debug($"to remove index : {index}");
            try
            {
                HistoryItems.RemoveAt(index);
                await HistoryItemViewModel.SortHistoryItems(HistoryItems);
                _logger.Debug($@"Removed history item index: {index}");
                if(HistoryItems.Count == 0) GamePath = null;

            }
            catch (Exception e)
            {
                _logger.Error("{e}", e);
                _logger.Debug($@"Failed to remove history index : {index}");
            }
        }

        private int _selectedComboboxIndex;

        public int SelectedComboboxIndex
        {
            get => _selectedComboboxIndex;

            set
            {
                _logger.Debug("{value}", value);
                this.RaiseAndSetIfChanged(ref _selectedComboboxIndex, value);
            }
        }
        
        private HistoryItemViewModel _selectedComboboxItem;
        
        public HistoryItemViewModel SelectedHistoryItem
        {
            get => _selectedComboboxItem;
        
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedComboboxItem, value);
            }
        }
        
        // clipboard command

        public IClipBoardService ClipBoardService;
        
        public async Task CopyTextToClipboard(string text)
        {
            // text is pass by button command parameter
            await ClipBoardService.SetClipboardTextAsync(text);
        }
        
        public async Task PastePathFromClipboard()
        {
            string? path = await ClipBoardService.GetClipboardTextAsync();
            if (path is not null)
            {
                await AddHistoryItemAsync(new HistoryItemViewModel()
                {
                    CPUCoreUsed = this.CpuCoreCount,
                    LastUsed = DateTime.Now,
                    Path = path
                });
            }
            else
            {
                _logger.Debug($@"Clipboard text is empty");
            }
        }

        public async Task CutTextToClipboard(string text)
        {
            await ClipBoardService.SetClipboardTextAsync(text);
            await RemoveHistoryItemAsync(SelectedComboboxIndex);
        }

        public void ResetIndexAndItems()
        {
            SelectedHistoryItem = HistoryItems[SelectedComboboxIndex];
            GamePath = SelectedHistoryItem.Path;
        }
        
        // open the setting window

        public Interaction<SettingWindowViewModel, MyConfigModel?> InteractionSettingWindow { get; }

        public ICommand OpenSettingWindowCommand { get; }

        public async Task OpenSettingWindow()
        {
            SettingWindowViewModel settingModel =
                App.Current.Services.GetRequiredService<SettingWindowViewModel>();
            MyConfigModel? result = await InteractionSettingWindow.Handle(settingModel); 
        }
        
        
        // config 

        public MyConfigModel MainWindowConfigModel { get; set; } }
}
