using StudySphere.Models;

namespace StudySphere.Repositories.Interfaces
{
    public interface IStudentDashboardRepository
    {
        List<Course> GetAllCourses();

        List<Course> SearchCourses(
            string? keyword,
            string? category,
            string? level,
            decimal? minimumPrice,
            decimal? maximumPrice);

        List<Course> GetEnrolledCourses(int studentId);

        List<string> GetCategories();

        Course? GetCourseById(int id);

        List<LiveLecture> GetLiveLectures(int courseId);

        List<Material> GetPublishedMaterials(int courseId);

        Material? GetMaterialById(int materialId);

        List<Course> GetCoursesByCategory(string category);

        Student? GetStudentByEmail(string email);

        bool IsEnrolled(int studentId, int courseId);

        bool TryEnroll(int studentId, int courseId);
    }
}