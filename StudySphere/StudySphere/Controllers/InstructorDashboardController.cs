using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySphere.Data;
using StudySphere.Models;
using StudySphere.Models.ViewModels;
using StudySphere.Repositories.Interfaces;

namespace StudySphere.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorDashboardController : Controller
    {
        // =========================================================
        // REPOSITORY
        // =========================================================

        private readonly IInstructorDashboardRepository _repository;
        private readonly IWebHostEnvironment _environment;
        private readonly StudySphereDbContext _dbContext;

        public InstructorDashboardController(
            IInstructorDashboardRepository repository,
            IWebHostEnvironment environment,
            StudySphereDbContext dbContext)
        {
            _repository = repository;
            _environment = environment;
            _dbContext = dbContext;
        }


        // =========================================================
        // 1. INSTRUCTOR DASHBOARD
        // =========================================================

        public IActionResult Index()
        {
            var instructorId = CurrentInstructorId();
            if (instructorId is null)
            {
                return Forbid();
            }

            var instructor = _repository.GetInstructor(instructorId.Value);
            var courses = _repository.GetCourses(instructorId.Value);

            var materialsCount = courses
                .SelectMany(course =>
                    _repository.GetMaterials(course.CourseId))
                .Count();

            var upcomingLectures = courses
                .SelectMany(course =>
                    _repository.GetLiveLectures(course.CourseId))
                .Count(x => x.Status == "Scheduled");

            var totalStudents =
                _repository.GetTotalStudents(instructorId.Value);

            ViewBag.Instructor = instructor;
            ViewBag.Courses = courses;
            ViewBag.TotalCourses = courses.Count;
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalMaterials = materialsCount;
            ViewBag.UpcomingLectures = upcomingLectures;

            ViewData["IsInstructor"] = true;

            return View();
        }


        // =========================================================
        // 2. MY COURSES
        // =========================================================

        public IActionResult Courses()
        {
            var instructorId = CurrentInstructorId();
            if (instructorId is null)
            {
                return Forbid();
            }

            var courses = _repository.GetCourses(instructorId.Value);

            ViewData["IsInstructor"] = true;

            ViewBag.Courses = courses;

            return View();
        }


        // =========================================================
        // 3. CREATE COURSE
        // =========================================================

        [HttpGet]
        public IActionResult CreateCourse()
        {
            var instructorId = CurrentInstructorId();
            if (instructorId is null)
            {
                return Forbid();
            }

            var instructor =
                _repository.GetInstructor(instructorId.Value);

            ViewBag.Instructor = instructor;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCourse(Course course)
        {
            var instructorId = CurrentInstructorId();
            if (instructorId is null)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Instructor =
                    _repository.GetInstructor(instructorId.Value);

                return View(course);
            }

            course.InstructorId = instructorId.Value;
            course.Status = "Pending";
            course.CreatedAt = DateTime.UtcNow;

            _repository.AddCourse(course);

            return RedirectToAction(nameof(Courses));
        }


        // =========================================================
        // 4. COURSE DETAILS
        // =========================================================

        public IActionResult CourseDetails(int id)
        {
            var course = GetOwnedCourse(id);

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

            var assignments = _dbContext.Assignments
                .AsNoTracking()
                .Where(assignment => assignment.CourseId == id)
                .ToList();
            var quizzes = _dbContext.Quizzes
                .AsNoTracking()
                .Where(quiz => quiz.CourseId == id)
                .ToList();

            ViewBag.Course = course;
            ViewBag.Materials = materials;
            ViewBag.Students = students;
            ViewBag.Announcements = announcements;
            ViewBag.LiveLectures = lectures;
            ViewBag.Assignments = assignments;
            ViewBag.Quizzes = quizzes;

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartLiveLecture(int courseId)
        {
            var instructorId = CurrentInstructorId();
            var course = GetOwnedCourse(courseId);

            if (course == null || instructorId is null)
            {
                return NotFound();
            }

            // Check whether a live lecture is already running
            var existingLecture = _repository
                .GetLiveLectures(courseId)
                .FirstOrDefault(x => x.Status == "Live");

            if (existingLecture != null)
            {
                return RedirectToAction(
                    "Room",
                    "LiveLecture",
                    new { lectureId = existingLecture.LiveLectureId }
                );
            }

            // Create a new live lecture
            var lecture = new LiveLecture
            {
                CourseId = courseId,
                InstructorId = instructorId.Value,
                Title = $"{course.Title} - Live Lecture",
                Description = $"Live lecture for {course.Title}",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(2),
                Status = "Live",
                CreatedAt = DateTime.UtcNow
            };

            _repository.AddLiveLecture(lecture);

            // lecture.LiveLectureId is now available
            return RedirectToAction(
                "Room",
                "LiveLecture",
                new { lectureId = lecture.LiveLectureId }
            );
        }

        // =========================================================
        // 5. EDIT COURSE
        // =========================================================

        [HttpGet]
        public IActionResult EditCourse(int id)
        {
            var course = GetOwnedCourse(id);

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
            var instructorId = CurrentInstructorId();
            var existing = GetOwnedCourse(course.CourseId);
            if (existing is null || instructorId is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Instructor = _repository.GetInstructor(instructorId.Value);
                return View(course);
            }

            course.InstructorId = instructorId.Value;
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
            var course = GetOwnedCourse(courseId);

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
            var course = GetOwnedCourse(courseId);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Course = course;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(524_288_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 524_288_000)]
        public async Task<IActionResult> CreateMaterial(
            Material material,
            IFormFile? upload)
        {
            var course = GetOwnedCourse(material.CourseId);
            if (course is null)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(material.FileUrl));
            if (upload is null || upload.Length == 0)
            {
                ModelState.AddModelError(
                    nameof(upload),
                    "Choose a file to upload.");
            }
            else if (upload.Length > 500L * 1024 * 1024)
            {
                ModelState.AddModelError(
                    nameof(upload),
                    "The maximum upload size is 500 MB.");
            }

            if (upload is not null &&
                !IsAllowedMaterialFile(material.MaterialType, upload))
            {
                ModelState.AddModelError(
                    nameof(upload),
                    "The selected file type does not match the material type.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Course = course;
                return View(material);
            }

            material.FileUrl = await SaveMaterialFileAsync(
                course.CourseId,
                upload!);
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

            if (material == null || GetOwnedCourse(material.CourseId) is null)
            {
                return NotFound();
            }

            return View(material);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(524_288_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 524_288_000)]
        public async Task<IActionResult> EditMaterial(
            Material material,
            IFormFile? upload)
        {
            var existing = _repository.GetMaterialById(material.MaterialId);
            if (existing is null || GetOwnedCourse(existing.CourseId) is null)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(material.FileUrl));
            if (upload is not null && upload.Length > 500L * 1024 * 1024)
            {
                ModelState.AddModelError(
                    nameof(upload),
                    "The maximum upload size is 500 MB.");
            }

            if (upload is not null &&
                !IsAllowedMaterialFile(material.MaterialType, upload))
            {
                ModelState.AddModelError(
                    nameof(upload),
                    "The selected file type does not match the material type.");
            }

            if (!ModelState.IsValid)
            {
                material.CourseId = existing.CourseId;
                return View(material);
            }

            material.CourseId = existing.CourseId;
            material.FileUrl = upload is null
                ? existing.FileUrl
                : await SaveMaterialFileAsync(existing.CourseId, upload);
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

            if (material == null || GetOwnedCourse(material.CourseId) is null)
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
            var course = GetOwnedCourse(courseId);

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
            var course = GetOwnedCourse(courseId);

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
            var course = GetOwnedCourse(courseId);

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
            var instructorId = CurrentInstructorId();
            var course = GetOwnedCourse(announcement.CourseId);
            if (instructorId is null || course is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Course = course;
                return View(announcement);
            }

            announcement.InstructorId = instructorId.Value;
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

            if (announcement == null ||
                GetOwnedCourse(announcement.CourseId) is null)
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
            var existing =
                _repository.GetAnnouncementById(announcement.AnnouncementId);
            if (existing is null ||
                GetOwnedCourse(existing.CourseId) is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Course =
                    _repository.GetCourseById(existing.CourseId);

                ViewBag.Announcement = announcement;

                return View(announcement);
            }

            announcement.CourseId = existing.CourseId;
            announcement.InstructorId = existing.InstructorId;
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

            if (announcement == null ||
                GetOwnedCourse(announcement.CourseId) is null)
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
            var course = GetOwnedCourse(courseId);

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
            var course = GetOwnedCourse(courseId);

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
            var instructorId = CurrentInstructorId();
            var course = GetOwnedCourse(lecture.CourseId);
            if (instructorId is null || course is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Course = course;
                return View(lecture);
            }

            if (lecture.EndTime <= lecture.StartTime)
            {
                ModelState.AddModelError(
                    nameof(lecture.EndTime),
                    "The lecture end time must be later than its start time.");
                ViewBag.Course = course;
                return View(lecture);
            }

            lecture.InstructorId = instructorId.Value;
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

            if (lecture == null ||
                GetOwnedCourse(lecture.CourseId) is null)
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
            var existing =
                _repository.GetLiveLectureById(lecture.LiveLectureId);
            if (existing is null ||
                GetOwnedCourse(existing.CourseId) is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Course =
                    _repository.GetCourseById(existing.CourseId);

                ViewBag.LiveLecture = lecture;

                return View(lecture);
            }

            if (lecture.EndTime <= lecture.StartTime)
            {
                ModelState.AddModelError(
                    nameof(lecture.EndTime),
                    "The lecture end time must be later than its start time.");
                lecture.CourseId = existing.CourseId;
                ViewBag.Course = _repository.GetCourseById(existing.CourseId);
                ViewBag.LiveLecture = lecture;
                return View(lecture);
            }

            lecture.CourseId = existing.CourseId;
            lecture.InstructorId = existing.InstructorId;
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

            if (lecture == null ||
                GetOwnedCourse(lecture.CourseId) is null)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartScheduledLecture(int id)
        {
            var lecture = _repository.GetLiveLectureById(id);
            if (lecture is null ||
                GetOwnedCourse(lecture.CourseId) is null)
            {
                return NotFound();
            }

            if (lecture.Status != "Scheduled" ||
                lecture.StartTime > DateTime.UtcNow ||
                lecture.EndTime <= DateTime.UtcNow ||
                _repository.GetLiveLectures(lecture.CourseId)
                    .Any(item =>
                        item.LiveLectureId != id &&
                        item.Status == "Live"))
            {
                TempData["LiveLectureMessage"] =
                    "This lecture cannot be started now. Check its schedule and ensure no other lecture is live.";
                return RedirectToAction(
                    nameof(LiveLectures),
                    new { courseId = lecture.CourseId });
            }

            lecture.Status = "Live";
            _repository.UpdateLiveLecture(lecture);

            return RedirectToAction(
                "Test",
                "LiveLecture",
                new { lectureId = lecture.LiveLectureId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EndLiveLecture(int id)
        {
            var lecture = _repository.GetLiveLectureById(id);
            if (lecture is null ||
                GetOwnedCourse(lecture.CourseId) is null)
            {
                return NotFound();
            }

            if (lecture.Status != "Live")
            {
                return BadRequest("Only a live lecture can be ended.");
            }

            lecture.Status = "Completed";
            _repository.UpdateLiveLecture(lecture);

            return RedirectToAction(
                nameof(LiveLectures),
                new { courseId = lecture.CourseId });
        }

        public IActionResult Quizzes(int courseId)
        {
            var course = GetOwnedCourse(courseId);
            if (course is null)
            {
                return NotFound();
            }

            var quizzes = _dbContext.Quizzes
                .AsNoTracking()
                .Include(quiz => quiz.Questions)
                .Include(quiz => quiz.Attempts)
                .Where(quiz => quiz.CourseId == courseId)
                .OrderByDescending(quiz => quiz.CreatedAt)
                .ToList();
            ViewBag.Course = course;
            return View(quizzes);
        }

        [HttpGet]
        public IActionResult CreateQuiz(int courseId)
        {
            var course = GetOwnedCourse(courseId);
            if (course is null)
            {
                return NotFound();
            }

            ViewBag.Course = course;
            var now = DateTime.UtcNow;
            return View(new CreateQuizViewModel
            {
                OpensAt = new DateTime(
                    now.Year,
                    now.Month,
                    now.Day,
                    now.Hour,
                    now.Minute,
                    0,
                    DateTimeKind.Utc),
                MaxAttempts = 1
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateQuiz(int courseId, CreateQuizViewModel model)
        {
            var course = GetOwnedCourse(courseId);
            if (course is null)
            {
                return NotFound();
            }

            if (model.DueAt.HasValue && model.DueAt <= model.OpensAt)
            {
                ModelState.AddModelError(
                    nameof(model.DueAt),
                    "The closing time must be later than the opening time.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Course = course;
                return View(model);
            }

            var quiz = new Quiz
            {
                CourseId = course.CourseId,
                Course = course,
                Title = model.Title,
                Description = model.Description,
                OpensAt = model.OpensAt,
                DueAt = model.DueAt,
                MaxAttempts = model.MaxAttempts,
                IsPublished = false,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Quizzes.Add(quiz);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Quizzes), new { courseId = course.CourseId });
        }

        [HttpGet]
        public IActionResult AddQuizQuestion(int quizId)
        {
            var quiz = GetOwnedQuiz(quizId);
            if (quiz is null)
            {
                return NotFound();
            }

            ViewBag.Quiz = quiz;
            return View(new QuizQuestion { QuizId = quizId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddQuizQuestion(QuizQuestion question)
        {
            var quiz = GetOwnedQuiz(question.QuizId);
            if (quiz is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Quiz = quiz;
                return View(question);
            }

            _dbContext.QuizQuestions.Add(question);
            _dbContext.SaveChanges();
            TempData["QuizMessage"] = "Question added.";
            return RedirectToAction(nameof(Quizzes), new { courseId = quiz.CourseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PublishQuiz(int quizId)
        {
            var quiz = GetOwnedQuiz(quizId);
            if (quiz is null)
            {
                return NotFound();
            }

            if (!_dbContext.QuizQuestions.Any(question => question.QuizId == quizId))
            {
                TempData["QuizError"] = "Add at least one question before publishing.";
                return RedirectToAction(nameof(Quizzes), new { courseId = quiz.CourseId });
            }

            quiz.IsPublished = true;
            _dbContext.SaveChanges();
            TempData["QuizMessage"] = "Quiz published for enrolled students.";
            return RedirectToAction(nameof(Quizzes), new { courseId = quiz.CourseId });
        }

        public IActionResult QuizAttempts(int quizId)
        {
            var quiz = GetOwnedQuiz(quizId);
            if (quiz is null)
            {
                return NotFound();
            }

            var attempts = _dbContext.QuizAttempts
                .AsNoTracking()
                .Include(attempt => attempt.Student)
                    .ThenInclude(student => student.User)
                .Where(attempt => attempt.QuizId == quizId)
                .OrderByDescending(attempt => attempt.SubmittedAt)
                .ToList();
            ViewBag.Quiz = quiz;
            return View(attempts);
        }

        public IActionResult Assignments(int courseId)
        {
            var course = GetOwnedCourse(courseId);
            if (course is null)
            {
                return NotFound();
            }

            var assignments = _dbContext.Assignments
                .AsNoTracking()
                .Where(assignment => assignment.CourseId == courseId)
                .OrderByDescending(assignment => assignment.CreatedAt)
                .ToList();
            ViewBag.Course = course;
            return View(assignments);
        }

        [HttpGet]
        public IActionResult CreateAssignment(int courseId)
        {
            var course = GetOwnedCourse(courseId);
            if (course is null)
            {
                return NotFound();
            }

            ViewBag.Course = course;
            return View(new Assignment
            {
                CourseId = courseId,
                DueAt = DateTime.UtcNow.AddDays(7),
                MaxMarks = 100
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAssignment(Assignment assignment)
        {
            var course = GetOwnedCourse(assignment.CourseId);
            if (course is null)
            {
                return NotFound();
            }

            if (assignment.DueAt <= DateTime.UtcNow)
            {
                ModelState.AddModelError(
                    nameof(assignment.DueAt),
                    "The deadline must be in the future.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Course = course;
                return View(assignment);
            }

            assignment.IsPublished = true;
            assignment.CreatedAt = DateTime.UtcNow;
            _dbContext.Assignments.Add(assignment);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Assignments), new { courseId = course.CourseId });
        }

        public IActionResult AssignmentSubmissions(int assignmentId)
        {
            var assignment = _dbContext.Assignments
                .Include(item => item.Course)
                .FirstOrDefault(item => item.AssignmentId == assignmentId);
            if (assignment is null ||
                GetOwnedCourse(assignment.CourseId) is null)
            {
                return NotFound();
            }

            var submissions = _dbContext.AssignmentSubmissions
                .AsNoTracking()
                .Include(submission => submission.Student)
                    .ThenInclude(student => student.User)
                .Where(submission => submission.AssignmentId == assignmentId)
                .OrderByDescending(submission => submission.SubmittedAt)
                .ToList();
            ViewBag.Assignment = assignment;
            return View(submissions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EvaluateAssignmentSubmission(
            int submissionId,
            decimal marks,
            string? feedback)
        {
            var submission = _dbContext.AssignmentSubmissions
                .Include(item => item.Assignment)
                    .ThenInclude(assignment => assignment.Course)
                .FirstOrDefault(item => item.SubmissionId == submissionId);
            if (submission is null ||
                GetOwnedCourse(submission.Assignment.CourseId) is null)
            {
                return NotFound();
            }

            if (marks < 0 || marks > submission.Assignment.MaxMarks)
            {
                ModelState.AddModelError(
                    nameof(marks),
                    $"Marks must be between 0 and {submission.Assignment.MaxMarks}.");
                TempData["AssignmentError"] =
                    $"Marks must be between 0 and {submission.Assignment.MaxMarks}.";
                return RedirectToAction(
                    nameof(AssignmentSubmissions),
                    new { assignmentId = submission.AssignmentId });
            }

            submission.MarksAwarded = marks;
            submission.Feedback = feedback?.Trim();
            submission.EvaluatedAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
            TempData["AssignmentMessage"] = "Submission evaluated.";
            return RedirectToAction(
                nameof(AssignmentSubmissions),
                new { assignmentId = submission.AssignmentId });
        }

        public IActionResult DownloadSubmission(int submissionId)
        {
            var submission = _dbContext.AssignmentSubmissions
                .Include(item => item.Assignment)
                    .ThenInclude(assignment => assignment.Course)
                .FirstOrDefault(item => item.SubmissionId == submissionId);
            if (submission is null ||
                GetOwnedCourse(submission.Assignment.CourseId) is null)
            {
                return NotFound();
            }

            var filePath = GetPrivateFilePath(
                "assignment-submissions",
                submission.FileUrl);
            if (filePath is null || !System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            return File(
                filePath,
                "application/octet-stream",
                Path.GetFileName(filePath));
        }

        private int? CurrentInstructorId()
        {
            var email = User.Identity?.Name;
            return string.IsNullOrWhiteSpace(email)
                ? null
                : _repository.GetInstructorByEmail(email)?.InstructorId;
        }

        private Course? GetOwnedCourse(int courseId)
        {
            var instructorId = CurrentInstructorId();
            var course = _repository.GetCourseById(courseId);
            return instructorId is not null &&
                course?.InstructorId == instructorId
                    ? course
                    : null;
        }

        private Quiz? GetOwnedQuiz(int quizId)
        {
            var quiz = _dbContext.Quizzes
                .Include(item => item.Course)
                .FirstOrDefault(item => item.QuizId == quizId);
            return quiz is not null &&
                GetOwnedCourse(quiz.CourseId) is not null
                    ? quiz
                    : null;
        }

        private static bool IsAllowedMaterialFile(
            string materialType,
            IFormFile? upload)
        {
            if (upload is null || upload.Length == 0)
            {
                return false;
            }

            var extension = Path.GetExtension(upload.FileName);
            return materialType switch
            {
                "Video" => extension.Equals(".mp4", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".webm", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".mov", StringComparison.OrdinalIgnoreCase),
                "PDF" => extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase),
                "Document" => extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".doc", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".docx", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".ppt", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".pptx", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".txt", StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }

        private async Task<string> SaveMaterialFileAsync(
            int courseId,
            IFormFile upload)
        {
            var extension = Path.GetExtension(upload.FileName).ToLowerInvariant();
            var storedName = $"{Guid.NewGuid():N}{extension}";
            var courseDirectory = Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                "course-materials",
                courseId.ToString());
            Directory.CreateDirectory(courseDirectory);

            var destination = Path.Combine(courseDirectory, storedName);
            await using (var fileStream = System.IO.File.Create(destination))
            {
                await upload.CopyToAsync(fileStream);
            }

            return Path.Combine("course-materials", courseId.ToString(), storedName)
                .Replace(Path.DirectorySeparatorChar, '/');
        }

        private string? GetPrivateFilePath(string folder, string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath) ||
                Path.IsPathRooted(relativePath))
            {
                return null;
            }

            var root = Path.GetFullPath(Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                folder));
            var path = Path.GetFullPath(Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                relativePath.Replace('/', Path.DirectorySeparatorChar)));
            return path.StartsWith(
                root + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase)
                    ? path
                    : null;
        }
    }
}