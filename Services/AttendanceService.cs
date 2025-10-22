using InstituteManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace InstituteManagement.Data
{
    public class AttendanceService
    {
        private readonly string _connectionString;

        public AttendanceService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["InstituteDB"].ConnectionString;
        }

        public List<AttendanceRecord> GetBatchAttendance(int batchId, DateTime date)
        {
            var records = new List<AttendanceRecord>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                @"SELECT e.EnrollmentId, s.StudentId, s.StudentCode, s.FullNameEnglish, 
                         a.Status, a.Remarks
                  FROM Enrollments e
                  INNER JOIN Students s ON e.StudentId = s.StudentId
                  LEFT JOIN Attendance a ON e.EnrollmentId = a.EnrollmentId AND a.AttendanceDate = @Date
                  WHERE e.BatchId = @BatchId AND e.Status = 'Active'
                  ORDER BY s.FullNameEnglish", connection))
            {
                command.Parameters.AddWithValue("@BatchId", batchId);
                command.Parameters.AddWithValue("@Date", date.Date);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        records.Add(new AttendanceRecord
                        {
                            EnrollmentId = reader.GetInt32("EnrollmentId"),
                            StudentId = reader.GetInt32("StudentId"),
                            StudentCode = reader.GetString("StudentCode"),
                            StudentName = reader.GetString("FullNameEnglish"),
                            Status = reader["Status"] as string ?? "P", // Default to Present
                            Remarks = reader["Remarks"] as string,
                            AttendanceDate = date
                        });
                    }
                }
            }
            return records;
        }

        public bool SaveAttendance(List<AttendanceRecord> records, DateTime date)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Delete existing attendance for the date
                        var deleteCommand = new SqlCommand(
                            "DELETE FROM Attendance WHERE EnrollmentId IN (" +
                            string.Join(",", records.Select(r => r.EnrollmentId)) +
                            ") AND AttendanceDate = @Date", connection, transaction);
                        deleteCommand.Parameters.AddWithValue("@Date", date.Date);
                        deleteCommand.ExecuteNonQuery();

                        // Insert new attendance records
                        foreach (var record in records)
                        {
                            var insertCommand = new SqlCommand(
                                @"INSERT INTO Attendance (EnrollmentId, AttendanceDate, Status, Remarks)
                                  VALUES (@EnrollmentId, @Date, @Status, @Remarks)",
                                connection, transaction);

                            insertCommand.Parameters.AddWithValue("@EnrollmentId", record.EnrollmentId);
                            insertCommand.Parameters.AddWithValue("@Date", date.Date);
                            insertCommand.Parameters.AddWithValue("@Status", record.Status);
                            insertCommand.Parameters.AddWithValue("@Remarks", (object)record.Remarks ?? DBNull.Value);

                            insertCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public AttendanceSummary GetAttendanceSummary(int enrollmentId, DateTime startDate, DateTime endDate)
        {
            var summary = new AttendanceSummary();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                @"SELECT 
                    COUNT(*) as TotalClasses,
                    SUM(CASE WHEN Status = 'P' THEN 1 ELSE 0 END) as Present,
                    SUM(CASE WHEN Status = 'A' THEN 1 ELSE 0 END) as Absent,
                    SUM(CASE WHEN Status = 'L' THEN 1 ELSE 0 END) as Late
                  FROM Attendance 
                  WHERE EnrollmentId = @EnrollmentId 
                  AND AttendanceDate BETWEEN @StartDate AND @EndDate", connection))
            {
                command.Parameters.AddWithValue("@EnrollmentId", enrollmentId);
                command.Parameters.AddWithValue("@StartDate", startDate.Date);
                command.Parameters.AddWithValue("@EndDate", endDate.Date);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        summary.TotalClasses = reader.GetInt32("TotalClasses");
                        summary.Present = reader.GetInt32("Present");
                        summary.Absent = reader.GetInt32("Absent");
                        summary.Late = reader.GetInt32("Late");
                        summary.AttendancePercentage = summary.TotalClasses > 0 ?
                            (summary.Present * 100.0 / summary.TotalClasses) : 0;
                    }
                }
            }
            return summary;
        }
    }
}

public class AttendanceRecord
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public string StudentCode { get; set; }
    public string StudentName { get; set; }
    public string Status { get; set; } // P, A, L
    public string Remarks { get; set; }
    public DateTime AttendanceDate { get; set; }
}

public class AttendanceSummary
{
    public int TotalClasses { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public double AttendancePercentage { get; set; }
}