using StudySphere.Models;

namespace StudySphere.Repositories.Interfaces
{
    public interface IAdminDashboardRepository
    {
        // Students
        List<Student> GetAllStudents();
        Student? GetStudentById(int id);

        // Instructors
        List<Instructor> GetAllInstructors();
        Instructor? GetInstructorById(int id);

        // Courses
        List<Course> GetAllCourses();
        Course? GetCourseById(int id);
        List<Course> GetPendingCourses();
        List<Course> GetApprovedCourses();
        List<Course> GetRejectedCourses();
        List<Course> GetCoursesByInstructorId(int instructorId);

        // Enrollments
        List<Enrollment> GetAllEnrollments();
        List<Enrollment> GetEnrollmentsByStudentId(int studentId);
        List<Enrollment> GetEnrollmentsByCourseId(int courseId);

        // Course actions
        bool ApproveCourse(int id);
        bool RejectCourse(int id);
    }
}