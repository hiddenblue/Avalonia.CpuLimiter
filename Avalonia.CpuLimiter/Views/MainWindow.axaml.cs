using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.CpuLimiter.Models;
using Avalonia.CpuLimiter.Services;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Avalonia.Styling;
using DynamicData.Kernel;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using Serilog;

namespace Avalonia.CpuLimiter.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        // dummpy

        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated(action =>
                action(ViewModel!.InteractionSettingWindow.RegisterHandler(DoOpenSettingsWindowAsync)));

            this.WhenAnyValue(x => x.HistoryComboBox.SelectionBoxItem)
                .Subscribe(Console.WriteLine);
            this.WhenAnyValue(x => x.HistoryComboBox.SelectedIndex)
                .Subscribe(x => Console.WriteLine($@"history combobox selected index {x}"));
            this.WhenAnyValue(x => x.HistoryComboBox.SelectedValue)
                .Subscribe(x =>
                {
                    Console.WriteLine($@"history combobox selected value {x}");
                    AutoAlterScreenWidth();
                });
            this.WhenAnyValue(view => view.HistoryComboBox.ItemCount)
                .Subscribe(count =>
                {
                    if (count > 0)
                        HistoryComboBox.SelectedIndex = 0;
                });

            this.WhenAnyValue(view => view.RequestedThemeVariant)
                .Subscribe(_ =>
                {
                    if (RequestedThemeVariant == ThemeVariant.Dark || RequestedThemeVariant == ThemeVariant.Default)
                        ToggleThemeButton.IsChecked = true;
                    else
                    {
                        ToggleThemeButton.IsChecked = false;
                    }
                });
        }

        public MainWindow(ILogger logger)
        {
            InitializeComponent();
            this._logger = logger;

            this.WhenActivated(action =>
                action(ViewModel!.InteractionSettingWindow.RegisterHandler(DoOpenSettingsWindowAsync)));

            this.WhenAnyValue(x => x.HistoryComboBox.SelectionBoxItem)
                .Subscribe(x => _logger.Debug("HistoryComboBox.SelectionBoxItem: {0}",x?.ToString()));
            this.WhenAnyValue(x => x.HistoryComboBox.SelectedIndex)
                .Subscribe( x => _logger.Debug($@"history combobox selected index {x}"));
            this.WhenAnyValue(x => x.HistoryComboBox.SelectedValue)
                .Subscribe(x =>
                {
                    _logger.Debug($"history combobox selected value {x}");
                    AutoAlterScreenWidth();
                });
            this.WhenAnyValue(view => view.HistoryComboBox.ItemCount)
                .Subscribe(count =>
                {
                    if (count > 0)
                        HistoryComboBox.SelectedIndex = 0;
                });

            this.WhenAnyValue(view => view.RequestedThemeVariant)
                .Subscribe( _ =>
                {
                    if (RequestedThemeVariant == ThemeVariant.Dark || RequestedThemeVariant == ThemeVariant.Default)
                        ToggleThemeButton.IsChecked = true;
                    else
                    {
                        ToggleThemeButton.IsChecked = false;
                    }
                });
            
            slider.Maximum = Environment.ProcessorCount;
            
            // let the window  hides to tayicon rather than be closed

#if DEBUG
            
#else
            this.Closing += (sender, args) =>
            {
                (((Window)sender!)).Hide();
                args.Cancel = true;

            };
#endif

        }
        private ILogger _logger;

        private readonly string _docsWebsiteUrl = "https://github.com/hiddenblue/Avalonia.CpuLimiter";
        
        private readonly string _projectsWebsiteUrl = "https://github.com/hiddenblue/Avalonia.CpuLimiter";

        private async Task _openUri(Uri uri)
        {
            ILauncher launcher = TopLevel.GetTopLevel(this)!.Launcher;

            await launcher.LaunchUriAsync(uri); 
        }

        private async void OnDocsButtonClicked(object? sender, RoutedEventArgs e)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Uri url = new Uri(_docsWebsiteUrl);
                await this._openUri(url);
            }
            else
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _docsWebsiteUrl,
                    UseShellExecute = true,
                    UserName = App.Current.UserName,
                });
            }
        }


        private async void OnAboutWindowButtonClicked(object? sender, RoutedEventArgs e)
        {
                AboutWindow aboutWindow = App.Current.Services.GetRequiredService<AboutWindow>();
                await aboutWindow.ShowDialog(this);
        }
        
        // project website button

        private async void OnOpenProjButtonClicked(object? sender, RoutedEventArgs e)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Uri url = new Uri(_projectsWebsiteUrl);
                await this._openUri(url);
            }
            else
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _projectsWebsiteUrl,
                    UseShellExecute = true,
                    UserName = App.Current.UserName,
                });
            }
        }

        private async void OnExitButtonClicked(object? sender, RoutedEventArgs e)
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard(new MessageBoxStandardParams()
                {
                    Width = 300,
                    Height = 120,
                    ContentTitle = "Exit",
                    ContentMessage = "Are you sure to exit?",
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    ButtonDefinitions = ButtonEnum.YesNo,
                    Icon = MsBox.Avalonia.Enums.Icon.Question
                });

            ButtonResult result = await box.ShowWindowDialogAsync(this);
            
            if(result == ButtonResult.Yes) 
               ExitApp(); 
                

        }

        private void  OnPointerWheelChanged(object sender, PointerWheelEventArgs e)
        {
            _logger.Debug("PointerWheelChanged e.Delta: {0}",e.Delta);
            // e.Delta.Y value is vary between [-2, 2]
            slider.Value += e.Delta.Y;
        }

        private async void AutoAlterScreenWidth()
        {
            _logger.Debug("Screen Width: {0}", Width);

            var selectedItem = HistoryComboBox.SelectedItem as HistoryItemViewModel;
            
            if (selectedItem is { } temp)
            {
                    var scaleParameter = 14;
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        scaleParameter = 15;
                    _logger.Debug($"Width: {Width}");
                    _logger.Debug($@"Width_compare: {temp.Path.Length * scaleParameter}");
                
                    if(Width < temp.Path.Length * scaleParameter )
                        Width = temp.Path.Length * scaleParameter;
                    if(Width > temp.Path.Length * scaleParameter * 1.4)
                        Width = temp.Path.Length * scaleParameter;
            }
        }

        // Exit command
        private async void ExitApp()
        {
            ClosedApp.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler ClosedApp = App.Current!.OnExitApplicationTriggered;

        private void OnSwtichDarkThemeButtonClicked(object? sender, RoutedEventArgs e)
        {
            RequestedThemeVariant = ThemeVariant.Dark;
            // Border.Material.TintColor = Colors.Black;
        }

        private void OnSwatchLightThemeButtonClicked(object? sender, RoutedEventArgs e)
        {
            RequestedThemeVariant = ThemeVariant.Light;
            // Border.Material.TintColor = Colors.White;
            MainBorder.Material.MaterialOpacity = 0.1;
        }

        private void OnToggleThemeButtonClicked(object? sender, RoutedEventArgs e)
        {
            if (RequestedThemeVariant == ThemeVariant.Dark)
                RequestedThemeVariant = ThemeVariant.Light;
            else
            {
                RequestedThemeVariant = ThemeVariant.Dark;
            }
        }


        public async Task DoOpenSettingsWindowAsync(
            IInteractionContext<SettingWindowViewModel, MyConfigModel> interaction)
        {
            SettingWindow  settingWindow= App.Current.Services.GetRequiredService<SettingWindow>();
            // the mvm create a settingWindowsViewModel in interaction as datacontext
            settingWindow.DataContext = interaction.Input;
            
            var result = await settingWindow.ShowDialog<MyConfigModel?>(this);
            interaction.SetOutput(result);
            
            RefreshThemeColor();
        }

        private void RefreshThemeColor()
        {
            var tempConfig = App.Current.ConfigModel;
            this.MainBorder.Material.TintColor = App.Current.ColorCollection[tempConfig.ColorIndex].Color;
            RequestedThemeVariant = tempConfig.ThemeVariantConfig;
        }


    }
}