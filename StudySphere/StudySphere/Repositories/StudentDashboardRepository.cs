using Microsoft.EntityFrameworkCore;
using StudySphere.Data;
using StudySphere.Models;
using StudySphere.Repositories.Interfaces;

namespace StudySphere.Repositories
{
    public class StudentDashboardRepository : IStudentDashboardRepository
    {
        private readonly StudySphereDbContext _dbContext;

        public StudentDashboardRepository(StudySphereDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Course> GetAllCourses()
        {
            return _dbContext.Courses
                .AsNoTracking()
                .Where(course => course.Status == "Approved")
                .OrderByDescending(course => course.CreatedAt)
                .ToList();
        }

        public List<Course> SearchCourses(
            string? keyword,
            string? category,
            string? level,
            decimal? minimumPrice,
            decimal? maximumPrice)
        {
            var courses = _dbContext.Courses
                .AsNoTracking()
                .Include(course => course.Instructor)
                    .ThenInclude(instructor => instructor.User)
                .Where(course => course.Status == "Approved");

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = keyword.Trim().ToLower();
                courses = courses.Where(course =>
                    course.Title.ToLower().Contains(normalizedKeyword) ||
                    course.Description.ToLower().Contains(normalizedKeyword));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                var normalizedCategory = category.Trim().ToLower();
                courses = courses.Where(course =>
                    course.Category != null &&
                    course.Category.ToLower() == normalizedCategory);
            }

            if (!string.IsNullOrWhiteSpace(level))
            {
                var normalizedLevel = level.Trim().ToLower();
                courses = courses.Where(course =>
                    course.Level != null &&
                    course.Level.ToLower() == normalizedLevel);
            }

            if (minimumPrice.HasValue)
            {
                courses = courses.Where(course =>
                    course.Price >= minimumPrice.Value);
            }

            if (maximumPrice.HasValue)
            {
                courses = courses.Where(course =>
                    course.Price <= maximumPrice.Value);
            }

            return courses
                .OrderByDescending(course => course.CreatedAt)
                .ToList();
        }

        public List<Course> GetEnrolledCourses(int studentId)
        {
            return _dbContext.Enrollments
                .AsNoTracking()
                .Where(enrollment =>
                    enrollment.StudentId == studentId &&
                    enrollment.Status != "Withdrawn")
                .Select(enrollment => enrollment.Course)
                .Where(course => course.Status == "Approved")
                .OrderByDescending(course => course.CreatedAt)
                .ToList();
        }

        public List<string> GetCategories()
        {
            return _dbContext.Courses
                .AsNoTracking()
                .Where(course =>
                    course.Status == "Approved" &&
                    course.Category != null)
                .Select(course => course.Category!)
                .Distinct()
                .OrderBy(category => category)
                .ToList();
        }

        public Course? GetCourseById(int id)
        {
            return _dbContext.Courses
                .AsNoTracking()
                .Include(course => course.Instructor)
                    .ThenInclude(instructor => instructor.User)
                .FirstOrDefault(course =>
                    course.CourseId == id &&
                    course.Status == "Approved");
        }

        public List<LiveLecture> GetLiveLectures(int courseId)
        {
            return _dbContext.LiveLectures
                .AsNoTracking()
                .Where(lecture =>
                    lecture.CourseId == courseId &&
                    lecture.Status != "Cancelled")
                .OrderBy(lecture => lecture.StartTime)
                .ToList();
        }

        public List<Material> GetPublishedMaterials(int courseId)
        {
            return _dbContext.Materials
                .AsNoTracking()
                .Where(material =>
                    material.CourseId == courseId &&
                    material.IsPublished)
                .OrderBy(material => material.DisplayOrder)
                .ThenBy(material => material.MaterialId)
                .ToList();
        }

        public Material? GetMaterialById(int materialId)
        {
            return _dbContext.Materials
                .AsNoTracking()
                .FirstOrDefault(material => material.MaterialId == materialId);
        }

        public List<Course> GetCoursesByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return new List<Course>();
            }

            var normalizedCategory = category.Trim();
            return _dbContext.Courses
                .AsNoTracking()
                .Where(course =>
                    course.Status == "Approved" &&
                    course.Category != null &&
                    course.Category.ToLower() == normalizedCategory.ToLower())
                .OrderByDescending(course => course.CreatedAt)
                .ToList();
        }

        public Student? GetStudentByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var normalizedEmail = email.Trim().ToLowerInvariant();
            return _dbContext.Students
                .Include(student => student.User)
                .FirstOrDefault(student =>
                    student.User.Email.ToLower() == normalizedEmail);
        }

        public bool IsEnrolled(int studentId, int courseId)
        {
            return _dbContext.Enrollments.Any(enrollment =>
                enrollment.StudentId == studentId &&
                enrollment.CourseId == courseId &&
                enrollment.Status != "Withdrawn" &&
                enrollment.Course.Status == "Approved");
        }

        public bool TryEnroll(int studentId, int courseId)
        {
            if (IsEnrolled(studentId, courseId) ||
                !_dbContext.Courses.Any(course =>
                    course.CourseId == courseId &&
                    course.Status == "Approved"))
            {
                return false;
            }

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrolledAt = DateTime.UtcNow,
                Status = "Active",
                Progress = 0
            };
            _dbContext.Enrollments.Add(enrollment);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException)
            {
                _dbContext.Entry(enrollment).State = EntityState.Detached;
                if (IsEnrolled(studentId, courseId))
                {
                    return false;
                }

                throw;
            }

            return true;
        }
    }
}
