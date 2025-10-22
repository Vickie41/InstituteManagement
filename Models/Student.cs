using System;
using System.ComponentModel;

namespace InstituteManagement.Models
{
    public class Student : INotifyPropertyChanged
    {
        private int _studentId;
        private string _studentCode;
        private string _fullNameEnglish;
        private string _phoneNumber;

        public int StudentId
        {
            get => _studentId;
            set { _studentId = value; OnPropertyChanged(nameof(StudentId)); }
        }

        public string StudentCode
        {
            get => _studentCode;
            set { _studentCode = value; OnPropertyChanged(nameof(StudentCode)); }
        }

        public string FullNameEnglish
        {
            get => _fullNameEnglish;
            set { _fullNameEnglish = value; OnPropertyChanged(nameof(FullNameEnglish)); }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(nameof(PhoneNumber)); }
        }

        public string FullNameMyanmar { get; set; }
        public string NRCNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ParentName { get; set; }
        public string ParentPhone { get; set; }
        public DateTime CreatedDate { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}