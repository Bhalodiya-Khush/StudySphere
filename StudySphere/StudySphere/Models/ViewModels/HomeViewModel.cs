using StudySphere.Models;

namespace StudySphere.Models.ViewModels
{
    public class HomeViewModel
    {
        public int CourseCount { get; set; }

        public int InstructorCount { get; set; }

        public int StudentCount { get; set; }

        public List<Course> Courses { get; set; } = new();

        public List<CourseCategoryViewModel> Categories { get; set; } = new();
    }

    public class CourseCategoryViewModel
    {
        public string Name { get; set; } = string.Empty;

        public int CourseCount { get; set; }
    }
}
