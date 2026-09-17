using Microsoft.AspNetCore.Mvc;
using StudySphere.Repositories.Interfaces;

namespace StudySphere.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly IAdminDashboardRepository _adminRepository;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public AdminDashboardController(
            IAdminDashboardRepository adminRepository)
        {
            _adminRepository = adminRepository;
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

            ViewBag.RecentPendingCourses =
                _adminRepository
                    .GetPendingCourses()
                    .Take(5)
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
    }
}