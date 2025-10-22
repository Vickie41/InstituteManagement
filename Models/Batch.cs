using System;
using System.ComponentModel;

namespace InstituteManagement.Models
{
    public class Batch : INotifyPropertyChanged
    {
    
            public int BatchId { get; set; }
            public string BatchName { get; set; }
            public int CourseId { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public string DaysOfWeek { get; set; }
            public int MaxStudents { get; set; }
            public int CurrentStudents { get; set; }
            public bool IsActive { get; set; }
            
        

        // Navigation property
        public Course Course { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}