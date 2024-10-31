using System.Threading.Tasks;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.ReactiveUI;

namespace Avalonia.CpuLimiter.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static async Task DoOpenAboutWindowAsync()
        {
            var aboutWindow = AboutWindow.GetInstance();
            aboutWindow.Show();
        }
    }
}