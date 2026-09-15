using Microsoft.AspNetCore.Mvc;
using StudySphere.Models;

namespace StudySphere.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            // Static data for now
            ViewBag.TotalStudents = 120;
            ViewBag.TotalInstructors = 15;
            ViewBag.TotalCourses = 35;
            ViewBag.PendingCourses = 5;

            return View();
        }
    }
}