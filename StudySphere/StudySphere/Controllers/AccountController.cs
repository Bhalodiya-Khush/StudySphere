using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySphere.Data;
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
        private readonly StudySphereDbContext _dbContext;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            StudySphereDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
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
                var signedInUser = await _userManager.FindByEmailAsync(model.Email);
                var isAdministrator = signedInUser is not null &&
                    await _userManager.IsInRoleAsync(signedInUser, "Admin");
                var isActive = isAdministrator;
                if (!isActive && signedInUser is not null)
                {
                    var identityRoles =
                        await _userManager.GetRolesAsync(signedInUser);
                    var normalizedEmail = (signedInUser.Email ?? string.Empty)
                        .Trim()
                        .ToLowerInvariant();
                    var profile = await _dbContext.Users
                        .AsNoTracking()
                        .SingleOrDefaultAsync(user =>
                            user.Email.ToLower() == normalizedEmail);
                    isActive = profile?.IsActive == true &&
                        identityRoles.Contains(profile.Role);
                }

                if (!isActive)
                {
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(
                        string.Empty,
                        "This account is inactive, awaiting instructor approval, or has no active profile. Contact an administrator.");
                    ViewBag.ReturnUrl = returnUrl;
                    return View("Login", new AuthViewModel { Login = model });
                }

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

            if (model.Role == InstructorRole)
            {
                if (string.IsNullOrWhiteSpace(model.ProfessionalTitle))
                {
                    ModelState.AddModelError(
                        nameof(model.ProfessionalTitle),
                        "Professional title is required for instructors.");
                }

                if (string.IsNullOrWhiteSpace(model.AreaOfExpertise))
                {
                    ModelState.AddModelError(
                        nameof(model.AreaOfExpertise),
                        "Area of expertise is required for instructors.");
                }

                if (string.IsNullOrWhiteSpace(model.Qualification))
                {
                    ModelState.AddModelError(
                        nameof(model.Qualification),
                        "Qualification is required for instructors.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ActiveForm = "signUpForm";
                return View("Login", new AuthViewModel { Register = model });
            }

            var email = model.Email.Trim();
            var normalizedEmail = email.ToLowerInvariant();
            var user = new ApplicationUser
            {
                UserName = normalizedEmail,
                Email = normalizedEmail,
                FullName = model.FullName.Trim(),
                PhoneNumber = model.PhoneNumber
            };

            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync();

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                await transaction.RollbackAsync();
                AddIdentityErrors(createResult);
                ViewBag.ActiveForm = "signUpForm";
                return View("Login", new AuthViewModel { Register = model });
            }

            var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                AddIdentityErrors(roleResult);
                ViewBag.ActiveForm = "signUpForm";
                return View("Login", new AuthViewModel { Register = model });
            }

            var profileUser = new User
            {
                FullName = user.FullName,
                Email = normalizedEmail,
                PasswordHash = user.PasswordHash!,
                Phone = user.PhoneNumber,
                Role = model.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = model.Role == StudentRole
            };
            _dbContext.Users.Add(profileUser);

            if (model.Role == StudentRole)
            {
                _dbContext.Students.Add(new Student { User = profileUser });
            }
            else
            {
                _dbContext.Instructors.Add(new Instructor
                {
                    User = profileUser,
                    ProfessionalTitle = model.ProfessionalTitle?.Trim(),
                    AreaOfExpertise = model.AreaOfExpertise?.Trim(),
                    Qualification = model.Qualification?.Trim()
                });
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            if (model.Role == InstructorRole)
            {
                TempData["AuthenticationMessage"] =
                    "Your instructor registration was submitted for administrator approval. You can sign in after your account is activated.";
                return RedirectToAction(nameof(Login));
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
