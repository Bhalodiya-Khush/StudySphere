using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudySphere.Models.Dashboard;

//[Authorize(Roles = "Instructor")]
public class InstructorDashboardController : Controller
{
    public IActionResult Index()
    {
        //var model = new InstructorDashboardViewModel
        //{
        //    InstructorName = "Instructor",
        //    TotalCourses = 5,
        //    TotalStudents = 120,
        //    PendingAssignments = 14,
        //    AverageCourseRating = 4.6
        //};

        return View("~/Views/Dashboard/InstructorDashboard/Index.cshtml"/*,model*/);
    }
}