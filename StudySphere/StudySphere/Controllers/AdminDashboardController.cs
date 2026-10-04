using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudySphere.Data;
using StudySphere.Models;
using StudySphere.Repositories.Interfaces;

namespace StudySphere.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IAdminDashboardRepository _adminRepository;
        private readonly StudySphereDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public AdminDashboardController(
            IAdminDashboardRepository adminRepository,
            StudySphereDbContext dbContext,
            UserManager<ApplicationUser> userManager)
        {
            _adminRepository = adminRepository;
            _dbContext = dbContext;
            _userManager = userManager;
        }


        // =========================================================
        // 1. ADMIN DASHBOARD
        // =========================================================

        public IActionResult Index()
        {
            var students = _adminRepository.GetAllStudents();
            var instructors = _adminRepository.GetAllInstructors();
            var courses = _adminRepository.GetAllCourses();
            var enrollments = _adminRepository.GetAllEnrollments();

            ViewBag.TotalStudents = students.Count;

            ViewBag.TotalInstructors = instructors.Count;

            ViewBag.TotalCourses = courses.Count;

            ViewBag.PendingCourses =
                courses.Count(c => c.Status == "Pending");

            ViewBag.ApprovedCourses =
                courses.Count(c => c.Status == "Approved");

            ViewBag.RejectedCourses =
                courses.Count(c => c.Status == "Rejected");

            ViewBag.TotalEnrollments =
                enrollments.Count;
            ViewBag.CompletedEnrollments =
                enrollments.Count(enrollment => enrollment.Status == "Completed");
            ViewBag.PendingInstructors =
                instructors.Count(instructor => !instructor.User.IsActive);

            ViewBag.RecentPendingCourses =
                _adminRepository
                    .GetPendingCourses()
                    .Take(5)
                    .ToList();
            ViewBag.RecentRegistrations = students
                .Select(student => new
                {
                    Name = student.User.FullName,
                    Role = "Student",
                    RegisteredAt = student.User.CreatedAt,
                    Id = student.StudentId
                })
                .Concat(instructors.Select(instructor => new
                {
                    Name = instructor.User.FullName,
                    Role = "Instructor",
                    RegisteredAt = instructor.User.CreatedAt,
                    Id = instructor.InstructorId
                }))
                .OrderByDescending(registration => registration.RegisteredAt)
                .Take(8)
                .ToList();

            return View();
        }


        // =========================================================
        // 2. COURSE REQUESTS
        // =========================================================

        public IActionResult CourseRequests()
        {
            var courses =
                _adminRepository.GetPendingCourses();

            return View(courses);
        }


        // =========================================================
        // 3. REVIEW COURSE
        // =========================================================

        public IActionResult ReviewCourse(int id)
        {
            var course =
                _adminRepository.GetCourseById(id);

            if (course == null)
            {
                return NotFound();
            }

            var instructor =
                _adminRepository.GetInstructorById(
                    course.InstructorId);

            ViewBag.Instructor = instructor;

            ViewBag.Materials = course.Materials;

            ViewBag.LiveLectures = course.LiveLectures;

            ViewBag.Enrollments =
                _adminRepository
                    .GetEnrollmentsByCourseId(course.CourseId);

            return View(course);
        }


        // =========================================================
        // 4. APPROVE COURSE
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveCourse(int id)
        {
            var result =
                _adminRepository.ApproveCourse(id);

            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(CourseRequests));
        }


        // =========================================================
        // 5. REJECT COURSE
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectCourse(int id)
        {
            var result =
                _adminRepository.RejectCourse(id);

            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(CourseRequests));
        }


        // =========================================================
        // 6. ALL COURSES
        // =========================================================

        public IActionResult Courses()
        {
            var courses =
                _adminRepository.GetAllCourses();

            return View(courses);
        }


        // =========================================================
        // 7. APPROVED COURSES
        // =========================================================

        public IActionResult ApprovedCourses()
        {
            var courses =
                _adminRepository.GetApprovedCourses();

            return View(courses);
        }


        // =========================================================
        // 8. REJECTED COURSES
        // =========================================================

        public IActionResult RejectedCourses()
        {
            var courses =
                _adminRepository.GetRejectedCourses();

            return View(courses);
        }


        // =========================================================
        // 9. INSTRUCTORS
        // =========================================================

        public IActionResult Instructors()
        {
            var instructors =
                _adminRepository.GetAllInstructors();

            return View(instructors);
        }


        // =========================================================
        // 10. VIEW INSTRUCTOR
        // =========================================================

        public IActionResult ViewInstructor(int id)
        {
            var instructor =
                _adminRepository.GetInstructorById(id);

            if (instructor == null)
            {
                return NotFound();
            }

            var courses =
                _adminRepository
                    .GetCoursesByInstructorId(id);

            ViewBag.Courses = courses;

            return View(instructor);
        }


        // =========================================================
        // 11. STUDENTS
        // =========================================================

        public IActionResult Students()
        {
            var students =
                _adminRepository.GetAllStudents();

            return View(students);
        }


        // =========================================================
        // 12. VIEW STUDENT
        // =========================================================

        public IActionResult ViewStudent(int id)
        {
            var student =
                _adminRepository.GetStudentById(id);

            if (student == null)
            {
                return NotFound();
            }

            var enrollments =
                _adminRepository
                    .GetEnrollmentsByStudentId(id);

            ViewBag.Enrollments = enrollments;

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStudentActive(int id, bool isActive)
        {
            var student = _adminRepository.GetStudentById(id);
            if (student is null)
            {
                return NotFound();
            }

            var failure = await UpdateAccountStatusAsync(
                student.User.Email,
                isActive,
                () => _adminRepository.SetStudentActive(id, isActive));
            if (failure is not null)
            {
                return failure;
            }

            return RedirectToAction(nameof(Students));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetInstructorActive(int id, bool isActive)
        {
            return await UpdateInstructorStatusAsync(id, isActive);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveInstructor(int id)
        {
            return await UpdateInstructorStatusAsync(id, isActive: true);
        }

        private async Task<IActionResult> UpdateInstructorStatusAsync(
            int id,
            bool isActive)
        {
            var instructor = _adminRepository.GetInstructorById(id);
            if (instructor is null)
            {
                return NotFound();
            }

            var failure = await UpdateAccountStatusAsync(
                instructor.User.Email,
                isActive,
                () => _adminRepository.SetInstructorActive(id, isActive));
            if (failure is not null)
            {
                return failure;
            }

            TempData["AdminStatusMessage"] = isActive
                ? $"Instructor {instructor.User.FullName} was approved and can now sign in."
                : $"Instructor {instructor.User.FullName} was deactivated.";
            return RedirectToAction(nameof(Instructors));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetCourseActive(int id, bool isActive)
        {
            if (!_adminRepository.SetCourseActive(id, isActive))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Courses));
        }

        private async Task<IActionResult?> UpdateAccountStatusAsync(
            string email,
            bool isActive,
            Func<bool> updateProfile)
        {
            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync();

            if (!updateProfile())
            {
                await transaction.RollbackAsync();
                return NotFound();
            }

            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser is null)
            {
                await transaction.RollbackAsync();
                return Problem(
                    "The account's authentication record could not be found.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            var lockoutEnabledResult =
                await _userManager.SetLockoutEnabledAsync(identityUser, true);
            if (!lockoutEnabledResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return Problem(
                    "The account lockout status could not be updated.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            var lockoutResult = await _userManager.SetLockoutEndDateAsync(
                identityUser,
                isActive ? null : DateTimeOffset.MaxValue);
            if (!lockoutResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return Problem(
                    "The account status could not be updated in the authentication store.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            var stampResult =
                await _userManager.UpdateSecurityStampAsync(identityUser);
            if (!stampResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return Problem(
                    "The account sessions could not be revoked.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            await transaction.CommitAsync();
            return null;
        }
    }
}