using StudySphere.Models;
using StudySphere.Repositories.Interfaces;

namespace StudySphere.Repositories
{
    public class StudentDashboardRepository : IStudentDashboardRepository
    {
        // =========================================================
        // STATIC COURSE DATA
        // =========================================================

        private readonly List<Course> _courses = new()
        {
            new Course
            {
                CourseId = 1,
                Title = "C# Programming",
                Description = "Learn C# programming from basics to advanced concepts.",
                ThumbnailUrl = "/images/courses/csharp.jpg",
                Category = "Development"
            },

            new Course
            {
                CourseId = 2,
                Title = "ASP.NET Core MVC",
                Description = "Build modern web applications using ASP.NET Core MVC.",
                ThumbnailUrl = "/images/courses/aspnet.jpg",
                Category = "Development"
            },

            new Course
            {
                CourseId = 3,
                Title = "Python for Beginners",
                Description = "Learn Python programming with practical examples.",
                ThumbnailUrl = "/images/courses/python.jpg",
                Category = "Development"
            },

            new Course
            {
                CourseId = 4,
                Title = "Artificial Intelligence",
                Description = "Understand the fundamentals of Artificial Intelligence.",
                ThumbnailUrl = "/images/courses/ai.jpg",
                Category = "AI"
            },

            new Course
            {
                CourseId = 5,
                Title = "Machine Learning",
                Description = "Learn machine learning algorithms and applications.",
                ThumbnailUrl = "/images/courses/ml.jpg",
                Category = "AI"
            },

            new Course
            {
                CourseId = 6,
                Title = "Project Management",
                Description = "Learn how to manage projects effectively.",
                ThumbnailUrl = "/images/courses/project-management.jpg",
                Category = "Management"
            },

            new Course
            {
                CourseId = 7,
                Title = "UI/UX Design",
                Description = "Learn the principles of modern UI and UX design.",
                ThumbnailUrl = "/images/courses/uiux.jpg",
                Category = "Design"
            }
        };


        // =========================================================
        // STATIC ENROLLED COURSE IDs
        // =========================================================

        private readonly List<int> _enrolledCourseIds = new()
        {
            1,
            2,
            5
        };


        // =========================================================
        // GET ALL COURSES
        // =========================================================

        public List<Course> GetAllCourses()
        {
            return _courses;
        }


        // =========================================================
        // GET ENROLLED COURSES
        // =========================================================

        public List<Course> GetEnrolledCourses()
        {
            return _courses
                .Where(course =>
                    _enrolledCourseIds.Contains(course.CourseId))
                .ToList();
        }


        // =========================================================
        // GET CATEGORIES
        // =========================================================

        public List<string> GetCategories()
        {
            return _courses
                .Select(course => course.Category)
                .Distinct()
                .ToList();
        }


        // =========================================================
        // GET COURSE BY ID
        // =========================================================

        public Course? GetCourseById(int id)
        {
            return _courses
                .FirstOrDefault(course => course.CourseId == id);
        }


        // =========================================================
        // GET COURSES BY CATEGORY
        // =========================================================

        public List<Course> GetCoursesByCategory(string category)
        {
            return _courses
                .Where(course =>
                    course.Category.Equals(
                        category,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}