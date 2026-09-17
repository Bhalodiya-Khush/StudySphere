using StudySphere.Models;
using StudySphere.Repositories.Interfaces;

namespace StudySphere.Repositories
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        // =========================================================
        // STATIC STUDENT DATA
        // =========================================================

        private static readonly List<Student> StudentsList = new()
        {
            new Student
            {
                StudentId = 1
            },

            new Student
            {
                StudentId = 2
            },

            new Student
            {
                StudentId = 3
            },

            new Student
            {
                StudentId = 4
            },

            new Student
            {
                StudentId = 5
            }
        };


        // =========================================================
        // STATIC INSTRUCTOR DATA
        // =========================================================

        private static readonly List<Instructor> InstructorsList = new()
        {
            new Instructor
            {
                InstructorId = 1,
                ProfessionalTitle = "Senior Software Developer",
                AreaOfExpertise = "Web Development",
                Qualification = "B.Tech Computer Engineering",
                Bio = "Experienced instructor in web and backend development."
            },

            new Instructor
            {
                InstructorId = 2,
                ProfessionalTitle = "Java Developer",
                AreaOfExpertise = "Java and Spring Boot",
                Qualification = "M.Tech Computer Science",
                Bio = "Instructor specializing in Java backend development."
            },

            new Instructor
            {
                InstructorId = 3,
                ProfessionalTitle = "Database Specialist",
                AreaOfExpertise = "Database Management",
                Qualification = "M.Tech Information Technology",
                Bio = "Instructor specializing in SQL and database systems."
            }
        };


        // =========================================================
        // STATIC COURSE DATA
        // =========================================================

        private static readonly List<Course> CoursesList = new()
        {
            // -----------------------------------------------------
            // APPROVED COURSE
            // -----------------------------------------------------

            new Course
            {
                CourseId = 1,
                Title = "ASP.NET Core MVC",
                Description =
                    "Learn ASP.NET Core MVC from fundamentals to advanced concepts.",
                Category = "Web Development",
                Level = "Intermediate",
                ThumbnailUrl = "/images/courses/aspnet.jpg",
                Price = 1499,
                Status = "Approved",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                InstructorId = 1
            },


            // -----------------------------------------------------
            // APPROVED COURSE
            // -----------------------------------------------------

            new Course
            {
                CourseId = 2,
                Title = "Java Programming",
                Description =
                    "Learn Java programming, OOP concepts and application development.",
                Category = "Programming",
                Level = "Beginner",
                ThumbnailUrl = "/images/courses/java.jpg",
                Price = 999,
                Status = "Approved",
                CreatedAt = DateTime.UtcNow.AddDays(-18),
                InstructorId = 2
            },


            // -----------------------------------------------------
            // PENDING COURSE
            // -----------------------------------------------------

            new Course
            {
                CourseId = 3,
                Title = "Complete C# Programming",
                Description =
                    "Learn C# programming from basic syntax to object oriented programming.",
                Category = "Programming",
                Level = "Beginner",
                ThumbnailUrl = "/images/courses/csharp.jpg",
                Price = 799,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                InstructorId = 1
            },


            // -----------------------------------------------------
            // PENDING COURSE
            // -----------------------------------------------------

            new Course
            {
                CourseId = 4,
                Title = "Database Management System",
                Description =
                    "Learn database concepts, SQL, normalization and database design.",
                Category = "Database",
                Level = "Intermediate",
                ThumbnailUrl = "/images/courses/database.jpg",
                Price = 1199,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                InstructorId = 3
            },


            // -----------------------------------------------------
            // REJECTED COURSE
            // -----------------------------------------------------

            new Course
            {
                CourseId = 5,
                Title = "Basic Web Design",
                Description =
                    "Learn HTML and CSS fundamentals for creating websites.",
                Category = "Web Development",
                Level = "Beginner",
                ThumbnailUrl = "/images/courses/webdesign.jpg",
                Price = 599,
                Status = "Rejected",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                InstructorId = 2
            }
        };


        // =========================================================
        // STATIC ENROLLMENT DATA
        // =========================================================

        private static readonly List<Enrollment> EnrollmentsList = new()
        {
            new Enrollment
            {
                EnrollmentId = 1,
                StudentId = 1,
                CourseId = 1,
                EnrolledAt = DateTime.UtcNow.AddDays(-20),
                Status = "Active",
                Progress = 75
            },

            new Enrollment
            {
                EnrollmentId = 2,
                StudentId = 2,
                CourseId = 1,
                EnrolledAt = DateTime.UtcNow.AddDays(-15),
                Status = "Active",
                Progress = 50
            },

            new Enrollment
            {
                EnrollmentId = 3,
                StudentId = 3,
                CourseId = 2,
                EnrolledAt = DateTime.UtcNow.AddDays(-10),
                Status = "Completed",
                Progress = 100,
                CompletedAt = DateTime.UtcNow.AddDays(-2)
            },

            new Enrollment
            {
                EnrollmentId = 4,
                StudentId = 4,
                CourseId = 1,
                EnrolledAt = DateTime.UtcNow.AddDays(-8),
                Status = "Active",
                Progress = 40
            }
        };


        // =========================================================
        // STUDENT METHODS
        // =========================================================

        public List<Student> GetAllStudents()
        {
            return StudentsList;
        }


        public Student? GetStudentById(int id)
        {
            return StudentsList
                .FirstOrDefault(s => s.StudentId == id);
        }


        // =========================================================
        // INSTRUCTOR METHODS
        // =========================================================

        public List<Instructor> GetAllInstructors()
        {
            return InstructorsList;
        }


        public Instructor? GetInstructorById(int id)
        {
            return InstructorsList
                .FirstOrDefault(i => i.InstructorId == id);
        }


        // =========================================================
        // COURSE METHODS
        // =========================================================

        public List<Course> GetAllCourses()
        {
            return CoursesList
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }


        public Course? GetCourseById(int id)
        {
            return CoursesList
                .FirstOrDefault(c => c.CourseId == id);
        }


        public List<Course> GetPendingCourses()
        {
            return CoursesList
                .Where(c => c.Status == "Pending")
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }


        public List<Course> GetApprovedCourses()
        {
            return CoursesList
                .Where(c => c.Status == "Approved")
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }


        public List<Course> GetRejectedCourses()
        {
            return CoursesList
                .Where(c => c.Status == "Rejected")
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }


        public List<Course> GetCoursesByInstructorId(int instructorId)
        {
            return CoursesList
                .Where(c => c.InstructorId == instructorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }


        // =========================================================
        // ENROLLMENT METHODS
        // =========================================================

        public List<Enrollment> GetAllEnrollments()
        {
            return EnrollmentsList;
        }


        public List<Enrollment> GetEnrollmentsByStudentId(int studentId)
        {
            return EnrollmentsList
                .Where(e => e.StudentId == studentId)
                .ToList();
        }


        public List<Enrollment> GetEnrollmentsByCourseId(int courseId)
        {
            return EnrollmentsList
                .Where(e => e.CourseId == courseId)
                .ToList();
        }


        // =========================================================
        // COURSE APPROVAL METHODS
        // =========================================================

        public bool ApproveCourse(int id)
        {
            var course = CoursesList
                .FirstOrDefault(c => c.CourseId == id);

            if (course == null)
            {
                return false;
            }

            course.Status = "Approved";
            course.UpdatedAt = DateTime.UtcNow;

            return true;
        }


        public bool RejectCourse(int id)
        {
            var course = CoursesList
                .FirstOrDefault(c => c.CourseId == id);

            if (course == null)
            {
                return false;
            }

            course.Status = "Rejected";
            course.UpdatedAt = DateTime.UtcNow;

            return true;
        }
    }
}