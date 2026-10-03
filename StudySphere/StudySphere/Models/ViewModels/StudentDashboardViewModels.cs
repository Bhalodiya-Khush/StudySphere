using StudySphere.Models;

namespace StudySphere.Models.ViewModels
{
    public class StudentDashboardViewModel
    {
        public string StudentName { get; set; } = string.Empty;

        public List<Course> EnrolledCourses { get; set; } = new();

        public List<Course> AllCourses { get; set; } = new();

        public List<string> Categories { get; set; } = new();

        public Dictionary<int, decimal> CourseProgress { get; set; } = new();

        public decimal OverallProgress { get; set; }

        public int CompletedCourses { get; set; }

        public int UpcomingAssignments { get; set; }
    }
}