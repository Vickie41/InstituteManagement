using System.Configuration;
using System.Data;
using System.Windows;

namespace InstituteManagement
{
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            // Create and show main window
            var mainWindow = new Views.MainWindow();
            mainWindow.Show();
        }
    }
}