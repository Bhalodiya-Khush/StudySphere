using Microsoft.AspNetCore.Mvc;
using StudySphere.ViewModels.Authentication;

namespace StudySphere.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(AuthViewModel model)
        {
            // TEMPORARY DEBUG
            Console.WriteLine("LOGIN POST CALLED");
            Console.WriteLine($"Email: {model.Login?.Email}");
            Console.WriteLine($"Password: {model.Login?.Password}");

            if (model.Login?.Email == "student@gmail.com" &&
                model.Login.Password == "123456")
            {
                Console.WriteLine("STUDENT CREDENTIALS MATCHED");

                return RedirectToAction(
                    "Index",
                    "StudentDashboard");
            }

            if (model.Login?.Email == "instructor@gmail.com" &&
                model.Login.Password == "123456")
            {
                return RedirectToAction(
                    "Index",
                    "InstructorDashboard");
            }

            if (model.Login?.Email == "admin@gmail.com" &&
                model.Login.Password == "123456")
            {
                return RedirectToAction(
                    "Index",
                    "AdminDashboard");
            }

            ViewBag.ErrorMessage = "Invalid email or password.";

            return View(model);
        }
    }
}