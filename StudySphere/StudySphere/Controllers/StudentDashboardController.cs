using Microsoft.AspNetCore.Mvc;
using StudySphere.Models.ViewModels;
using StudySphere.Repositories.Interfaces;

namespace StudySphere.Controllers
{
    public class StudentDashboardController : Controller
    {
        private readonly IStudentDashboardRepository _repository;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public StudentDashboardController(
            IStudentDashboardRepository repository)
        {
            _repository = repository;
        }


        // =========================================================
        // STUDENT DASHBOARD
        // =========================================================

        public IActionResult Index()
        {
            var model = new StudentDashboardViewModel
            {
                StudentName = "Student",

                EnrolledCourses =
                    _repository.GetEnrolledCourses(),

                AllCourses =
                    _repository.GetAllCourses(),

                Categories =
                    _repository.GetCategories()
            };

            return View(model);
        }


        // =========================================================
        // MY COURSES
        // =========================================================

        public IActionResult MyCourses()
        {
            var enrolledCourses =
                _repository.GetEnrolledCourses();

            return View(enrolledCourses);
        }


        // =========================================================
        // ALL COURSES
        // =========================================================

        public IActionResult AllCourses()
        {
            var allCourses =
                _repository.GetAllCourses();

            return View(allCourses);
        }


        // =========================================================
        // COURSES BY CATEGORY
        // =========================================================

        public IActionResult CategoryCourses(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return RedirectToAction(nameof(Index));
            }

            var courses =
                _repository.GetCoursesByCategory(category);

            ViewBag.Category = category;

            return View(courses);
        }


        // =========================================================
        // COURSE DETAILS
        // =========================================================

        public IActionResult CourseDetails(int id)
        {
            var course =
                _repository.GetCourseById(id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }
    }
}