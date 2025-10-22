using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InstituteManagement.Views;

namespace InstituteManagement.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object _currentView;

        [ObservableProperty]
        private string _currentDateTime;

        public MainViewModel()
        {
            // Set initial view to Dashboard
            CurrentView = new DashboardView();
            UpdateDateTime();

            // Update time every second
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => UpdateDateTime();
            timer.Start();
        }

        private void UpdateDateTime()
        {
            CurrentDateTime = DateTime.Now.ToString("dddd, MMMM dd, yyyy hh:mm:ss tt");
        }

        [RelayCommand]
        private void Navigate(string viewName)
        {
            CurrentView = viewName switch
            {
                "Dashboard" => new DashboardView(),
                "Students" => new StudentsView(),
                "Courses" => new CoursesView(),
                "Payments" => new PaymentsView(),
                "Attendance" => new AttendanceView(),
                "Reports" => new ReportsView(),
                _ => new DashboardView()
            };
        }
    }
}