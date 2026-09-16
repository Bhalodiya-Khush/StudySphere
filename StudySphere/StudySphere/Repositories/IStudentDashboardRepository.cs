using StudySphere.Models;

namespace StudySphere.Repositories.Interfaces
{
    public interface IStudentDashboardRepository
    {
        List<Course> GetAllCourses();

        List<Course> GetEnrolledCourses();

        List<string> GetCategories();

        Course? GetCourseById(int id);

        List<Course> GetCoursesByCategory(string category);
    }
}