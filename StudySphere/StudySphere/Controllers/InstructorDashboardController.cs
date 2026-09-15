using Microsoft.AspNetCore.Mvc;
using StudySphere.Models;

namespace StudySphere.Controllers
{
    public class InstructorDashboardController : Controller
    {
        public IActionResult Index()
        {
            // Static instructor information
            var instructor = new Instructor
            {
                InstructorId = 1,
                ProfessionalTitle = "Senior Software Developer",
                AreaOfExpertise = "Web Development",
                Qualification = "B.Tech Computer Engineering",
                Bio = "Experienced instructor in web and backend development."
            };

            // Static courses created by this instructor
            var courses = new List<Course>
            {
                new Course
                {
                    CourseId = 1,
                    Title = "ASP.NET Core MVC",
                    Description = "Learn ASP.NET Core MVC from basic to advanced concepts.",
                    Category = "Web Development",
                    Level = "Intermediate",
                    ThumbnailUrl = "/images/courses/aspnet.jpg",
                    Price = 999,
                    Status = "Published"
                },

                new Course
                {
                    CourseId = 2,
                    Title = "Java Spring Boot",
                    Description = "Learn backend development using Java and Spring Boot.",
                    Category = "Backend Development",
                    Level = "Intermediate",
                    ThumbnailUrl = "/images/courses/java.jpg",
                    Price = 1199,
                    Status = "Published"
                },

                new Course
                {
                    CourseId = 3,
                    Title = "Database Management",
                    Description = "Learn SQL and database management concepts.",
                    Category = "Database",
                    Level = "Beginner",
                    ThumbnailUrl = "/images/courses/database.jpg",
                    Price = 799,
                    Status = "Published"
                },

                new Course
                {
                    CourseId = 4,
                    Title = "Web Development",
                    Description = "Learn HTML, CSS and JavaScript.",
                    Category = "Web Development",
                    Level = "Beginner",
                    ThumbnailUrl = "/images/courses/web.jpg",
                    Price = 699,
                    Status = "Published"
                }
            };

            // Static dashboard values for now
            ViewBag.Instructor = instructor;
            ViewBag.Courses = courses;

            ViewBag.TotalCourses = courses.Count;
            ViewBag.TotalStudents = 48;
            ViewBag.TotalMaterials = 25;
            ViewBag.UpcomingLectures = 2;

            return View();
        }
    }
}