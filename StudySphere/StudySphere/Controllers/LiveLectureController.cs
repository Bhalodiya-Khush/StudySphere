using Microsoft.AspNetCore.Mvc;
using StudySphere.Repositories.Interfaces;

namespace StudySphere.Controllers
{
    public class LiveLectureController : Controller
    {
        private readonly IInstructorDashboardRepository _repository;

        public LiveLectureController(
            IInstructorDashboardRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Test(int lectureId)
        {
            var lecture =
                _repository.GetLiveLectureById(lectureId);

            if (lecture == null)
            {
                return NotFound();
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