using System;
using System.Windows;
using System.Windows.Controls;
using InstituteManagement.Models;

namespace InstituteManagement.Views
{
    public partial class ReceiptWindow : Window
    {
        public Payment Payment { get; set; }

        public string ReceiptNumber => Payment?.ReceiptNumber ?? "N/A";
        public DateTime PaymentDate => Payment?.PaymentDate ?? DateTime.Now;
        public string StudentName => Payment?.Student?.FullNameEnglish ?? "N/A";
        public string CourseName => Payment?.Course?.CourseName ?? "N/A";
        public decimal Amount => Payment?.Amount ?? 0;
        public string PaymentMethod => Payment?.PaymentMethod ?? "N/A";
        public string Remarks => Payment?.Remarks ?? string.Empty;

        public string AmountInWords => ConvertAmountToWords(Amount);

        public ReceiptWindow(Payment payment)
        {
            InitializeComponent();
            Payment = payment;
            DataContext = this;
        }

        private string ConvertAmountToWords(decimal amount)
        {
            // Simple implementation - you can enhance this
            return $"{amount:#,##0} Kyats Only";
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Print functionality would be implemented here.", "Print Receipt",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}