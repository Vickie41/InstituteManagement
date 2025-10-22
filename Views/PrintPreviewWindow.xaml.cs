using System.Collections.Generic;
using System.Windows;

namespace InstituteManagement.Views
{
    public partial class PrintPreviewWindow : Window
    {
        public object PrintContent { get; set; }

        public PrintPreviewWindow(List<object> data, string reportType)
        {
            InitializeComponent();

            // Create a simple print preview content
            var stackPanel = new System.Windows.Controls.StackPanel();
            stackPanel.Children.Add(new System.Windows.Controls.TextBlock
            {
                Text = $"{reportType} Report",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            // Add your data here for preview
            foreach (var item in data)
            {
                if (item is ReportData reportItem)
                {
                    stackPanel.Children.Add(new System.Windows.Controls.TextBlock
                    {
                        Text = $"{reportItem.Description}: {reportItem.Count} - {reportItem.Amount:C}",
                        Margin = new Thickness(0, 2, 0, 2)
                    });
                }
            }

            PrintContent = stackPanel;
            DataContext = this;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Print functionality would be implemented here.", "Print",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}