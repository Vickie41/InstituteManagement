using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Configuration;

namespace InstituteManagement.Services
{
    public class ReportService
    {
        private readonly string _connectionString;

        public ReportService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["InstituteDB"].ConnectionString;
        }

        public List<ReportData> GetRevenueReport(DateTime startDate, DateTime endDate)
        {
            var reportData = new List<ReportData>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                @"SELECT 
                    c.CourseName,
                    COUNT(DISTINCT e.EnrollmentId) as EnrollmentCount,
                    SUM(p.Amount) as TotalRevenue,
                    AVG(p.Amount) as AveragePayment
                  FROM Payments p
                  INNER JOIN Enrollments e ON p.EnrollmentId = e.EnrollmentId
                  INNER JOIN Batches b ON e.BatchId = b.BatchId
                  INNER JOIN Courses c ON b.CourseId = c.CourseId
                  WHERE p.PaymentDate BETWEEN @StartDate AND @EndDate
                  GROUP BY c.CourseName
                  ORDER BY TotalRevenue DESC", connection))
            {
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reportData.Add(new ReportData
                        {
                            Category = "Revenue",
                            Description = reader.GetString("CourseName"),
                            Count = reader.GetInt32("EnrollmentCount"),
                            Amount = reader.GetDecimal("TotalRevenue"),
                            Percentage = (double)reader.GetDecimal("AveragePayment"),
                            Date = DateTime.Now
                        });
                    }
                }
            }
            return reportData;
        }

        public List<ReportData> GetAttendanceReport(DateTime startDate, DateTime endDate)
        {
            var reportData = new List<ReportData>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                @"SELECT 
                    c.CourseName,
                    b.BatchName,
                    COUNT(DISTINCT e.EnrollmentId) as TotalStudents,
                    AVG(CASE WHEN a.Status = 'P' THEN 1.0 ELSE 0.0 END) * 100 as AttendanceRate
                  FROM Enrollments e
                  INNER JOIN Batches b ON e.BatchId = b.BatchId
                  INNER JOIN Courses c ON b.CourseId = c.CourseId
                  LEFT JOIN Attendance a ON e.EnrollmentId = a.EnrollmentId 
                    AND a.AttendanceDate BETWEEN @StartDate AND @EndDate
                  WHERE e.Status = 'Active'
                  GROUP BY c.CourseName, b.BatchName
                  ORDER BY c.CourseName, b.BatchName", connection))
            {
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reportData.Add(new ReportData
                        {
                            Category = "Attendance",
                            Description = $"{reader.GetString("CourseName")} - {reader.GetString("BatchName")}",
                            Count = reader.GetInt32("TotalStudents"),
                            Percentage = reader.GetDouble("AttendanceRate"),
                            Date = DateTime.Now
                        });
                    }
                }
            }
            return reportData;
        }

        public List<ReportData> GetStudentProgressReport(DateTime startDate, DateTime endDate)
        {
            var reportData = new List<ReportData>();
            // Implementation for student progress report
            // Add your logic here based on your assessment data
            return reportData;
        }

        public void ExportToExcel(List<ReportData> data, string reportType, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Simple CSV export instead of Excel Interop
                var csvContent = new System.Text.StringBuilder();
                csvContent.AppendLine($"Hlaing Tharyar Skills Institute - {reportType} Report");
                csvContent.AppendLine($"Period: {startDate:dd-MMM-yyyy} to {endDate:dd-MMM-yyyy}");
                csvContent.AppendLine($"Generated on: {DateTime.Now:dd-MMM-yyyy HH:mm}");
                csvContent.AppendLine();
                csvContent.AppendLine("Category,Description,Count,Amount,Percentage,Date");

                foreach (var item in data)
                {
                    csvContent.AppendLine($"\"{item.Category}\",\"{item.Description}\",{item.Count},{item.Amount},{item.Percentage:F2},\"{item.Date:yyyy-MM-dd}\"");
                }

                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = $"{reportType}_Report_{DateTime.Now:yyyyMMdd_HHmm}.csv",
                    Filter = "CSV Files (*.csv)|*.csv"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveDialog.FileName, csvContent.ToString());
                    MessageBox.Show($"Report exported successfully to:\n{saveDialog.FileName}",
                                  "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting report: {ex.Message}", "Export Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}