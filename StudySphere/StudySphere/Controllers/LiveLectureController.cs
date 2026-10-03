using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudySphere.Repositories.Interfaces;

namespace StudySphere.Controllers
{
    [Authorize(Roles = "Student,Instructor")]
    public class LiveLectureController : Controller
    {
        private readonly IInstructorDashboardRepository _repository;
        private readonly IStudentDashboardRepository _studentRepository;

        public LiveLectureController(
            IInstructorDashboardRepository repository,
            IStudentDashboardRepository studentRepository)
        {
            _repository = repository;
            _studentRepository = studentRepository;
        }

        public IActionResult Test(int lectureId)
        {
            var lecture =
                _repository.GetLiveLectureById(lectureId);

            if (lecture == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Student"))
            {
                var email = User.Identity?.Name;
                var student = string.IsNullOrWhiteSpace(email)
                    ? null
                    : _studentRepository.GetStudentByEmail(email);
                if (student is null ||
                    !_studentRepository.IsEnrolled(
                        student.StudentId,
                        lecture.CourseId))
                {
                    return Forbid();
                }

                if (lecture.Status != "Live")
                {
                    return NotFound();
                }
            }
            else
            {
                var email = User.Identity?.Name;
                var instructor = string.IsNullOrWhiteSpace(email)
                    ? null
                    : _repository.GetInstructorByEmail(email);
                if (instructor is null ||
                    instructor.InstructorId != lecture.InstructorId)
                {
                    return Forbid();
                }
            }

            ViewBag.LectureId =
                lecture.LiveLectureId;

            ViewBag.CourseId =
                lecture.CourseId;

            ViewBag.LectureTitle =
                lecture.Title;

            return View();
        }
    }
}