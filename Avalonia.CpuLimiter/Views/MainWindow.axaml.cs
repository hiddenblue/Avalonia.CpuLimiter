using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using Avalonia.Visuals;

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
                .Subscribe(x => Console.WriteLine($@"history combobox selected value {x}"));
        }

        private async void OnDocsButtonClicked(object? sender, RoutedEventArgs e)
        {
            Uri url = new Uri("https://github.com/hiddenblue/Avalonia.CpuLimiter");

            ILauncher launcher = TopLevel.GetTopLevel(this)!.Launcher;

            await launcher.LaunchUriAsync(url);
        }
        
        private void OnAboutWindowButtonClicked(object? sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = AboutWindow.GetInstance();
            aboutWindow.Show();
        }
        
        // project website button

        private async void OnOpenProjButtonClicked(object? sender, RoutedEventArgs e)
        {
            Uri uri = new("https://github.com/hiddenblue/Avalonia.CpuLimiter");
            
            ILauncher launcher = TopLevel.GetTopLevel(this)!.Launcher;

            await launcher.LaunchUriAsync(uri);
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

        private async void OnOpenAboutWindowClicked(object? sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }
        
        
        // Add a scroll event to change slider value. the unit change is 1
        private void  OnPointerWheelChanged(object sender, PointerWheelEventArgs e)
        {
            Console.WriteLine(e.Delta);
            // e.Delta.Y value is vary between [-2, 2]
            slider.Value += e.Delta.Y;
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // if select another, both added and remove are exist.
            // if removed, no addeditems referrence
            if (e.AddedItems.Count != 0)
            {
                var historyItemViewModel = (HistoryItemViewModel)e.AddedItems[0]!;
                slider.Value = (double)historyItemViewModel.CPUCoreUsed!;
                Auxiliary.Text = historyItemViewModel.Path;
                
                if(Width < historyItemViewModel.Path.Length * 15 )
                    Width = historyItemViewModel.Path.Length * 15;
                if(Width > historyItemViewModel.Path.Length * 15 * 1.5)
                    Width = historyItemViewModel.Path.Length * 15;

                Console.WriteLine(historyItemViewModel.CPUCoreUsed);
                Console.WriteLine(historyItemViewModel.Path);
                Console.WriteLine(historyItemViewModel.LastUsed);
            }
            else if (e.AddedItems.Count == 0)
            {
                // var historyItemViewModel = 
            }

        }

        private async void ResourcesChanged(object? sender, ResourcesChangedEventArgs e)
        {
            HistoryComboBox.SelectedIndex = 0;
            // Console.WriteLine($"History ComboBox slec";
            Console.WriteLine($@"HistoryComboBox.SelectedIndex : {HistoryComboBox.SelectedIndex}");
            Console.WriteLine($@"HistoryComboBox.SelectedValue): {HistoryComboBox.SelectedValue}");
            Console.WriteLine($@"HistoryComboBox.SelectedItem): {HistoryComboBox.SelectedItem}");
            HistoryComboBox.SelectedIndex = 0;


        }
        
        
        // Exit command
        private void ExitApp()
        {
            ClosedApp.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler ClosedApp = App.Current!.OnExitApplicationTriggered;
        
    }
}