using System.ComponentModel;

namespace InstituteManagement.Models
{
    public class Course : INotifyPropertyChanged
    {
        private int _courseId;
        private string _courseName;
        private string _description;
        private int _durationWeeks;
        private int _totalHours;
        private decimal _fee;

        public int CourseId
        {
            get => _courseId;
            set { _courseId = value; OnPropertyChanged(nameof(CourseId)); }
        }

        public string CourseName
        {
            get => _courseName;
            set { _courseName = value; OnPropertyChanged(nameof(CourseName)); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        public int DurationWeeks
        {
            get => _durationWeeks;
            set { _durationWeeks = value; OnPropertyChanged(nameof(DurationWeeks)); }
        }

        public int TotalHours
        {
            get => _totalHours;
            set { _totalHours = value; OnPropertyChanged(nameof(TotalHours)); }
        }

        public decimal Fee
        {
            get => _fee;
            set { _fee = value; OnPropertyChanged(nameof(Fee)); }
        }

        public bool IsActive { get; set; } = true;
        public System.DateTime CreatedDate { get; set; } = System.DateTime.Now;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}