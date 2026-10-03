using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySphere.Data;
using StudySphere.Models;

namespace StudySphere.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private const long MaximumProfilePictureSize = 5 * 1024 * 1024;
        private readonly StudySphereDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(
            StudySphereDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return Challenge();
            }

            var model = new ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Role = GetRole()
            };
            await LoadProfileDetailsAsync(model, user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(MaximumProfilePictureSize + 64 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaximumProfilePictureSize + 64 * 1024)]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return Challenge();
            }

            var role = GetRole();
            var normalizedEmail = (user.Email ?? string.Empty).Trim().ToLowerInvariant();
            var student = role == "Student"
                ? await _dbContext.Students
                    .Include(profile => profile.User)
                    .FirstOrDefaultAsync(profile =>
                        profile.User.Email.ToLower() == normalizedEmail)
                : null;
            var instructor = role == "Instructor"
                ? await _dbContext.Instructors
                    .Include(profile => profile.User)
                    .FirstOrDefaultAsync(profile =>
                        profile.User.Email.ToLower() == normalizedEmail)
                : null;

            if ((role == "Student" && student is null) ||
                (role == "Instructor" && instructor is null))
            {
                return Forbid();
            }

            if (model.ProfilePicture is not null &&
                (role == "Admin" ||
                 !await IsSupportedProfilePictureAsync(model.ProfilePicture)))
            {
                ModelState.AddModelError(
                    nameof(model.ProfilePicture),
                    "Upload a JPG, PNG, or WebP image no larger than 5 MB to a student or instructor profile.");
            }

            if (!ModelState.IsValid)
            {
                model.Email = user.Email;
                model.Role = role;
                model.ProfileImage = student?.ProfileImage ?? instructor?.ProfileImage;
                await LoadProfileDetailsAsync(model, user, student, instructor);
                return View(model);
            }

            string? newImagePath = null;
            if (model.ProfilePicture is not null)
            {
                newImagePath = await SaveProfilePictureAsync(model.ProfilePicture);
            }

            var previousImagePath = student?.ProfileImage ?? instructor?.ProfileImage;
            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync();

            user.FullName = model.FullName.Trim();
            user.PhoneNumber = model.Phone?.Trim();
            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
            {
                await transaction.RollbackAsync();
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                DeleteProfilePicture(newImagePath);
                model.Email = user.Email;
                model.Role = role;
                model.ProfileImage = student?.ProfileImage ?? instructor?.ProfileImage;
                await LoadProfileDetailsAsync(model, user, student, instructor);
                return View(model);
            }

            if (student is not null)
            {
                student.User.FullName = user.FullName;
                student.User.Phone = user.PhoneNumber;
                student.Bio = model.Bio?.Trim();
                student.ProfileImage = newImagePath ?? student.ProfileImage;
            }

            if (instructor is not null)
            {
                instructor.User.FullName = user.FullName;
                instructor.User.Phone = user.PhoneNumber;
                instructor.Bio = model.Bio?.Trim();
                instructor.ProfessionalTitle = model.ProfessionalTitle?.Trim();
                instructor.AreaOfExpertise = model.AreaOfExpertise?.Trim();
                instructor.Qualification = model.Qualification?.Trim();
                instructor.ProfileImage = newImagePath ?? instructor.ProfileImage;
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            if (newImagePath is not null)
            {
                DeleteProfilePicture(previousImagePath);
            }

            TempData["ProfileMessage"] = "Your profile was updated.";
            return RedirectToAction(nameof(Index));
        }

        private string GetRole()
        {
            return User.IsInRole("Admin")
                ? "Admin"
                : User.IsInRole("Instructor")
                    ? "Instructor"
                    : "Student";
        }

        private async Task LoadProfileDetailsAsync(
            ProfileViewModel model,
            ApplicationUser user,
            Student? student = null,
            Instructor? instructor = null)
        {
            if (model.Role == "Student")
            {
                var normalizedEmail = (user.Email ?? string.Empty).Trim().ToLowerInvariant();
                student ??= await _dbContext.Students
                    .AsNoTracking()
                    .Include(profile => profile.User)
                    .FirstOrDefaultAsync(profile =>
                        profile.User.Email.ToLower() == normalizedEmail);
                if (student is not null)
                {
                    model.Bio ??= student.Bio;
                    model.ProfileImage ??= student.ProfileImage;
                    model.EnrolledCourses = await _dbContext.Enrollments
                        .CountAsync(enrollment =>
                            enrollment.StudentId == student.StudentId &&
                            enrollment.Status != "Withdrawn");
                    model.CompletedCourses = await _dbContext.Enrollments
                        .CountAsync(enrollment =>
                            enrollment.StudentId == student.StudentId &&
                            enrollment.Status == "Completed");
                    model.Certificates = await _dbContext.CourseCertificates
                        .CountAsync(certificate =>
                            certificate.StudentId == student.StudentId);
                }
            }
            else if (model.Role == "Instructor")
            {
                var normalizedEmail = (user.Email ?? string.Empty).Trim().ToLowerInvariant();
                instructor ??= await _dbContext.Instructors
                    .AsNoTracking()
                    .Include(profile => profile.User)
                    .FirstOrDefaultAsync(profile =>
                        profile.User.Email.ToLower() == normalizedEmail);
                if (instructor is not null)
                {
                    model.Bio ??= instructor.Bio;
                    model.ProfileImage ??= instructor.ProfileImage;
                    model.ProfessionalTitle ??= instructor.ProfessionalTitle;
                    model.AreaOfExpertise ??= instructor.AreaOfExpertise;
                    model.Qualification ??= instructor.Qualification;
                }
            }
        }

        private static async Task<bool> IsSupportedProfilePictureAsync(
            IFormFile file)
        {
            if (file.Length is <= 0 or > MaximumProfilePictureSize)
            {
                return false;
            }

            var extension = Path.GetExtension(file.FileName)
                .ToLowerInvariant();
            var header = new byte[12];
            await using var stream = file.OpenReadStream();
            var bytesRead = await stream.ReadAsync(header);

            return extension switch
            {
                ".jpg" or ".jpeg" =>
                    bytesRead >= 3 &&
                    header[0] == 0xFF &&
                    header[1] == 0xD8 &&
                    header[2] == 0xFF,
                ".png" =>
                    bytesRead >= 8 &&
                    header.AsSpan(0, 8).SequenceEqual(
                        new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
                ".webp" =>
                    bytesRead >= 12 &&
                    header.AsSpan(0, 4).SequenceEqual("RIFF"u8) &&
                    header.AsSpan(8, 4).SequenceEqual("WEBP"u8),
                _ => false
            };
        }

        private async Task<string> SaveProfilePictureAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var directory = Path.Combine(
                _environment.WebRootPath
                    ?? Path.Combine(_environment.ContentRootPath, "wwwroot"),
                "uploads",
                "profile-images");
            Directory.CreateDirectory(directory);

            var path = Path.Combine(directory, fileName);
            await using var stream = new FileStream(
                path,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None);
            await file.CopyToAsync(stream);
            return $"/uploads/profile-images/{fileName}";
        }

        private void DeleteProfilePicture(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath) ||
                !imagePath.StartsWith(
                    "/uploads/profile-images/",
                    StringComparison.Ordinal))
            {
                return;
            }

            var webRoot = _environment.WebRootPath
                ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var directory = Path.GetFullPath(Path.Combine(
                webRoot,
                "uploads",
                "profile-images"));
            var filePath = Path.GetFullPath(Path.Combine(
                webRoot,
                imagePath.TrimStart('/').Replace(
                    '/',
                    Path.DirectorySeparatorChar)));
            if (filePath.StartsWith(
                    directory + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase) &&
                System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
