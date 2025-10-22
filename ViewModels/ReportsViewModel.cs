using InstituteManagement.Common;
using InstituteManagement.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using InstituteManagement.Models;
using InstituteManagement.Services;
using InstituteManagement.Views;
using CommunityToolkit.Mvvm.Input; // Add this for RelayCommand

namespace InstituteManagement.ViewModels
{
    public class ReportsViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;
        private readonly ReportService _reportService;

        private ObservableCollection<ReportData> _reports;
        public ObservableCollection<ReportData> Reports
        {
            get => _reports;
            set { _reports = value; OnPropertyChanged(nameof(Reports)); }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(nameof(StartDate)); }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(nameof(EndDate)); }
        }

        private string _selectedReportType;
        public string SelectedReportType
        {
            get => _selectedReportType;
            set { _selectedReportType = value; OnPropertyChanged(nameof(SelectedReportType)); }
        }

        public RelayCommand GenerateReportCommand { get; }
        public RelayCommand ExportToExcelCommand { get; }
        public RelayCommand PrintReportCommand { get; }

        public ReportsViewModel()
        {
            _dbService = new DatabaseService();
            _reportService = new ReportService();
            Reports = new ObservableCollection<ReportData>();

            StartDate = DateTime.Today.AddMonths(-1);
            EndDate = DateTime.Today;

            GenerateReportCommand = new RelayCommand(GenerateReport);
            ExportToExcelCommand = new RelayCommand(ExportToExcel);
            PrintReportCommand = new RelayCommand(PrintReport);
        }

        private void GenerateReport()
        {
            try
            {
                Reports.Clear();

                switch (SelectedReportType)
                {
                    case "Revenue":
                        var revenueData = _reportService.GetRevenueReport(StartDate, EndDate);
                        foreach (var item in revenueData)
                        {
                            Reports.Add(item);
                        }
                        break;

                    case "Attendance":
                        var attendanceData = _reportService.GetAttendanceReport(StartDate, EndDate);
                        foreach (var item in attendanceData)
                        {
                            Reports.Add(item);
                        }
                        break;

                    case "StudentProgress":
                        var progressData = _reportService.GetStudentProgressReport(StartDate, EndDate);
                        foreach (var item in progressData)
                        {
                            Reports.Add(item);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToExcel()
        {
            if (Reports.Count == 0)
            {
                MessageBox.Show("No data to export.", "Information",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                // Convert ObservableCollection to List for the export method
                var reportList = new List<ReportData>(Reports);
                _reportService.ExportToExcel(reportList, SelectedReportType, StartDate, EndDate);
                MessageBox.Show("Report exported successfully!", "Success",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to Excel: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintReport()
        {
            if (Reports.Count == 0)
            {
                MessageBox.Show("No data to print.", "Information",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Convert to List<object> for the PrintPreviewWindow
            var dataList = new List<object>();
            foreach (var item in Reports)
            {
                dataList.Add(item);
            }

            var printWindow = new PrintPreviewWindow(dataList, SelectedReportType);
            printWindow.ShowDialog();
        }
    }
}

public class ReportData
{
    public string Category { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
    public DateTime Date { get; set; }
}