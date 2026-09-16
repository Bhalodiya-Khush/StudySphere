using Microsoft.AspNetCore.Mvc;
using StudySphere.Models;

namespace StudySphere.Controllers
{
    public class InstructorDashboardController : Controller
    {
        // =========================================================
        // STATIC INSTRUCTOR DATA
        // =========================================================

        private Instructor GetInstructor()
        {
            return new Instructor
            {
                InstructorId = 1,
                ProfessionalTitle = "Senior Software Developer",
                AreaOfExpertise = "Web Development",
                Qualification = "B.Tech Computer Engineering",
                Bio = "Experienced instructor in web and backend development."
            };
        }


        // =========================================================
        // STATIC COURSE DATA
        // =========================================================

        private List<Course> GetCourses()
        {
            return new List<Course>
            {
                new Course
                {
                    CourseId = 1,
                    InstructorId = 1,
                    Title = "ASP.NET Core MVC",
                    Description = "Learn ASP.NET Core MVC from basic to advanced concepts.",
                    Category = "Web Development",
                    Level = "Intermediate",
                    ThumbnailUrl = "/images/courses/aspnet.jpg",
                    Price = 999,
                    Status = "Published"
                },

                new Course
                {
                    CourseId = 2,
                    InstructorId = 1,
                    Title = "Java Spring Boot",
                    Description = "Learn backend development using Java and Spring Boot.",
                    Category = "Backend Development",
                    Level = "Intermediate",
                    ThumbnailUrl = "/images/courses/java.jpg",
                    Price = 1199,
                    Status = "Published"
                },

                new Course
                {
                    CourseId = 3,
                    InstructorId = 1,
                    Title = "Database Management",
                    Description = "Learn SQL and database management concepts.",
                    Category = "Database",
                    Level = "Beginner",
                    ThumbnailUrl = "/images/courses/database.jpg",
                    Price = 799,
                    Status = "Published"
                },

                new Course
                {
                    CourseId = 4,
                    InstructorId = 1,
                    Title = "Web Development",
                    Description = "Learn HTML, CSS and JavaScript for modern web development.",
                    Category = "Web Development",
                    Level = "Beginner",
                    ThumbnailUrl = "/images/courses/web.jpg",
                    Price = 699,
                    Status = "Draft"
                }
            };
        }


        // =========================================================
        // STATIC MATERIAL DATA
        // =========================================================

        private List<Material> GetMaterials()
        {
            return new List<Material>
            {
                new Material
                {
                    MaterialId = 1,
                    CourseId = 1,
                    Title = "Introduction to ASP.NET Core MVC",
                    Description = "Introduction to MVC architecture.",
                    MaterialType = "Video",
                    FileUrl = "/materials/aspnet/introduction.mp4",
                    DurationInMinutes = 25,
                    DisplayOrder = 1,
                    IsPublished = true
                },

                new Material
                {
                    MaterialId = 2,
                    CourseId = 1,
                    Title = "MVC Architecture Notes",
                    Description = "PDF notes about MVC architecture.",
                    MaterialType = "PDF",
                    FileUrl = "/materials/aspnet/mvc-notes.pdf",
                    DurationInMinutes = null,
                    DisplayOrder = 2,
                    IsPublished = true
                },

                new Material
                {
                    MaterialId = 3,
                    CourseId = 1,
                    Title = "Controllers in ASP.NET Core",
                    Description = "Understanding controllers and actions.",
                    MaterialType = "Video",
                    FileUrl = "/materials/aspnet/controllers.mp4",
                    DurationInMinutes = 30,
                    DisplayOrder = 3,
                    IsPublished = true
                },

                new Material
                {
                    MaterialId = 4,
                    CourseId = 2,
                    Title = "Spring Boot Introduction",
                    Description = "Introduction to Spring Boot.",
                    MaterialType = "Video",
                    FileUrl = "/materials/java/spring-introduction.mp4",
                    DurationInMinutes = 35,
                    DisplayOrder = 1,
                    IsPublished = true
                },

                new Material
                {
                    MaterialId = 5,
                    CourseId = 2,
                    Title = "Spring Boot Notes",
                    Description = "Spring Boot study material.",
                    MaterialType = "PDF",
                    FileUrl = "/materials/java/spring-notes.pdf",
                    DurationInMinutes = null,
                    DisplayOrder = 2,
                    IsPublished = false
                }
            };
        }


        // =========================================================
        // STATIC ENROLLMENT / STUDENT DATA
        // =========================================================

