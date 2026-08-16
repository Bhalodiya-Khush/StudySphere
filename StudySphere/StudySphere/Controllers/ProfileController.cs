using Microsoft.AspNetCore.Mvc;

namespace StudySphere.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
