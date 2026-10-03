using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySphere.Data;
using StudySphere.Models.Dashboard;
using StudySphere.Models.ViewModels;
using System.Diagnostics;

namespace StudySphere.Controllers
{
    public class HomeController : Controller
    {
        private readonly StudySphereDbContext _dbContext;

        public HomeController(StudySphereDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "AdminDashboard");
                }

                if (User.IsInRole("Instructor"))
                {
                    return RedirectToAction("Index", "InstructorDashboard");
                }

                if (User.IsInRole("Student"))
                {
                    return RedirectToAction("Index", "StudentDashboard");
                }
            }

            var courses = _dbContext.Courses
                .AsNoTracking()
                .Where(course => course.Status == "Approved")
                .OrderByDescending(course => course.CreatedAt)
                .Take(6)
                .ToList();

            var categories = _dbContext.Courses
                .AsNoTracking()
                .Where(course =>
                    course.Status == "Approved" &&
                    course.Category != null &&
                    course.Category != "")
                .GroupBy(course => course.Category!)
                .Select(group => new CourseCategoryViewModel
                {
                    Name = group.Key,
                    CourseCount = group.Count()
                })
                .OrderBy(category => category.Name)
                .ToList();

            return View(new HomeViewModel
            {
                CourseCount = _dbContext.Courses.Count(course =>
                    course.Status == "Approved"),
                InstructorCount = _dbContext.Instructors.Count(instructor =>
                    instructor.User.IsActive),
                StudentCount = _dbContext.Students.Count(student =>
                    student.User.IsActive),
                Courses = courses,
                Categories = categories
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new global::StudySphere.Models.Dashboard.StudentDashboardViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
