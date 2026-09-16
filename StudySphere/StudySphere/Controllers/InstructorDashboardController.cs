using Microsoft.AspNetCore.Mvc;
using StudySphere.Models;
using StudySphere.Repositories.Interfaces;

namespace StudySphere.Controllers
{
    public class InstructorDashboardController : Controller
    {
        // =========================================================
        // REPOSITORY
        // =========================================================

        private readonly IInstructorDashboardRepository _repository;

        public InstructorDashboardController(
            IInstructorDashboardRepository repository)
        {
            _repository = repository;
        }


        // =========================================================
        // 1. INSTRUCTOR DASHBOARD
        // =========================================================

        public IActionResult Index()
        {
            int instructorId = 1;

            var instructor =
                _repository.GetInstructor(instructorId);

            var courses =
                _repository.GetCourses(instructorId);

            var materialsCount = courses
                .SelectMany(course =>
                    _repository.GetMaterials(course.CourseId))
                .Count();

            var upcomingLectures = courses
                .SelectMany(course =>
                    _repository.GetLiveLectures(course.CourseId))
                .Count(x => x.Status == "Scheduled");

            var totalStudents =
                _repository.GetTotalStudents(instructorId);


            ViewBag.Instructor = instructor;
            ViewBag.Courses = courses;

            ViewBag.TotalCourses = courses.Count;
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalMaterials = materialsCount;
            ViewBag.UpcomingLectures = upcomingLectures;

            return View();
        }


        // =========================================================
        // 2. MY COURSES
        // =========================================================

        public IActionResult Courses()
        {
            int instructorId = 1;

            var courses =
                _repository.GetCourses(instructorId);

            ViewBag.Courses = courses;

            return View();
        }


        // =========================================================
        // 3. CREATE COURSE
        // =========================================================

        [HttpGet]
        public IActionResult CreateCourse()
        {
            int instructorId = 1;

            var instructor =
                _repository.GetInstructor(instructorId);

            ViewBag.Instructor = instructor;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCourse(Course course)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Instructor =
                    _repository.GetInstructor(1);

                return View(course);
            }

            // -----------------------------------------------------
            // Instructor comes from logged-in user later.
            // For now static instructor ID = 1.
            // -----------------------------------------------------

            course.InstructorId = 1;
            course.Status = "Draft";
            course.CreatedAt = DateTime.UtcNow;

            _repository.AddCourse(course);

            return RedirectToAction(nameof(Courses));
        }


        // =========================================================
        // 4. COURSE DETAILS
        // =========================================================

        public IActionResult CourseDetails(int id)
        {
            var course =
                _repository.GetCourseById(id);

            if (course == null)
            {
                return NotFound();
            }


            var materials =
                _repository.GetMaterials(id);

            var students =
                _repository.GetEnrollments(id);

            var announcements =
                _repository.GetAnnouncements(id);

            var lectures =
                _repository.GetLiveLectures(id);


            ViewBag.Course = course;
            ViewBag.Materials = materials;
            ViewBag.Students = students;
            ViewBag.Announcements = announcements;
            ViewBag.LiveLectures = lectures;

            return View(course);
        }


        // =========================================================
        // 5. EDIT COURSE
        // =========================================================

