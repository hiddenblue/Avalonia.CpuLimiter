using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Avalonia.Styling;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using ReactiveUI;

namespace Avalonia.CpuLimiter.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated(action =>
                action(ViewModel!.InteractionSettingWindow.RegisterHandler(DoOpenSettingsWindowAsync)));
            
            this.WhenAnyValue(x => x.HistoryComboBox.SelectionBoxItem)
                .Subscribe(Console.WriteLine);
            this.WhenAnyValue(x => x.HistoryComboBox.SelectedIndex)
                .Subscribe( x => Console.WriteLine($@"history combobox selected index {x}"));
            this.WhenAnyValue(x => x.HistoryComboBox.SelectedValue)
                .Subscribe(x =>
                {
                    Console.WriteLine($@"history combobox selected value {x}");
                    AutoAlterScreenWidth();
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
        }

        private readonly string _docsWebsiteUrl = "https://github.com/hiddenblue/Avalonia.CpuLimiter";
        
        private readonly string _projectsWebsiteUrl = "https://github.com/hiddenblue/Avalonia.CpuLimiter";

        private async Task _openUri(Uri uri)
        {
            ILauncher launcher = TopLevel.GetTopLevel(this)!.Launcher;

            await launcher.LaunchUriAsync(uri); 
        }

        private async void OnDocsButtonClicked(object? sender, RoutedEventArgs e)
        {
            Uri url = new Uri(_docsWebsiteUrl);
            await this._openUri(url);
        }
        
        private void OnAboutWindowButtonClicked(object? sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = App.Current.Services.GetRequiredService<AboutWindow>();
            aboutWindow.Show();

        }
        
        // project website button

        private async void OnOpenProjButtonClicked(object? sender, RoutedEventArgs e)
        {
            Uri uri = new(_projectsWebsiteUrl);
            await this._openUri(uri);
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
            Console.WriteLine(e.Delta);
            // e.Delta.Y value is vary between [-2, 2]
            slider.Value += e.Delta.Y;
        }

        private async void AutoAlterScreenWidth()
        {
            Console.WriteLine(Width);

            var selectedItem = HistoryComboBox.SelectedItem as HistoryItemViewModel;
            
            if (selectedItem is HistoryItemViewModel temp)
            {
                Console.WriteLine($@"Width: {Width}");
                Console.WriteLine($@"Width_compare: {temp.Path.Length * 14}");
                
                if(Width < temp.Path.Length * 14 )
                    Width = temp.Path.Length * 14;
                if(Width > temp.Path.Length * 14 * 1.4)
                    Width = temp.Path.Length * 14;
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
            IInteractionContext<SettingWindowViewModel, SettingWindowViewModel> interaction)
        {
            SettingWindow  settingWindow= App.Current.Services.GetRequiredService<SettingWindow>();
            settingWindow.DataContext = interaction.Input;
            
            var result = await settingWindow.ShowDialog<SettingWindowViewModel?>(this);
            interaction.SetOutput(result);
        }

        private void RefreshThemeColor(CustomColor userColor)
        {
            this.MainBorder.Material.TintColor = userColor.Color;
        }

    }
}