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
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Authentication logic later

            return RedirectToAction("Index", "Home");
        }
    }
}