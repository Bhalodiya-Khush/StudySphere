using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySphere.Data;
using StudySphere.Models;
using StudySphere.Models.ViewModels;
using StudySphere.Repositories.Interfaces;
using System.Text.Json;

namespace StudySphere.Controllers
{
    public class StudentDashboardController : Controller
    {
        private readonly IStudentDashboardRepository _repository;
        private readonly IWebHostEnvironment _environment;
        private readonly StudySphereDbContext _dbContext;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public StudentDashboardController(
            IStudentDashboardRepository repository,
            IWebHostEnvironment environment,
            StudySphereDbContext dbContext)
        {
            _repository = repository;
            _environment = environment;
            _dbContext = dbContext;
        }


        // =========================================================
        // STUDENT DASHBOARD
        // =========================================================

        [Authorize(Roles = "Student")]
        public IActionResult Index()
        {
            var student = CurrentStudent();
            if (student is null)
            {
                return Forbid();
            }

            var model = new StudentDashboardViewModel
            {
                StudentName = student.User.FullName,

                EnrolledCourses =
                    _repository.GetEnrolledCourses(student.StudentId),

                AllCourses =
                    _repository.GetAllCourses(),

                Categories =
                    _repository.GetCategories()
            };

            return View(model);
        }


        // =========================================================
        // MY COURSES
        // =========================================================

        [Authorize(Roles = "Student")]
        public IActionResult MyCourses()
        {
            var student = CurrentStudent();
            if (student is null)
            {
                return Forbid();
            }

            var enrolledCourses =
                _repository.GetEnrolledCourses(student.StudentId);

            return View(enrolledCourses);
        }


        // =========================================================
        // ALL COURSES
        // =========================================================

        public IActionResult AllCourses(
            string? search,
            string? category,
            string? level,
            decimal? minimumPrice,
            decimal? maximumPrice)
        {
            var allCourses = _repository.SearchCourses(
                search,
                category,
                level,
                minimumPrice,
                maximumPrice);

            ViewBag.Categories = _repository.GetCategories();
            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.Level = level;
            ViewBag.MinimumPrice = minimumPrice;
            ViewBag.MaximumPrice = maximumPrice;
            return View(allCourses);
        }


        // =========================================================
        // COURSES BY CATEGORY
        // =========================================================

        public IActionResult CategoryCourses(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return RedirectToAction(nameof(AllCourses));
            }

            var courses =
                _repository.GetCoursesByCategory(category);

            ViewBag.Category = category;

            return View(courses);
        }


        // =========================================================
        // COURSE DETAILS
        // =========================================================

        // =========================================================
        // COURSE DETAILS
        // =========================================================

