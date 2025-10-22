using System;
using System.ComponentModel;

namespace InstituteManagement.Models
{
    public class Enrollment : INotifyPropertyChanged
    {
        private int _enrollmentId;
        private int _studentId;
        private int _batchId;
        private DateTime _enrollmentDate;
        private decimal _courseFee;
        private decimal _discount;
        private decimal _totalFee;
        private string _status;

        public int EnrollmentId
        {
            get => _enrollmentId;
            set { _enrollmentId = value; OnPropertyChanged(nameof(EnrollmentId)); }
        }

        public int StudentId
        {
            get => _studentId;
            set { _studentId = value; OnPropertyChanged(nameof(StudentId)); }
        }

        public int BatchId
        {
            get => _batchId;
            set { _batchId = value; OnPropertyChanged(nameof(BatchId)); }
        }

        public DateTime EnrollmentDate
        {
            get => _enrollmentDate;
            set { _enrollmentDate = value; OnPropertyChanged(nameof(EnrollmentDate)); }
        }

        public decimal CourseFee
        {
            get => _courseFee;
            set { _courseFee = value; OnPropertyChanged(nameof(CourseFee)); CalculateTotalFee(); }
        }

        public decimal Discount
        {
            get => _discount;
            set { _discount = value; OnPropertyChanged(nameof(Discount)); CalculateTotalFee(); }
        }

        public decimal TotalFee
        {
            get => _totalFee;
            set { _totalFee = value; OnPropertyChanged(nameof(TotalFee)); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        // Navigation properties - MAKE THEM SETTABLE
        public Student Student { get; set; }
        public Batch Batch { get; set; }
        public Course Course { get; set; }

        private void CalculateTotalFee()
        {
            TotalFee = CourseFee - Discount;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}