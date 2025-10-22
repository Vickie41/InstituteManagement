using System;
using System.ComponentModel;

namespace InstituteManagement.Models
{
    public class Payment : INotifyPropertyChanged
    {
        private int _paymentId;
        private int _enrollmentId;
        private string _receiptNumber;
        private DateTime _paymentDate;
        private decimal _amount;
        private string _paymentMethod;
        private string _remarks;
        private DateTime _createdDate;

        public int PaymentId
        {
            get => _paymentId;
            set { _paymentId = value; OnPropertyChanged(nameof(PaymentId)); }
        }

        public int EnrollmentId
        {
            get => _enrollmentId;
            set { _enrollmentId = value; OnPropertyChanged(nameof(EnrollmentId)); }
        }

        public string ReceiptNumber
        {
            get => _receiptNumber;
            set { _receiptNumber = value; OnPropertyChanged(nameof(ReceiptNumber)); }
        }

        public DateTime PaymentDate
        {
            get => _paymentDate;
            set { _paymentDate = value; OnPropertyChanged(nameof(PaymentDate)); }
        }

        public decimal Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(nameof(Amount)); }
        }

        public string PaymentMethod
        {
            get => _paymentMethod;
            set { _paymentMethod = value; OnPropertyChanged(nameof(PaymentMethod)); }
        }

        public string Remarks
        {
            get => _remarks;
            set { _remarks = value; OnPropertyChanged(nameof(Remarks)); }
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set { _createdDate = value; OnPropertyChanged(nameof(CreatedDate)); }
        }

        // Navigation properties
        public Enrollment Enrollment { get; set; }
        public Student Student { get; set; }
        public Course Course { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}