        public IActionResult CourseDetails(int id)
        {
            var course =
                _repository.GetCourseById(id);

            if (course == null)
            {
                return NotFound();
            }

            // Get live lectures for this course
            var liveLectures =
                new List<LiveLecture>();
            var completedMaterialIds = new HashSet<int>();
            var courseProgress = 0m;
            var completedCourse = false;

            var student = User.IsInRole("Student")
                ? CurrentStudent()
                : null;
            var isEnrolled = student is not null &&
                _repository.IsEnrolled(student.StudentId, id);

            if (isEnrolled)
            {
                liveLectures = _repository.GetLiveLectures(id);
                var publishedMaterials = _repository.GetPublishedMaterials(id);
                var materialIds = publishedMaterials
                    .Select(material => material.MaterialId)
                    .ToList();
                completedMaterialIds = _dbContext.LessonProgress
                    .Where(progress =>
                        progress.StudentId == student!.StudentId &&
                        materialIds.Contains(progress.MaterialId))
                    .Select(progress => progress.MaterialId)
                    .ToHashSet();
                courseProgress = materialIds.Count == 0
                    ? 0
                    : Math.Round(
                        completedMaterialIds.Count * 100m / materialIds.Count,
                        2);
                var enrollment = _dbContext.Enrollments.FirstOrDefault(item =>
                    item.StudentId == student!.StudentId &&
                    item.CourseId == id &&
                    item.Status != "Withdrawn");
                completedCourse = enrollment?.Status == "Completed";
            }

            ViewBag.LiveLectures = liveLectures;
            ViewBag.Materials = isEnrolled
                ? _repository.GetPublishedMaterials(id)
                : new List<Material>();
            ViewBag.IsEnrolled = isEnrolled;
            ViewBag.CompletedMaterialIds = completedMaterialIds;
            ViewBag.CourseProgress = courseProgress;
            ViewBag.CourseCompleted = completedCourse;
            ViewBag.CertificateAvailable = isEnrolled &&
                _dbContext.CourseCertificates.Any(certificate =>
                    certificate.StudentId == student!.StudentId &&
                    certificate.CourseId == id);

            return View(course);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Enroll(int courseId)
        {
            var student = CurrentStudent();
            if (student is null)
            {
                return Forbid();
            }

            if (!_repository.TryEnroll(student.StudentId, courseId))
            {
                if (_repository.GetCourseById(courseId) is null)
                {
                    return NotFound();
                }

                TempData["EnrollmentMessage"] =
                    "You are already enrolled in this course.";
                return RedirectToAction(nameof(CourseDetails), new { id = courseId });
            }

            TempData["EnrollmentMessage"] =
                "You are now enrolled and can access the course materials.";
            return RedirectToAction(nameof(CourseDetails), new { id = courseId });
        }

        [Authorize(Roles = "Student")]
        public IActionResult DownloadMaterial(int id)
        {
            var student = CurrentStudent();
            var material = _repository.GetMaterialById(id);
            if (student is null)
            {
                return Forbid();
            }

            if (material is null || !material.IsPublished)
            {
                return NotFound();
            }

            if (!_repository.IsEnrolled(student.StudentId, material.CourseId))
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(material.FileUrl) ||
                Path.IsPathRooted(material.FileUrl))
            {
                return NotFound();
            }

            var materialsRoot = Path.GetFullPath(Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                "course-materials"));
            var relativePath = material.FileUrl.Replace(
                '/',
                Path.DirectorySeparatorChar);
            var filePath = Path.GetFullPath(Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                relativePath));
            if (!filePath.StartsWith(
                    materialsRoot + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase) ||
                !System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            return File(
                filePath,
                GetMaterialContentType(filePath),
                Path.GetFileName(filePath));
        }

        [Authorize(Roles = "Student")]
        public IActionResult Assignments(int courseId)
        {
            var student = CurrentStudent();
            if (student is null)
            {
                return Forbid();
            }

            if (!_repository.IsEnrolled(student.StudentId, courseId))
            {
                return Forbid();
            }

            var course = _repository.GetCourseById(courseId);
            if (course is null)
            {
                return NotFound();
            }

            var assignments = _dbContext.Assignments
                .AsNoTracking()
                .Where(assignment =>
                    assignment.CourseId == courseId &&
                    assignment.IsPublished)
                .Include(assignment => assignment.Submissions
                    .Where(submission =>
                        submission.StudentId == student.StudentId))
                .OrderBy(assignment => assignment.DueAt)
                .ToList();
            ViewBag.Course = course;
            return View(assignments);
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public IActionResult SubmitAssignment(int assignmentId)
        {
            var student = CurrentStudent();
            if (student is null)
            {
                return Forbid();
            }

            var assignment = GetStudentAssignment(assignmentId);
            if (assignment is null)
            {
                return NotFound();
            }

            if (!_repository.IsEnrolled(student.StudentId, assignment.CourseId))
            {
                return Forbid();
            }

            ViewBag.Submission = _dbContext.AssignmentSubmissions
                .AsNoTracking()
                .FirstOrDefault(submission =>
                    submission.AssignmentId == assignmentId &&
                    submission.StudentId == student.StudentId);
            return View(assignment);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(26_214_400)]
        [RequestFormLimits(MultipartBodyLengthLimit = 26_214_400)]
        public async Task<IActionResult> SubmitAssignment(
            int assignmentId,
            IFormFile? upload,
            string? studentComment)
        {
            var student = CurrentStudent();
            if (student is null)
            {
                return Forbid();
            }

            var assignment = GetStudentAssignment(assignmentId);
            if (assignment is null)
            {
                return NotFound();
            }

            if (!_repository.IsEnrolled(student.StudentId, assignment.CourseId))
            {
                return Forbid();
            }

            var existing = _dbContext.AssignmentSubmissions
                .FirstOrDefault(submission =>
                    submission.AssignmentId == assignmentId &&
                    submission.StudentId == student.StudentId);

            if (assignment.DueAt <= DateTime.UtcNow)
            {
                ModelState.AddModelError(
                    nameof(upload),
                    "The submission deadline has passed.");
            }

            if (existing?.MarksAwarded is not null)
            {
                ModelState.AddModelError(
                    nameof(upload),
                    "This submission has already been graded and cannot be replaced.");
            }

            if (upload is null || upload.Length == 0)
            {
                ModelState.AddModelError(nameof(upload), "Choose a file to submit.");
            }
            else if (upload.Length > 25L * 1024 * 1024)
            {
                ModelState.AddModelError(nameof(upload), "The maximum file size is 25 MB.");
            }
            else if (!IsAllowedAssignmentFile(upload.FileName))
            {
                ModelState.AddModelError(
                    nameof(upload),
                    "Upload a PDF, Word document, or plain text file.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Submission = existing;
                return View(assignment);
            }

            var extension = Path.GetExtension(upload!.FileName).ToLowerInvariant();
            var storedName = $"{Guid.NewGuid():N}{extension}";
            var submissionDirectory = Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                "assignment-submissions",
                assignment.CourseId.ToString());
            Directory.CreateDirectory(submissionDirectory);
            var destination = Path.Combine(submissionDirectory, storedName);
            await using (var fileStream = System.IO.File.Create(destination))
            {
                await upload.CopyToAsync(fileStream);
            }

            var relativePath = Path.Combine(
                "assignment-submissions",
                assignment.CourseId.ToString(),
                storedName).Replace(Path.DirectorySeparatorChar, '/');
            if (existing is null)
            {
                existing = new AssignmentSubmission
                {
                    AssignmentId = assignmentId,
                    StudentId = student.StudentId
                };
                _dbContext.AssignmentSubmissions.Add(existing);
            }

            existing.FileUrl = relativePath;
            existing.StudentComment = studentComment?.Trim();
            existing.SubmittedAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
            TempData["AssignmentMessage"] = "Your assignment was submitted.";
            return RedirectToAction(
                nameof(Assignments),
                new { courseId = assignment.CourseId });
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CompleteLesson(int materialId)
        {
            var student = CurrentStudent();
            if (student is null)
            {
                return Forbid();
            }

            var material = _repository.GetMaterialById(materialId);
            if (material is null || !material.IsPublished)
            {
                return NotFound();
            }

            if (!_repository.IsEnrolled(student.StudentId, material.CourseId))
            {
                return Forbid();
            }

            var progress = _dbContext.LessonProgress.FirstOrDefault(item =>
                item.StudentId == student.StudentId &&
                item.MaterialId == materialId);
            var isNewProgress = progress is null;
            if (isNewProgress)
            {
                _dbContext.LessonProgress.Add(new LessonProgress
                {
                    StudentId = student.StudentId,
                    MaterialId = materialId,
                    CompletedAt = DateTime.UtcNow
                });
            }

            var totalLessons = _dbContext.Materials.Count(item =>
                item.CourseId == material.CourseId &&
                item.IsPublished);
            var completedLessons = _dbContext.LessonProgress.Count(item =>
                item.StudentId == student.StudentId &&
                item.Material.CourseId == material.CourseId &&
                item.Material.IsPublished);
            if (isNewProgress)
            {
                completedLessons++;
            }

            var enrollment = _dbContext.Enrollments.FirstOrDefault(item =>
                item.StudentId == student.StudentId &&
                item.CourseId == material.CourseId &&
                item.Status != "Withdrawn");
            if (enrollment is null)
            {
                return Forbid();
            }

            enrollment.Progress = totalLessons == 0
                ? 0
                : Math.Round(completedLessons * 100m / totalLessons, 2);
            if (totalLessons > 0 && completedLessons >= totalLessons)
            {
                enrollment.Status = "Completed";
                enrollment.Progress = 100;
                enrollment.CompletedAt ??= DateTime.UtcNow;
                if (!_dbContext.CourseCertificates.Any(certificate =>
                        certificate.StudentId == student.StudentId &&
                        certificate.CourseId == material.CourseId))
                {
                    _dbContext.CourseCertificates.Add(new CourseCertificate
                    {
                        StudentId = student.StudentId,
                        CourseId = material.CourseId,
                        CertificateCode = Guid.NewGuid().ToString("N"),
                        IssuedAt = DateTime.UtcNow
                    });
                }
            }

            _dbContext.SaveChanges();
            TempData["ProgressMessage"] = "Lesson completion saved.";
            return RedirectToAction(
                nameof(CourseDetails),
                new { id = material.CourseId });
        }

        [Authorize(Roles = "Student")]
        public IActionResult Certificate(int courseId)
        {
            var student = CurrentStudent();
            if (student is null)
            {
                return Forbid();
            }

            var certificate = _dbContext.CourseCertificates
                .AsNoTracking()
                .Include(item => item.Student)
                    .ThenInclude(item => item.User)
                .Include(item => item.Course)
                .FirstOrDefault(item =>
                    item.StudentId == student.StudentId &&
                    item.CourseId == courseId);
            return certificate is null ? NotFound() : View(certificate);
        }

        [Authorize(Roles = "Student")]
        public IActionResult Quizzes(int courseId)
        {
            var student = CurrentStudent();
            if (student is null)
            {
                return Forbid();
            }

            if (!_repository.IsEnrolled(student.StudentId, courseId))
            {
                return Forbid();
            }

            var course = _repository.GetCourseById(courseId);
            if (course is null)
            {
                return NotFound();
            }

            var now = DateTime.UtcNow;
            var quizzes = _dbContext.Quizzes
                .AsNoTracking()
                .Where(quiz =>
                    quiz.CourseId == courseId &&
                    quiz.IsPublished &&
                    quiz.OpensAt <= now &&
                    (!quiz.DueAt.HasValue || quiz.DueAt > now))
                .Include(quiz => quiz.Attempts
                    .Where(attempt => attempt.StudentId == student.StudentId))
                .OrderBy(quiz => quiz.DueAt)
                .ToList();
            ViewBag.Course = course;
            return View(quizzes);
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public IActionResult AttemptQuiz(int quizId)
        {
            var student = CurrentStudent();
            if (student is null)
            {
                return Forbid();
            }

            var quiz = GetAvailableQuiz(quizId);
            if (quiz is null)
            {
                return NotFound();
            }

            if (!_repository.IsEnrolled(student.StudentId, quiz.CourseId))
            {
                return Forbid();
            }

            var attempts = _dbContext.QuizAttempts.Count(attempt =>
                attempt.QuizId == quizId &&
                attempt.StudentId == student.StudentId);
            if (attempts >= quiz.MaxAttempts)
            {
                return RedirectToAction(nameof(QuizResultHistory), new { quizId });
            }

            return View(CreateQuizAttemptViewModel(quiz));
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitQuiz(QuizAttemptViewModel model)
        {
            var student = CurrentStudent();
            if (student is null)
            {
                return Forbid();
            }

            var quiz = GetAvailableQuiz(model.QuizId);
            if (quiz is null)
            {
                return NotFound();
            }

            if (!_repository.IsEnrolled(student.StudentId, quiz.CourseId))
            {
                return Forbid();
            }

            var attempts = _dbContext.QuizAttempts.Count(attempt =>
                attempt.QuizId == quiz.QuizId &&
                attempt.StudentId == student.StudentId);
            if (attempts >= quiz.MaxAttempts)
            {
                return Conflict("The maximum number of quiz attempts has been reached.");
            }

            var validQuestionIds = quiz.Questions
                .Select(question => question.QuizQuestionId)
                .ToHashSet();
            var answers = model.Answers ?? new Dictionary<int, string>();
            var invalidAnswer = answers.Any(answer =>
                !validQuestionIds.Contains(answer.Key) ||
                answer.Value is not ("A" or "B" or "C" or "D"));
            if (invalidAnswer || answers.Count != validQuestionIds.Count)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Answer every question using one of the provided options.");
                var viewModel = CreateQuizAttemptViewModel(quiz);
                viewModel.Answers = answers;
                return View("AttemptQuiz", viewModel);
            }

            var score = quiz.Questions
                .Where(question =>
                    answers.TryGetValue(question.QuizQuestionId, out var answer) &&
                    answer == question.CorrectOption)
                .Sum(question => question.Marks);
            var maximumScore = quiz.Questions.Sum(question => question.Marks);
            var attempt = new QuizAttempt
            {
                QuizId = quiz.QuizId,
                StudentId = student.StudentId,
                AttemptNumber = attempts + 1,
                Score = score,
                MaxScore = maximumScore,
                AnswersJson = JsonSerializer.Serialize(answers),
                StartedAt = DateTime.UtcNow,
                SubmittedAt = DateTime.UtcNow
            };
            _dbContext.QuizAttempts.Add(attempt);
            _dbContext.SaveChanges();
            return RedirectToAction(
                nameof(QuizResult),
                new { attemptId = attempt.QuizAttemptId });
        }

        [Authorize(Roles = "Student")]
        public IActionResult QuizResult(int attemptId)
        {
            var student = CurrentStudent();
            if (student is null)
            {
                return Forbid();
            }

            var attempt = _dbContext.QuizAttempts
                .AsNoTracking()
                .Include(item => item.Quiz)
                .FirstOrDefault(item =>
                    item.QuizAttemptId == attemptId &&
                    item.StudentId == student.StudentId);
            return attempt is null ? NotFound() : View(attempt);
        }

        [Authorize(Roles = "Student")]
        public IActionResult QuizResultHistory(int quizId)
        {
            var student = CurrentStudent();
            if (student is null)
            {
                return Forbid();
            }

            var quiz = _dbContext.Quizzes.FirstOrDefault(item =>
                item.QuizId == quizId);
            if (quiz is null ||
                !_repository.IsEnrolled(student.StudentId, quiz.CourseId))
            {
                return NotFound();
            }

            var attempts = _dbContext.QuizAttempts
                .AsNoTracking()
                .Where(attempt =>
                    attempt.QuizId == quizId &&
                    attempt.StudentId == student.StudentId)
                .OrderByDescending(attempt => attempt.SubmittedAt)
                .ToList();
            ViewBag.Quiz = quiz;
            return View(attempts);
        }

        private Student? CurrentStudent()
        {
            var email = User.Identity?.Name;
            return string.IsNullOrWhiteSpace(email)
                ? null
                : _repository.GetStudentByEmail(email);
        }

        private Assignment? GetStudentAssignment(int assignmentId)
        {
            return _dbContext.Assignments
                .AsNoTracking()
                .FirstOrDefault(assignment =>
                    assignment.AssignmentId == assignmentId &&
                    assignment.IsPublished);
        }

        private static bool IsAllowedAssignmentFile(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".doc", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".docx", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".txt", StringComparison.OrdinalIgnoreCase);
        }

        private Quiz? GetAvailableQuiz(int quizId)
        {
            var now = DateTime.UtcNow;
            return _dbContext.Quizzes
                .Include(quiz => quiz.Questions)
                .FirstOrDefault(quiz =>
                    quiz.QuizId == quizId &&
                    quiz.IsPublished &&
                    quiz.OpensAt <= now &&
                    (!quiz.DueAt.HasValue || quiz.DueAt > now) &&
                    quiz.Questions.Any());
        }

        private static QuizAttemptViewModel CreateQuizAttemptViewModel(Quiz quiz)
        {
            return new QuizAttemptViewModel
            {
                QuizId = quiz.QuizId,
                Title = quiz.Title,
                Description = quiz.Description,
                Questions = quiz.Questions
                    .OrderBy(question => question.QuizQuestionId)
                    .Select(question => new QuizAttemptQuestionViewModel
                    {
                        QuizQuestionId = question.QuizQuestionId,
                        Prompt = question.Prompt,
                        Marks = question.Marks,
                        OptionA = question.OptionA,
                        OptionB = question.OptionB,
                        OptionC = question.OptionC,
                        OptionD = question.OptionD
                    })
                    .ToList()
            };
        }

        private static string GetMaterialContentType(string filePath)
        {
            return Path.GetExtension(filePath).ToLowerInvariant() switch
            {
                ".mp4" => "video/mp4",
                ".webm" => "video/webm",
                ".mov" => "video/quicktime",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}