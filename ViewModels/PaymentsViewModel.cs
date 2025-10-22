using CommunityToolkit.Mvvm.Input;
using InstituteManagement.Common;
using InstituteManagement.Data;
using InstituteManagement.Models;
using InstituteManagement.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace InstituteManagement.ViewModels
{
    public class PaymentsViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;

        private ObservableCollection<Models.Payment> _payments;
        public ObservableCollection<Models.Payment> Payments
        {
            get => _payments;
            set { _payments = value; OnPropertyChanged(nameof(Payments)); }
        }

        private ObservableCollection<Enrollment> _pendingEnrollments;
        public ObservableCollection<Enrollment> PendingEnrollments
        {
            get => _pendingEnrollments;
            set { _pendingEnrollments = value; OnPropertyChanged(nameof(PendingEnrollments)); }
        }

        private Models.Payment _selectedPayment;
        public Models.Payment SelectedPayment
        {
            get => _selectedPayment;
            set { _selectedPayment = value; OnPropertyChanged(nameof(SelectedPayment)); }
        }

        private Enrollment _selectedEnrollment;
        public Enrollment SelectedEnrollment
        {
            get => _selectedEnrollment;
            set { _selectedEnrollment = value; OnPropertyChanged(nameof(SelectedEnrollment)); }
        }

        public RelayCommand ProcessPaymentCommand { get; }
        public RelayCommand GenerateReceiptCommand { get; }
        public RelayCommand RefreshPaymentsCommand { get; }

        public PaymentsViewModel()
        {
            _dbService = new DatabaseService();
            Payments = new ObservableCollection<Models.Payment>();
            PendingEnrollments = new ObservableCollection<Enrollment>();

            ProcessPaymentCommand = new RelayCommand(ProcessPayment);
            GenerateReceiptCommand = new RelayCommand(GenerateReceipt);
            RefreshPaymentsCommand = new RelayCommand(RefreshData);

            LoadData();
        }

        private void LoadData()
        {
            RefreshPayments();
            LoadPendingEnrollments();
        }

        private void ProcessPayment()
        {
            if (SelectedEnrollment == null)
            {
                MessageBox.Show("Please select an enrollment to process payment.", "Information",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var paymentWindow = new PaymentProcessingWindow(SelectedEnrollment);
            if (paymentWindow.ShowDialog() == true)
            {
                var payment = new Models.Payment
                {
                    EnrollmentId = SelectedEnrollment.EnrollmentId,
                    Amount = paymentWindow.AmountPaid,
                    PaymentMethod = paymentWindow.SelectedPaymentMethod,
                    Remarks = paymentWindow.Remarks,
                    PaymentDate = DateTime.Now,
                    CreatedDate = DateTime.Now
                };

                var receiptNumber = _dbService.AddPayment(payment);

                if (!string.IsNullOrEmpty(receiptNumber))
                {
                    payment.ReceiptNumber = receiptNumber;
                    var receiptWindow = new ReceiptWindow(payment);
                    receiptWindow.ShowDialog();
                    RefreshData();
                }
            }
        }

        private void GenerateReceipt()
        {
            if (SelectedPayment == null)
            {
                MessageBox.Show("Please select a payment to generate receipt.", "Information",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var receiptWindow = new ReceiptWindow(SelectedPayment);
            receiptWindow.ShowDialog();
        }

        private void RefreshData()
        {
            RefreshPayments();
            LoadPendingEnrollments();
        }

        private void RefreshPayments()
        {
            Payments.Clear();
            var payments = _dbService.GetRecentPayments(50);
            foreach (var payment in payments)
            {
                Payments.Add(payment);
            }
        }

        private void LoadPendingEnrollments()
        {
            PendingEnrollments.Clear();
            var enrollments = _dbService.GetPendingEnrollments();
            foreach (var enrollment in enrollments)
            {
                PendingEnrollments.Add(enrollment);
            }
        }
    }
}