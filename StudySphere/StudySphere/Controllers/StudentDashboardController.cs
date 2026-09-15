using Microsoft.AspNetCore.Mvc;
using StudySphere.Models;
using StudySphere.Models.ViewModels;

namespace StudySphere.Controllers
{
    public class StudentDashboardController : Controller
    {
        public IActionResult Index()
        {
            var allCourses = GetAllCourses();

            var enrolledCourses = GetEnrolledCourses();

            var model = new StudentDashboardViewModel
            {
                StudentName = "Student",

                EnrolledCourses = enrolledCourses,

                AllCourses = allCourses,

                Categories = new List<string>
                {
                    "Development",
                    "AI",
                    "Management",
                    "Design"
                }
            };

            return View(model);
        }
        private List<Course> GetAllCourses()
        {
            return new List<Course>
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
        }
        private List<Course> GetEnrolledCourses()
        {
            var allCourses = GetAllCourses();

            return new List<Course>
            {
                allCourses[0],
                allCourses[1],
                allCourses[4]
            };
        }
        public IActionResult MyCourses()
        {
            var enrolledCourses = GetEnrolledCourses();

            return View(enrolledCourses);
        }

        public IActionResult AllCourses()
        {
            var allCourses = GetAllCourses();

            return View(allCourses);
        }

        public IActionResult CategoryCourses(string category)
        {
            var allCourses = GetAllCourses();

            var courses = allCourses
                .Where(c => c.Category.Equals(
                    category,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            ViewBag.Category = category;

            return View(courses);
        }

        public IActionResult CourseDetails(int id)
        {
            var allCourses = GetAllCourses();

            var course = allCourses.FirstOrDefault(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }
    }
}