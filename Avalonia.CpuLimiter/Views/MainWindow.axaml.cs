using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.CpuLimiter.Models;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
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
            // HistoryBox.Items = 
            this.WhenAnyValue(x => x.HistoryComboBox.SelectionBoxItem)
                .Subscribe(Console.WriteLine);
            this.WhenAnyValue(x => x.HistoryComboBox.SelectedIndex)
                .Subscribe( x => Console.WriteLine($@"history combobox selected index {x}"));            
            this.WhenAnyValue(x => x.HistoryComboBox.SelectedValue)
                .Subscribe(x => Console.WriteLine($@"history combobox selected value {x}"));
            
            
        }

        public static async Task DoOpenAboutWindowAsync()
        {
            var aboutWindow = AboutWindow.GetInstance();
            aboutWindow.Show();
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
                Environment.Exit(0);

        }

        private async void OnOpenAboutWindowClicked(object? sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }
    }
}