using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudySphere.Models.Dashboard;
//[Authorize(Roles = "Admin")]
public class AdminDashboardController : Controller
{
    public IActionResult Index()
    {
        var model = new Users
        {
            //TotalUsers = 850,
            //TotalStudents = 720,
            //TotalInstructors = 80,
            //TotalCourses = 65,
            //TotalCategories = 10
        };

        return View("~/Views/Dashboard/AdminDashboard/Index.cshtml", model);
    }
}