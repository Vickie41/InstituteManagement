using System;
using System.Windows;
using InstituteManagement.Models;

namespace InstituteManagement.Views
{
    public partial class PaymentProcessingWindow : Window
    {
        public Enrollment Enrollment { get; set; }
        public decimal AmountPaid { get; set; }
        public string SelectedPaymentMethod { get; set; }
        public string Remarks { get; set; }

        public string[] PaymentMethods => new[] { "Cash", "KBZ Pay", "Wave Money", "Bank Transfer" };

        public string StudentName => Enrollment?.Student?.FullNameEnglish ?? "N/A";
        public string CourseName => Enrollment?.Course?.CourseName ?? "N/A";
        public string BatchTime => Enrollment?.Batch?.StartTime.ToString() ?? "N/A";
        public decimal TotalFee => Enrollment?.TotalFee ?? 0;

        public PaymentProcessingWindow(Enrollment enrollment)
        {
            Enrollment = enrollment;
            SelectedPaymentMethod = "Cash";
            InitializeComponent();
            DataContext = this;
        }

        private void ProcessPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (AmountPaid <= 0)
            {
                MessageBox.Show("Please enter a valid payment amount.", "Invalid Amount",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(SelectedPaymentMethod))
            {
                MessageBox.Show("Please select a payment method.", "Payment Method Required",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}