        private List<Enrollment> GetEnrollments()
        {
            return new List<Enrollment>
            {
                new Enrollment
                {
                    EnrollmentId = 1,
                    StudentId = 1,
                    CourseId = 1,
                    EnrolledAt = DateTime.UtcNow.AddDays(-20),
                    Status = "Active",
                    Progress = 75
                },

                new Enrollment
                {
                    EnrollmentId = 2,
                    StudentId = 2,
                    CourseId = 1,
                    EnrolledAt = DateTime.UtcNow.AddDays(-15),
                    Status = "Active",
                    Progress = 50
                },

                new Enrollment
                {
                    EnrollmentId = 3,
                    StudentId = 3,
                    CourseId = 1,
                    EnrolledAt = DateTime.UtcNow.AddDays(-10),
                    Status = "Completed",
                    Progress = 100,
                    CompletedAt = DateTime.UtcNow.AddDays(-2)
                },

                new Enrollment
                {
                    EnrollmentId = 4,
                    StudentId = 4,
                    CourseId = 2,
                    EnrolledAt = DateTime.UtcNow.AddDays(-8),
                    Status = "Active",
                    Progress = 40
                }
            };
        }


        // =========================================================
        // STATIC ANNOUNCEMENT DATA
        // =========================================================

        private List<Announcement> GetAnnouncements()
        {
            return new List<Announcement>
            {
                new Announcement
                {
                    AnnouncementId = 1,
                    CourseId = 1,
                    InstructorId = 1,
                    Title = "Welcome to ASP.NET Core MVC",
                    Message = "Welcome everyone. Please complete the first module this week.",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    IsPublished = true
                },

                new Announcement
                {
                    AnnouncementId = 2,
                    CourseId = 1,
                    InstructorId = 1,
                    Title = "Assignment 1",
                    Message = "Assignment 1 has been uploaded. Submit it before the deadline.",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    IsPublished = true
                },

                new Announcement
                {
                    AnnouncementId = 3,
                    CourseId = 2,
                    InstructorId = 1,
                    Title = "Spring Boot Practice",
                    Message = "Practice the REST API examples covered in the latest lecture.",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    IsPublished = true
                }
            };
        }


        // =========================================================
        // STATIC LIVE LECTURE DATA
        // =========================================================

        private List<LiveLecture> GetLiveLectures()
        {
            return new List<LiveLecture>
            {
                new LiveLecture
                {
                    LiveLectureId = 1,
                    CourseId = 1,
                    InstructorId = 1,
                    Title = "ASP.NET Core MVC - Controllers",
                    Description = "Live session about controllers and routing.",
                    StartTime = DateTime.Now.AddDays(1).AddHours(2),
                    EndTime = DateTime.Now.AddDays(1).AddHours(3),
                    MeetingUrl = "https://meet.example.com/aspnet",
                    Status = "Scheduled"
                },

                new LiveLecture
                {
                    LiveLectureId = 2,
                    CourseId = 2,
                    InstructorId = 1,
                    Title = "Spring Boot REST API",
                    Description = "Live session about REST API development.",
                    StartTime = DateTime.Now.AddDays(2).AddHours(3),
                    EndTime = DateTime.Now.AddDays(2).AddHours(4),
                    MeetingUrl = "https://meet.example.com/spring",
                    Status = "Scheduled"
                }
            };
        }


        // =========================================================
        // 1. INSTRUCTOR DASHBOARD
        // =========================================================

        public IActionResult Index()
        {
            var instructor = GetInstructor();
            var courses = GetCourses();
            var materials = GetMaterials();
            var lectures = GetLiveLectures();

            ViewBag.Instructor = instructor;
            ViewBag.Courses = courses;

            ViewBag.TotalCourses = courses.Count;

            // Static for now
            ViewBag.TotalStudents = 48;

            ViewBag.TotalMaterials = materials.Count;

            ViewBag.UpcomingLectures = lectures
                .Count(x => x.Status == "Scheduled");

            return View();
        }


        // =========================================================
        // 2. MY COURSES
        // =========================================================

        public IActionResult Courses()
        {
            var courses = GetCourses();

            ViewBag.Courses = courses;

            return View();
        }


        // =========================================================
        // 3. CREATE COURSE
        // =========================================================

        [HttpGet]
        public IActionResult CreateCourse()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCourse(Course course)
        {
            if (!ModelState.IsValid)
            {
                return View(course);
            }

            // Database save will be added later.

            course.InstructorId = 1;
            course.Status = "Draft";
            course.CreatedAt = DateTime.UtcNow;

            return RedirectToAction(nameof(Courses));
        }


        // =========================================================
        // 4. COURSE DETAILS
        // =========================================================

