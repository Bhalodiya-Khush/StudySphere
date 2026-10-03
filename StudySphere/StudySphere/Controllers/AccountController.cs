using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudySphere.Models;
using StudySphere.ViewModels.Authentication;

namespace StudySphere.Controllers
{
    public class AccountController : Controller
    {
        private const string StudentRole = "Student";
        private const string InstructorRole = "Instructor";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToRoleDashboard();
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new AuthViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            [Bind(Prefix = "Login")] LoginViewModel model,
            string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View("Login", new AuthViewModel { Login = model });
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToRoleDashboard();
            }

            ModelState.AddModelError(
                string.Empty,
                result.IsLockedOut
                    ? "This account is temporarily locked. Please try again later."
                    : "Invalid email address or password.");

            ViewBag.ReturnUrl = returnUrl;
            return View("Login", new AuthViewModel { Login = model });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            [Bind(Prefix = "Register")] RegisterViewModel model)
        {
            if (model.Role is not (StudentRole or InstructorRole))
            {
                ModelState.AddModelError(
                    nameof(model.Role),
                    "Choose a valid account type.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ActiveForm = "signUpForm";
                return View("Login", new AuthViewModel { Register = model });
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName.Trim(),
                PhoneNumber = model.PhoneNumber
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                AddIdentityErrors(createResult);
                ViewBag.ActiveForm = "signUpForm";
                return View("Login", new AuthViewModel { Register = model });
            }

            var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!roleResult.Succeeded)
            {
                var cleanupResult = await _userManager.DeleteAsync(user);
                AddIdentityErrors(roleResult);
                if (!cleanupResult.Succeeded)
                {
                    AddIdentityErrors(cleanupResult);
                }

                ViewBag.ActiveForm = "signUpForm";
                return View("Login", new AuthViewModel { Register = model });
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToRoleDashboard(model.Role);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToRoleDashboard(string? role = null)
        {
            if (role is null)
            {
                role = User.IsInRole("Admin")
                    ? "Admin"
                    : User.IsInRole(InstructorRole)
                        ? InstructorRole
                        : User.IsInRole(StudentRole)
                            ? StudentRole
                            : null;
            }

            return role switch
            {
                "Admin" => RedirectToAction("Index", "AdminDashboard"),
                InstructorRole => RedirectToAction("Index", "InstructorDashboard"),
                StudentRole => RedirectToAction("Index", "StudentDashboard"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        private void AddIdentityErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