        [HttpGet]
        public IActionResult EditCourse(int id)
        {
            var course =
                _repository.GetCourseById(id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCourse(Course course)
        {
            if (!ModelState.IsValid)
            {
                return View(course);
            }

            course.UpdatedAt = DateTime.UtcNow;

            _repository.UpdateCourse(course);

            return RedirectToAction(
                nameof(CourseDetails),
                new { id = course.CourseId }
            );
        }


        // =========================================================
        // 6. MATERIALS
        // =========================================================

        public IActionResult Materials(int courseId)
        {
            var course =
                _repository.GetCourseById(courseId);

            if (course == null)
            {
                return NotFound();
            }


            var materials =
                _repository.GetMaterials(courseId);


            ViewBag.Course = course;
            ViewBag.Materials = materials;

            return View();
        }


        // =========================================================
        // 7. CREATE MATERIAL
        // =========================================================

        [HttpGet]
        public IActionResult CreateMaterial(int courseId)
        {
            var course =
                _repository.GetCourseById(courseId);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Course = course;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateMaterial(Material material)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Course =
                    _repository.GetCourseById(
                        material.CourseId);

                return View(material);
            }


            material.UploadedAt = DateTime.UtcNow;

            _repository.AddMaterial(material);

            return RedirectToAction(
                nameof(Materials),
                new { courseId = material.CourseId }
            );
        }


        // =========================================================
        // 8. EDIT MATERIAL
        // =========================================================

        [HttpGet]
        public IActionResult EditMaterial(int id)
        {
            var material =
                _repository.GetMaterialById(id);

            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMaterial(Material material)
        {
            if (!ModelState.IsValid)
            {
                return View(material);
            }


            _repository.UpdateMaterial(material);

            return RedirectToAction(
                nameof(Materials),
                new { courseId = material.CourseId }
            );
        }


        // =========================================================
        // 9. DELETE MATERIAL
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMaterial(int id)
        {
            var material =
                _repository.GetMaterialById(id);

            if (material == null)
            {
                return NotFound();
            }


            int courseId = material.CourseId;

            _repository.DeleteMaterial(id);

            return RedirectToAction(
                nameof(Materials),
                new { courseId = courseId }
            );
        }


        // =========================================================
        // 10. STUDENTS
        // =========================================================

        public IActionResult Students(int courseId)
        {
            var course =
                _repository.GetCourseById(courseId);

            if (course == null)
            {
                return NotFound();
            }


            var enrollments =
                _repository.GetEnrollments(courseId);


            ViewBag.Course = course;
            ViewBag.Enrollments = enrollments;

            return View();
        }


        // =========================================================
        // 11. ANNOUNCEMENTS
        // =========================================================

        public IActionResult Announcements(int courseId)
        {
            var course =
                _repository.GetCourseById(courseId);

            if (course == null)
            {
                return NotFound();
            }


            var announcements =
                _repository.GetAnnouncements(courseId);


            ViewBag.Course = course;
            ViewBag.Announcements = announcements;

            return View();
        }


        // =========================================================
        // 12. CREATE ANNOUNCEMENT
        // =========================================================

        [HttpGet]
        public IActionResult CreateAnnouncement(int courseId)
        {
            var course =
                _repository.GetCourseById(courseId);

            if (course == null)
            {
                return NotFound();
            }


            ViewBag.Course = course;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAnnouncement(
            Announcement announcement)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Course =
                    _repository.GetCourseById(
                        announcement.CourseId);

                return View(announcement);
            }


            announcement.InstructorId = 1;
            announcement.CreatedAt = DateTime.UtcNow;


            _repository.AddAnnouncement(announcement);


            return RedirectToAction(
                nameof(Announcements),
                new { courseId = announcement.CourseId }
            );
        }


        // =========================================================
        // 13. EDIT ANNOUNCEMENT
        // =========================================================

        [HttpGet]
        public IActionResult EditAnnouncement(int id)
        {
            var announcement =
                _repository.GetAnnouncementById(id);

            if (announcement == null)
            {
                return NotFound();
            }


            ViewBag.Course =
                _repository.GetCourseById(
                    announcement.CourseId);

            ViewBag.Announcement = announcement;

            return View(announcement);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAnnouncement(
            Announcement announcement)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Course =
                    _repository.GetCourseById(
                        announcement.CourseId);

                ViewBag.Announcement = announcement;

                return View(announcement);
            }


            _repository.UpdateAnnouncement(announcement);


            return RedirectToAction(
                nameof(Announcements),
                new { courseId = announcement.CourseId }
            );
        }


        // =========================================================
        // 14. DELETE ANNOUNCEMENT
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAnnouncement(int id)
        {
            var announcement =
                _repository.GetAnnouncementById(id);

            if (announcement == null)
            {
                return NotFound();
            }


            int courseId = announcement.CourseId;

            _repository.DeleteAnnouncement(id);


            return RedirectToAction(
                nameof(Announcements),
                new { courseId = courseId }
            );
        }


        // =========================================================
        // 15. LIVE LECTURES
        // =========================================================

        public IActionResult LiveLectures(int courseId)
        {
            var course =
                _repository.GetCourseById(courseId);

            if (course == null)
            {
                return NotFound();
            }


            var lectures =
                _repository.GetLiveLectures(courseId);


            ViewBag.Course = course;
            ViewBag.LiveLectures = lectures;

            return View();
        }


        // =========================================================
        // 16. CREATE LIVE LECTURE
        // =========================================================

        [HttpGet]
        public IActionResult CreateLiveLecture(int courseId)
        {
            var course =
                _repository.GetCourseById(courseId);

            if (course == null)
            {
                return NotFound();
            }


            ViewBag.Course = course;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateLiveLecture(
            LiveLecture lecture)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Course =
                    _repository.GetCourseById(
                        lecture.CourseId);

                return View(lecture);
            }


            lecture.InstructorId = 1;
            lecture.Status = "Scheduled";
            lecture.CreatedAt = DateTime.UtcNow;


            _repository.AddLiveLecture(lecture);


            return RedirectToAction(
                nameof(LiveLectures),
                new { courseId = lecture.CourseId }
            );
        }


        // =========================================================
        // 17. EDIT LIVE LECTURE
        // =========================================================

        [HttpGet]
        public IActionResult EditLiveLecture(int id)
        {
            var lecture =
                _repository.GetLiveLectureById(id);

            if (lecture == null)
            {
                return NotFound();
            }


            ViewBag.Course =
                _repository.GetCourseById(
                    lecture.CourseId);

            ViewBag.LiveLecture = lecture;

            return View(lecture);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditLiveLecture(
            LiveLecture lecture)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Course =
                    _repository.GetCourseById(
                        lecture.CourseId);

                ViewBag.LiveLecture = lecture;

                return View(lecture);
            }


            _repository.UpdateLiveLecture(lecture);


            return RedirectToAction(
                nameof(LiveLectures),
                new { courseId = lecture.CourseId }
            );
        }


        // =========================================================
        // 18. DELETE LIVE LECTURE
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteLiveLecture(int id)
        {
            var lecture =
                _repository.GetLiveLectureById(id);

            if (lecture == null)
            {
                return NotFound();
            }


            int courseId = lecture.CourseId;

            _repository.DeleteLiveLecture(id);


            return RedirectToAction(
                nameof(LiveLectures),
                new { courseId = courseId }
            );
        }
    }
}