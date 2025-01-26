using OpenCvSharpProjects.ViewModels;
using System.Windows;

namespace OpenCvSharpProjects.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel(); // DataContext 설정 (선택 사항)
        }
    }
}