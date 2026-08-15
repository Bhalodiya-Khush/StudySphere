using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudySphere.Models.Dashboard;

namespace StudySphere.Controllers.Dashboard
{
    //[Authorize(Roles = "Student")]
    public class StudentDashboardController : Controller
    {
        public IActionResult Index()
        {
            var model = new StudentDashboardViewModel
            {
                //StudentName = "Student",
                //EnrolledCourses = 4,
                //CompletedCourses = 2,
                //OverallProgress = 68,
                //LearningHours = 12
            };

            return View("~/Views/Dashboard/Student/Index.cshtml", model);
        }
    }
}