using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using InstituteManagement.Models;

namespace InstituteManagement.Data
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["InstituteDB"].ConnectionString;
        }

        public List<Enrollment> GetPendingEnrollments()
        {
            var enrollments = new List<Enrollment>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                @"SELECT 
                    e.EnrollmentId, e.StudentId, e.BatchId, e.EnrollmentDate, 
                    e.CourseFee, e.Discount, e.TotalFee, e.Status,
                    s.StudentCode, s.FullNameEnglish,
                    c.CourseId, c.CourseName,
                    b.BatchName, b.StartTime, b.EndTime
                  FROM Enrollments e
                  INNER JOIN Students s ON e.StudentId = s.StudentId
                  INNER JOIN Batches b ON e.BatchId = b.BatchId
                  INNER JOIN Courses c ON b.CourseId = c.CourseId
                  WHERE e.Status = 'Active'
                  ORDER BY e.EnrollmentDate DESC", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var enrollment = new Enrollment
                        {
                            EnrollmentId = reader.GetInt32(reader.GetOrdinal("EnrollmentId")),
                            StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                            BatchId = reader.GetInt32(reader.GetOrdinal("BatchId")),
                            EnrollmentDate = reader.GetDateTime(reader.GetOrdinal("EnrollmentDate")),
                            CourseFee = reader.GetDecimal(reader.GetOrdinal("CourseFee")),
                            Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                            TotalFee = reader.GetDecimal(reader.GetOrdinal("TotalFee")),
                            Status = reader.GetString(reader.GetOrdinal("Status")),
                            Student = new Student
                            {
                                StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                StudentCode = reader.GetString(reader.GetOrdinal("StudentCode")),
                                FullNameEnglish = reader.GetString(reader.GetOrdinal("FullNameEnglish"))
                            },
                            Batch = new Batch
                            {
                                BatchId = reader.GetInt32(reader.GetOrdinal("BatchId")),
                                BatchName = reader.GetString(reader.GetOrdinal("BatchName")),
                                StartTime = TimeSpan.Parse(reader.GetString(reader.GetOrdinal("StartTime"))),
                                EndTime = TimeSpan.Parse(reader.GetString(reader.GetOrdinal("EndTime")))
                            },
                            Course = new Course
                            {
                                CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                                CourseName = reader.GetString(reader.GetOrdinal("CourseName"))
                            }
                        };

                        enrollments.Add(enrollment);
                    }
                }
            }
            return enrollments;
        }

        public List<Payment> GetRecentPayments(int count = 50)
        {
            var payments = new List<Payment>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                @"SELECT TOP (@Count) 
                    p.PaymentId, p.EnrollmentId, p.ReceiptNumber, p.PaymentDate, 
                    p.Amount, p.PaymentMethod, p.Remarks, p.CreatedDate,
                    s.StudentId, s.StudentCode, s.FullNameEnglish,
                    c.CourseId, c.CourseName,
                    b.BatchName
                  FROM Payments p
                  INNER JOIN Enrollments e ON p.EnrollmentId = e.EnrollmentId
                  INNER JOIN Students s ON e.StudentId = s.StudentId
                  INNER JOIN Batches b ON e.BatchId = b.BatchId
                  INNER JOIN Courses c ON b.CourseId = c.CourseId
                  ORDER BY p.PaymentDate DESC", connection))
            {
                command.Parameters.AddWithValue("@Count", count);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var payment = new Payment
                        {
                            PaymentId = reader.GetInt32(reader.GetOrdinal("PaymentId")),
                            EnrollmentId = reader.GetInt32(reader.GetOrdinal("EnrollmentId")),
                            ReceiptNumber = reader.GetString(reader.GetOrdinal("ReceiptNumber")),
                            PaymentDate = reader.GetDateTime(reader.GetOrdinal("PaymentDate")),
                            Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                            PaymentMethod = reader.IsDBNull(reader.GetOrdinal("PaymentMethod")) ?
                                "Cash" : reader.GetString(reader.GetOrdinal("PaymentMethod")),
                            Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ?
                                string.Empty : reader.GetString(reader.GetOrdinal("Remarks")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                            Student = new Student
                            {
                                StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                StudentCode = reader.GetString(reader.GetOrdinal("StudentCode")),
                                FullNameEnglish = reader.GetString(reader.GetOrdinal("FullNameEnglish"))
                            },
                            Course = new Course
                            {
                                CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                                CourseName = reader.GetString(reader.GetOrdinal("CourseName"))
                            }
                        };

                        payments.Add(payment);
                    }
                }
            }
            return payments;
        }

        public string AddPayment(Payment payment)
        {
            var receiptNumber = $"RCP-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                @"INSERT INTO Payments (EnrollmentId, ReceiptNumber, PaymentDate, Amount, PaymentMethod, Remarks, CreatedDate)
                  VALUES (@EnrollmentId, @ReceiptNumber, @PaymentDate, @Amount, @PaymentMethod, @Remarks, @CreatedDate)", connection))
            {
                command.Parameters.AddWithValue("@EnrollmentId", payment.EnrollmentId);
                command.Parameters.AddWithValue("@ReceiptNumber", receiptNumber);
                command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                command.Parameters.AddWithValue("@Amount", payment.Amount);
                command.Parameters.AddWithValue("@PaymentMethod", (object)payment.PaymentMethod ?? DBNull.Value);
                command.Parameters.AddWithValue("@Remarks", (object)payment.Remarks ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDate", payment.CreatedDate);

                connection.Open();
                command.ExecuteNonQuery();
            }
            return receiptNumber;
        }

        // Student Operations
        public List<Student> GetStudents()
        {
            var students = new List<Student>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                @"SELECT 
                    StudentId, StudentCode, FullNameEnglish, FullNameMyanmar, 
                    PhoneNumber, Email, NRCNumber, DateOfBirth, Address, 
                    ParentName, ParentPhone, CreatedDate
                  FROM Students 
                  ORDER BY CreatedDate DESC", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(new Student
                        {
                            StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                            StudentCode = reader.GetString(reader.GetOrdinal("StudentCode")),
                            FullNameEnglish = reader.GetString(reader.GetOrdinal("FullNameEnglish")),
                            FullNameMyanmar = reader.IsDBNull(reader.GetOrdinal("FullNameMyanmar")) ?
                                string.Empty : reader.GetString(reader.GetOrdinal("FullNameMyanmar")),
                            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ?
                                string.Empty : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                            Email = reader.IsDBNull(reader.GetOrdinal("Email")) ?
                                string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                            NRCNumber = reader.IsDBNull(reader.GetOrdinal("NRCNumber")) ?
                                string.Empty : reader.GetString(reader.GetOrdinal("NRCNumber")),
                            DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ?
                                (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                            Address = reader.IsDBNull(reader.GetOrdinal("Address")) ?
                                string.Empty : reader.GetString(reader.GetOrdinal("Address")),
                            ParentName = reader.IsDBNull(reader.GetOrdinal("ParentName")) ?
                                string.Empty : reader.GetString(reader.GetOrdinal("ParentName")),
                            ParentPhone = reader.IsDBNull(reader.GetOrdinal("ParentPhone")) ?
                                string.Empty : reader.GetString(reader.GetOrdinal("ParentPhone")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                        });
                    }
                }
            }
            return students;
        }

        public int AddStudent(Student student)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                @"INSERT INTO Students (StudentCode, FullNameEnglish, FullNameMyanmar, PhoneNumber, Email, NRCNumber, DateOfBirth, Address, ParentName, ParentPhone, CreatedDate)
                  VALUES (@Code, @NameEng, @NameMyanmar, @Phone, @Email, @NRC, @DOB, @Address, @ParentName, @ParentPhone, @CreatedDate);
                  SELECT SCOPE_IDENTITY();", connection))
            {
                command.Parameters.AddWithValue("@Code", GenerateStudentCode());
                command.Parameters.AddWithValue("@NameEng", student.FullNameEnglish);
                command.Parameters.AddWithValue("@NameMyanmar", (object)student.FullNameMyanmar ?? DBNull.Value);
                command.Parameters.AddWithValue("@Phone", (object)student.PhoneNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@Email", (object)student.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@NRC", (object)student.NRCNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@DOB", (object)student.DateOfBirth ?? DBNull.Value);
                command.Parameters.AddWithValue("@Address", (object)student.Address ?? DBNull.Value);
                command.Parameters.AddWithValue("@ParentName", (object)student.ParentName ?? DBNull.Value);
                command.Parameters.AddWithValue("@ParentPhone", (object)student.ParentPhone ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private string GenerateStudentCode()
        {
            return $"ST-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }

        // Enrollment Operations
        public int EnrollStudent(Enrollment enrollment)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                @"INSERT INTO Enrollments (StudentId, BatchId, EnrollmentDate, CourseFee, Discount, TotalFee, Status)
                  VALUES (@StudentId, @BatchId, @EnrollmentDate, @CourseFee, @Discount, @TotalFee, 'Active');
                  SELECT SCOPE_IDENTITY();", connection))
            {
                command.Parameters.AddWithValue("@StudentId", enrollment.StudentId);
                command.Parameters.AddWithValue("@BatchId", enrollment.BatchId);
                command.Parameters.AddWithValue("@EnrollmentDate", DateTime.Now);
                command.Parameters.AddWithValue("@CourseFee", enrollment.CourseFee);
                command.Parameters.AddWithValue("@Discount", enrollment.Discount);
                command.Parameters.AddWithValue("@TotalFee", enrollment.TotalFee);

                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        // Get Courses
        public List<Course> GetCourses()
        {
            var courses = new List<Course>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                @"SELECT CourseId, CourseName, Description, DurationWeeks, TotalHours, Fee, IsActive, CreatedDate
                  FROM Courses 
                  WHERE IsActive = 1
                  ORDER BY CourseName", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courses.Add(new Course
                        {
                            CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                            CourseName = reader.GetString(reader.GetOrdinal("CourseName")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ?
                                string.Empty : reader.GetString(reader.GetOrdinal("Description")),
                            DurationWeeks = reader.GetInt32(reader.GetOrdinal("DurationWeeks")),
                            TotalHours = reader.GetInt32(reader.GetOrdinal("TotalHours")),
                            Fee = reader.GetDecimal(reader.GetOrdinal("Fee")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                        });
                    }
                }
            }
            return courses;
        }

        // Get Batches
        public List<Batch> GetBatches()
        {
            var batches = new List<Batch>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                @"SELECT b.BatchId, b.BatchName, b.CourseId, b.StartTime, b.EndTime, 
                         b.DaysOfWeek, b.MaxStudents, b.CurrentStudents, b.IsActive,
                         c.CourseName
                  FROM Batches b
                  INNER JOIN Courses c ON b.CourseId = c.CourseId
                  WHERE b.IsActive = 1
                  ORDER BY b.StartTime", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        batches.Add(new Batch
                        {
                            BatchId = reader.GetInt32(reader.GetOrdinal("BatchId")),
                            BatchName = reader.GetString(reader.GetOrdinal("BatchName")),
                            CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                            StartTime = TimeSpan.Parse(reader.GetString(reader.GetOrdinal("StartTime"))),
                            EndTime = TimeSpan.Parse(reader.GetString(reader.GetOrdinal("EndTime"))),
                            DaysOfWeek = reader.IsDBNull(reader.GetOrdinal("DaysOfWeek")) ?
                                string.Empty : reader.GetString(reader.GetOrdinal("DaysOfWeek")),
                            MaxStudents = reader.GetInt32(reader.GetOrdinal("MaxStudents")),
                            CurrentStudents = reader.GetInt32(reader.GetOrdinal("CurrentStudents")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            Course = new Course
                            {
                                CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                                CourseName = reader.GetString(reader.GetOrdinal("CourseName"))
                            }
                        });
                    }
                }
            }
            return batches;
        }
    }
}