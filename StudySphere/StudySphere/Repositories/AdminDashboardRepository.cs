using Microsoft.EntityFrameworkCore;
using StudySphere.Data;
using StudySphere.Models;
using StudySphere.Repositories.Interfaces;

namespace StudySphere.Repositories
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly StudySphereDbContext _dbContext;

        public AdminDashboardRepository(StudySphereDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Student> GetAllStudents()
        {
            return _dbContext.Students
                .AsNoTracking()
                .Include(student => student.User)
                .OrderBy(student => student.User.FullName)
                .ToList();
        }

        public Student? GetStudentById(int id)
        {
            return _dbContext.Students
                .AsNoTracking()
                .Include(student => student.User)
                .FirstOrDefault(student => student.StudentId == id);
        }

        public List<Instructor> GetAllInstructors()
        {
            return _dbContext.Instructors
                .AsNoTracking()
                .Include(instructor => instructor.User)
                .OrderBy(instructor => instructor.User.FullName)
                .ToList();
        }

        public Instructor? GetInstructorById(int id)
        {
            return _dbContext.Instructors
                .AsNoTracking()
                .Include(instructor => instructor.User)
                .FirstOrDefault(instructor => instructor.InstructorId == id);
        }

        public List<Course> GetAllCourses()
        {
            return _dbContext.Courses
                .AsNoTracking()
                .Include(course => course.Instructor)
                    .ThenInclude(instructor => instructor.User)
                .OrderByDescending(course => course.CreatedAt)
                .ToList();
        }

        public Course? GetCourseById(int id)
        {
            return _dbContext.Courses
                .AsNoTracking()
                .Include(course => course.Instructor)
                    .ThenInclude(instructor => instructor.User)
                .Include(course => course.Materials)
                .Include(course => course.LiveLectures)
                .Include(course => course.Enrollments)
                .FirstOrDefault(course => course.CourseId == id);
        }

        public List<Course> GetPendingCourses()
        {
            return GetCoursesByStatus("Pending");
        }

        public List<Course> GetApprovedCourses()
        {
            return GetCoursesByStatus("Approved");
        }

        public List<Course> GetRejectedCourses()
        {
            return GetCoursesByStatus("Rejected");
        }

        public List<Course> GetCoursesByInstructorId(int instructorId)
        {
            return _dbContext.Courses
                .AsNoTracking()
                .Where(course => course.InstructorId == instructorId)
                .OrderByDescending(course => course.CreatedAt)
                .ToList();
        }

        public List<Enrollment> GetAllEnrollments()
        {
            return _dbContext.Enrollments
                .AsNoTracking()
                .Include(enrollment => enrollment.Student)
                    .ThenInclude(student => student.User)
                .Include(enrollment => enrollment.Course)
                .OrderByDescending(enrollment => enrollment.EnrolledAt)
                .ToList();
        }

        public List<Enrollment> GetEnrollmentsByStudentId(int studentId)
        {
            return _dbContext.Enrollments
                .AsNoTracking()
                .Include(enrollment => enrollment.Course)
                .Where(enrollment => enrollment.StudentId == studentId)
                .OrderByDescending(enrollment => enrollment.EnrolledAt)
                .ToList();
        }

        public List<Enrollment> GetEnrollmentsByCourseId(int courseId)
        {
            return _dbContext.Enrollments
                .AsNoTracking()
                .Include(enrollment => enrollment.Student)
                    .ThenInclude(student => student.User)
                .Where(enrollment => enrollment.CourseId == courseId)
                .OrderByDescending(enrollment => enrollment.EnrolledAt)
                .ToList();
        }

        public bool ApproveCourse(int id)
        {
            return UpdateCourseStatus(id, "Approved");
        }

        public bool RejectCourse(int id)
        {
            return UpdateCourseStatus(id, "Rejected");
        }

        public bool SetCourseActive(int id, bool isActive)
        {
            var course = _dbContext.Courses.FirstOrDefault(item =>
                item.CourseId == id &&
                item.Status == (isActive ? "Deactivated" : "Approved"));
            if (course is null)
            {
                return false;
            }

            course.Status = isActive ? "Approved" : "Deactivated";
            course.UpdatedAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
            return true;
        }

        public bool SetStudentActive(int id, bool isActive)
        {
            var student = _dbContext.Students
                .Include(item => item.User)
                .FirstOrDefault(item => item.StudentId == id);
            if (student is null)
            {
                return false;
            }

            student.User.IsActive = isActive;
            _dbContext.SaveChanges();
            return true;
        }

        public bool SetInstructorActive(int id, bool isActive)
        {
            var instructor = _dbContext.Instructors
                .Include(item => item.User)
                .FirstOrDefault(item => item.InstructorId == id);
            if (instructor is null)
            {
                return false;
            }

            instructor.User.IsActive = isActive;
            _dbContext.SaveChanges();
            return true;
        }

        private List<Course> GetCoursesByStatus(string status)
        {
            return _dbContext.Courses
                .AsNoTracking()
                .Include(course => course.Instructor)
                    .ThenInclude(instructor => instructor.User)
                .Where(course => course.Status == status)
                .OrderByDescending(course => course.CreatedAt)
                .ToList();
        }

        private bool UpdateCourseStatus(int id, string status)
        {
            var course = _dbContext.Courses.FirstOrDefault(course =>
                course.CourseId == id &&
                course.Status == "Pending");
            if (course is null)
            {
                return false;
            }

            course.Status = status;
            course.UpdatedAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
            return true;
        }
    }
}