        public IActionResult CourseDetails(int id)
        {
            var courses = GetCourses();

            var course = courses.FirstOrDefault(x => x.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            var materials = GetMaterials()
                .Where(x => x.CourseId == id)
                .ToList();

            var students = GetEnrollments()
                .Where(x => x.CourseId == id)
                .ToList();

            var announcements = GetAnnouncements()
                .Where(x => x.CourseId == id)
                .ToList();

            var lectures = GetLiveLectures()
                .Where(x => x.CourseId == id)
                .ToList();

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
            var course = GetCourses()
                .FirstOrDefault(x => x.CourseId == id);

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

            // Database update will be added later.

            course.UpdatedAt = DateTime.UtcNow;

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
            var course = GetCourses()
                .FirstOrDefault(x => x.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            var materials = GetMaterials()
                .Where(x => x.CourseId == courseId)
                .OrderBy(x => x.DisplayOrder)
                .ToList();

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
            var course = GetCourses()
                .FirstOrDefault(x => x.CourseId == courseId);

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
                ViewBag.Course = GetCourses()
                    .FirstOrDefault(x => x.CourseId == material.CourseId);

                return View(material);
            }

            // File upload and database save will be added later.

            material.UploadedAt = DateTime.UtcNow;

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
            var material = GetMaterials()
                .FirstOrDefault(x => x.MaterialId == id);

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

            // Database update will be added later.

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
            var material = GetMaterials()
                .FirstOrDefault(x => x.MaterialId == id);

            if (material == null)
            {
                return NotFound();
            }

            // Database delete will be added later.

            return RedirectToAction(
                nameof(Materials),
                new { courseId = material.CourseId }
            );
        }


        // =========================================================
        // 10. STUDENTS
        // =========================================================

        public IActionResult Students(int courseId)
        {
            var course = GetCourses()
                .FirstOrDefault(x => x.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            var enrollments = GetEnrollments()
                .Where(x => x.CourseId == courseId)
                .ToList();

            ViewBag.Course = course;
            ViewBag.Enrollments = enrollments;

            return View();
        }


        // =========================================================
        // 11. ANNOUNCEMENTS
        // =========================================================

        public IActionResult Announcements(int courseId)
        {
            var course = GetCourses()
                .FirstOrDefault(x => x.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            var announcements = GetAnnouncements()
                .Where(x => x.CourseId == courseId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

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
            var course = GetCourses()
                .FirstOrDefault(x => x.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Course = course;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAnnouncement(Announcement announcement)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Course = GetCourses()
                    .FirstOrDefault(x => x.CourseId == announcement.CourseId);

                return View(announcement);
            }

            announcement.InstructorId = 1;
            announcement.CreatedAt = DateTime.UtcNow;

            // Database save will be added later.

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
            var announcement = GetAnnouncements()
                .FirstOrDefault(x => x.AnnouncementId == id);

            if (announcement == null)
            {
                return NotFound();
            }

            return View(announcement);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAnnouncement(Announcement announcement)
        {
            if (!ModelState.IsValid)
            {
                return View(announcement);
            }

            // Database update will be added later.

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
            var announcement = GetAnnouncements()
                .FirstOrDefault(x => x.AnnouncementId == id);

            if (announcement == null)
            {
                return NotFound();
            }

            // Database delete will be added later.

            return RedirectToAction(
                nameof(Announcements),
                new { courseId = announcement.CourseId }
            );
        }


        // =========================================================
        // 15. LIVE LECTURES
        // =========================================================

        public IActionResult LiveLectures(int courseId)
        {
            var course = GetCourses()
                .FirstOrDefault(x => x.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            var lectures = GetLiveLectures()
                .Where(x => x.CourseId == courseId)
                .OrderBy(x => x.StartTime)
                .ToList();

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
            var course = GetCourses()
                .FirstOrDefault(x => x.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Course = course;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateLiveLecture(LiveLecture lecture)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Course = GetCourses()
                    .FirstOrDefault(x => x.CourseId == lecture.CourseId);

                return View(lecture);
            }

            lecture.InstructorId = 1;
            lecture.Status = "Scheduled";
            lecture.CreatedAt = DateTime.UtcNow;

            // Database save will be added later.

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
            var lecture = GetLiveLectures()
                .FirstOrDefault(x => x.LiveLectureId == id);

            if (lecture == null)
            {
                return NotFound();
            }

            return View(lecture);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditLiveLecture(LiveLecture lecture)
        {
            if (!ModelState.IsValid)
            {
                return View(lecture);
            }

            // Database update will be added later.

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
            var lecture = GetLiveLectures()
                .FirstOrDefault(x => x.LiveLectureId == id);

            if (lecture == null)
            {
                return NotFound();
            }

            // Database delete will be added later.

            return RedirectToAction(
                nameof(LiveLectures),
                new { courseId = lecture.CourseId }
            );
        }
    }
}