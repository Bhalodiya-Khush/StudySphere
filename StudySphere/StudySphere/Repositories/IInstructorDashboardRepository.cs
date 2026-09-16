using StudySphere.Models;

namespace StudySphere.Repositories.Interfaces
{
    public interface IInstructorDashboardRepository
    {
        // =========================================================
        // INSTRUCTOR
        // =========================================================

        Instructor GetInstructor(int instructorId);


        // =========================================================
        // COURSES
        // =========================================================

        List<Course> GetCourses(int instructorId);

        Course? GetCourseById(int courseId);

        void AddCourse(Course course);

        void UpdateCourse(Course course);


        // =========================================================
        // MATERIALS
        // =========================================================

        List<Material> GetMaterials(int courseId);

        Material? GetMaterialById(int materialId);

        void AddMaterial(Material material);

        void UpdateMaterial(Material material);

        void DeleteMaterial(int materialId);


        // =========================================================
        // ENROLLMENTS / STUDENTS
        // =========================================================

        List<Enrollment> GetEnrollments(int courseId);


        // =========================================================
        // ANNOUNCEMENTS
        // =========================================================

        List<Announcement> GetAnnouncements(int courseId);

        Announcement? GetAnnouncementById(int announcementId);

        void AddAnnouncement(Announcement announcement);

        void UpdateAnnouncement(Announcement announcement);

        void DeleteAnnouncement(int announcementId);


        // =========================================================
        // LIVE LECTURES
        // =========================================================

        List<LiveLecture> GetLiveLectures(int courseId);

        LiveLecture? GetLiveLectureById(int liveLectureId);

        void AddLiveLecture(LiveLecture lecture);

        void UpdateLiveLecture(LiveLecture lecture);

        void DeleteLiveLecture(int liveLectureId);


        // =========================================================
        // DASHBOARD
        // =========================================================

        int GetTotalStudents(int instructorId);
    }
}