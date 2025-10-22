using System.Windows;
using InstituteManagement.ViewModels;

namespace InstituteManagement.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}