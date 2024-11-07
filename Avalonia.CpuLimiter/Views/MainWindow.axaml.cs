using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Avalonia.Styling;
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
            AboutWindow aboutWindow = AboutWindow.GetInstance();
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

        // Add a scroll event to change slider value. the unit change is 1
        private void  OnPointerWheelChanged(object sender, PointerWheelEventArgs e)
        {
            Console.WriteLine(e.Delta);
            // e.Delta.Y value is vary between [-2, 2]
            slider.Value += e.Delta.Y;
        }

        private async void AutoAlterScreenWidth()
        {
            Console.WriteLine(Width);

            var selectedItem = HistoryComboBox.SelectionBoxItem;
            
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

        private async void OnThemeToggleSwitchClicked(object? sender, RoutedEventArgs e)
        {
            Console.WriteLine($@"OnThemeToggleSwitchClicked {RequestedThemeVariant}");
            
            if(RequestedThemeVariant == ThemeVariant.Light)
                RequestedThemeVariant = ThemeVariant.Dark;
            else if(RequestedThemeVariant == ThemeVariant.Dark)
                RequestedThemeVariant = ThemeVariant.Light;
            else if(RequestedThemeVariant == ThemeVariant.Default)
                RequestedThemeVariant = ThemeVariant.Light;
            
            Console.WriteLine($@"OnThemeToggleSwitchClicked {RequestedThemeVariant}");
        }
    }
}