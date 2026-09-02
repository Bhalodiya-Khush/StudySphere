using StudySphere.Models;

namespace StudySphere.Models.ViewModels
{
    public class StudentDashboardViewModel
    {
        public string StudentName { get; set; }

        public List<Course> EnrolledCourses { get; set; }

        public List<Course> AllCourses { get; set; }

        public List<string> Categories { get; set; }
    }
}