-- Create Database
CREATE DATABASE InstituteManagement;
GO

USE InstituteManagement;
GO

-- Tables
CREATE TABLE Courses (
    CourseId INT PRIMARY KEY IDENTITY(1,1),
    CourseName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    DurationWeeks INT NOT NULL,
    TotalHours INT NOT NULL,
    Fee DECIMAL(18,2) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);

CREATE TABLE Students (
    StudentId INT PRIMARY KEY IDENTITY(1,1),
    StudentCode NVARCHAR(20) UNIQUE NOT NULL,
    FullNameEnglish NVARCHAR(100) NOT NULL,
    FullNameMyanmar NVARCHAR(100),
    NRCNumber NVARCHAR(50),
    DateOfBirth DATE,
    PhoneNumber NVARCHAR(20),
    Email NVARCHAR(100),
    Address NVARCHAR(500),
    ParentName NVARCHAR(100),
    ParentPhone NVARCHAR(20),
    CreatedDate DATETIME DEFAULT GETDATE()
);

CREATE TABLE Batches (
    BatchId INT PRIMARY KEY IDENTITY(1,1),
    BatchName NVARCHAR(50) NOT NULL,
    CourseId INT FOREIGN KEY REFERENCES Courses(CourseId),
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    DaysOfWeek NVARCHAR(50), -- 'Mon,Wed,Fri' or 'Mon-Fri'
    MaxStudents INT DEFAULT 15,
    CurrentStudents INT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    StartDate DATE,
    EndDate DATE
);

CREATE TABLE Enrollments (
    EnrollmentId INT PRIMARY KEY IDENTITY(1,1),
    StudentId INT FOREIGN KEY REFERENCES Students(StudentId),
    BatchId INT FOREIGN KEY REFERENCES Batches(BatchId),
    EnrollmentDate DATE DEFAULT GETDATE(),
    CourseFee DECIMAL(18,2) NOT NULL,
    Discount DECIMAL(18,2) DEFAULT 0,
    TotalFee DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Active', -- Active, Completed, Dropped
    CONSTRAINT UK_StudentBatch UNIQUE(StudentId, BatchId)
);

CREATE TABLE Payments (
    PaymentId INT PRIMARY KEY IDENTITY(1,1),
    EnrollmentId INT FOREIGN KEY REFERENCES Enrollments(EnrollmentId),
    ReceiptNumber NVARCHAR(50) UNIQUE NOT NULL,
    PaymentDate DATE DEFAULT GETDATE(),
    Amount DECIMAL(18,2) NOT NULL,
    PaymentMethod NVARCHAR(20), -- Cash, KBZPay, WaveMoney, BankTransfer
    Remarks NVARCHAR(200),
    CreatedDate DATETIME DEFAULT GETDATE()
);

CREATE TABLE Attendance (
    AttendanceId INT PRIMARY KEY IDENTITY(1,1),
    EnrollmentId INT FOREIGN KEY REFERENCES Enrollments(EnrollmentId),
    AttendanceDate DATE NOT NULL,
    Status NVARCHAR(1) NOT NULL, -- P=Present, A=Absent, L=Late
    Remarks NVARCHAR(100),
    CONSTRAINT UK_EnrollmentDate UNIQUE(EnrollmentId, AttendanceDate)
);

CREATE TABLE Assessments (
    AssessmentId INT PRIMARY KEY IDENTITY(1,1),
    EnrollmentId INT FOREIGN KEY REFERENCES Enrollments(EnrollmentId),
    AssessmentType NVARCHAR(50), -- Homework, Quiz, MidTerm, Final
    AssessmentDate DATE,
    Score DECIMAL(5,2),
    TotalScore DECIMAL(5,2) DEFAULT 100,
    Remarks NVARCHAR(200)
);

CREATE TABLE Teachers (
    TeacherId INT PRIMARY KEY IDENTITY(1,1),
    TeacherCode NVARCHAR(20) UNIQUE NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20),
    Email NVARCHAR(100),
    Specialization NVARCHAR(100),
    Bio NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    JoinDate DATE DEFAULT GETDATE()
);

CREATE TABLE TeacherAssignments (
    AssignmentId INT PRIMARY KEY IDENTITY(1,1),
    TeacherId INT FOREIGN KEY REFERENCES Teachers(TeacherId),
    BatchId INT FOREIGN KEY REFERENCES Batches(BatchId),
    AssignmentDate DATE DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);
GO

-- Insert Sample Data
INSERT INTO Courses (CourseName, Description, DurationWeeks, TotalHours, Fee) VALUES
('General English Level 1', 'Beginner English course focusing on conversation and basic grammar', 8, 48, 200000),
('Computer Basic', 'Essential computer skills for workplace', 10, 40, 220000),
('Python Programming', 'Foundation in Python programming language', 12, 48, 280000),
('YLE Starters', 'English for young learners age 6-8', 12, 36, 130000);

INSERT INTO Batches (BatchName, CourseId, StartTime, EndTime, DaysOfWeek, MaxStudents) VALUES
('GE-L1-Morning', 1, '06:30', '08:00', 'Mon,Tue,Wed,Thu,Fri', 15),
('Comp-Basic-Morning', 2, '10:00', '11:30', 'Mon,Tue,Wed,Thu,Fri,Sat', 15),
('Python-Evening', 3, '13:30', '15:00', 'Mon,Wed,Fri', 12),
('YLE-Starters', 4, '13:30', '15:00', 'Thu,Fri', 10);
GO