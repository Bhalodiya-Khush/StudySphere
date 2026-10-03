using Microsoft.EntityFrameworkCore;
using StudySphere.Data;
using StudySphere.Models;
using StudySphere.Repositories.Interfaces;

namespace StudySphere.Repositories
{
    public class InstructorDashboardRepository : IInstructorDashboardRepository
    {
        private readonly StudySphereDbContext _dbContext;

        public InstructorDashboardRepository(StudySphereDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Instructor GetInstructor(int instructorId)
        {
            return _dbContext.Instructors
                .Include(instructor => instructor.User)
                .First(instructor => instructor.InstructorId == instructorId);
        }

        public Instructor? GetInstructorByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var normalizedEmail = email.Trim().ToLowerInvariant();
            return _dbContext.Instructors
                .Include(instructor => instructor.User)
                .FirstOrDefault(instructor =>
                    instructor.User.Email.ToLower() == normalizedEmail);
        }

        public List<Course> GetCourses(int instructorId)
        {
            return _dbContext.Courses
                .AsNoTracking()
                .Where(course => course.InstructorId == instructorId)
                .OrderByDescending(course => course.CreatedAt)
                .ToList();
        }

        public Course? GetCourseById(int courseId)
        {
            return _dbContext.Courses
                .Include(course => course.Instructor)
                    .ThenInclude(instructor => instructor.User)
                .FirstOrDefault(course => course.CourseId == courseId);
        }

        public void AddCourse(Course course)
        {
            _dbContext.Courses.Add(course);
            _dbContext.SaveChanges();
        }

        public void UpdateCourse(Course course)
        {
            var existing = _dbContext.Courses.FirstOrDefault(item =>
                item.CourseId == course.CourseId &&
                item.InstructorId == course.InstructorId);
            if (existing is null)
            {
                return;
            }

            existing.Title = course.Title;
            existing.Description = course.Description;
            existing.Category = course.Category;
            existing.Level = course.Level;
            existing.ThumbnailUrl = course.ThumbnailUrl;
            existing.Price = course.Price;
            existing.Status = "Pending";
            existing.UpdatedAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
        }

        public List<Material> GetMaterials(int courseId)
        {
            return _dbContext.Materials
                .AsNoTracking()
                .Where(material => material.CourseId == courseId)
                .OrderBy(material => material.DisplayOrder)
                .ThenBy(material => material.MaterialId)
                .ToList();
        }

        public Material? GetMaterialById(int materialId)
        {
            return _dbContext.Materials
                .Include(material => material.Course)
                .FirstOrDefault(material => material.MaterialId == materialId);
        }

        public void AddMaterial(Material material)
        {
            _dbContext.Materials.Add(material);
            _dbContext.SaveChanges();
        }

        public void UpdateMaterial(Material material)
        {
            var existing = _dbContext.Materials.FirstOrDefault(item =>
                item.MaterialId == material.MaterialId &&
                item.CourseId == material.CourseId);
            if (existing is null)
            {
                return;
            }

            existing.Title = material.Title;
            existing.Description = material.Description;
            existing.MaterialType = material.MaterialType;
            existing.FileUrl = material.FileUrl;
            existing.DurationInMinutes = material.DurationInMinutes;
            existing.DisplayOrder = material.DisplayOrder;
            existing.IsPublished = material.IsPublished;
            _dbContext.SaveChanges();
        }

        public void DeleteMaterial(int materialId)
        {
            var material = _dbContext.Materials.Find(materialId);
            if (material is null)
            {
                return;
            }

            _dbContext.Materials.Remove(material);
            _dbContext.SaveChanges();
        }

        public List<Enrollment> GetEnrollments(int courseId)
        {
            return _dbContext.Enrollments
                .AsNoTracking()
                .Include(enrollment => enrollment.Student)
                    .ThenInclude(student => student.User)
                .Where(enrollment => enrollment.CourseId == courseId)
                .OrderByDescending(enrollment => enrollment.EnrolledAt)
                .ToList();
        }

        public List<Announcement> GetAnnouncements(int courseId)
        {
            return _dbContext.Announcements
                .AsNoTracking()
                .Where(announcement => announcement.CourseId == courseId)
                .OrderByDescending(announcement => announcement.CreatedAt)
                .ToList();
        }

        public Announcement? GetAnnouncementById(int announcementId)
        {
            return _dbContext.Announcements
                .Include(announcement => announcement.Course)
                .FirstOrDefault(announcement =>
                    announcement.AnnouncementId == announcementId);
        }

        public void AddAnnouncement(Announcement announcement)
        {
            _dbContext.Announcements.Add(announcement);
            _dbContext.SaveChanges();
        }

        public void UpdateAnnouncement(Announcement announcement)
        {
            var existing = _dbContext.Announcements.FirstOrDefault(item =>
                item.AnnouncementId == announcement.AnnouncementId &&
                item.CourseId == announcement.CourseId &&
                item.InstructorId == announcement.InstructorId);
            if (existing is null)
            {
                return;
            }

            existing.Title = announcement.Title;
            existing.Message = announcement.Message;
            existing.IsPublished = announcement.IsPublished;
            _dbContext.SaveChanges();
        }

        public void DeleteAnnouncement(int announcementId)
        {
            var announcement = _dbContext.Announcements.Find(announcementId);
            if (announcement is null)
            {
                return;
            }

            _dbContext.Announcements.Remove(announcement);
            _dbContext.SaveChanges();
        }

        public List<LiveLecture> GetLiveLectures(int courseId)
        {
            return _dbContext.LiveLectures
                .AsNoTracking()
                .Where(lecture => lecture.CourseId == courseId)
                .OrderBy(lecture => lecture.StartTime)
                .ToList();
        }

        public LiveLecture? GetLiveLectureById(int liveLectureId)
        {
            return _dbContext.LiveLectures
                .Include(lecture => lecture.Course)
                .FirstOrDefault(lecture =>
                    lecture.LiveLectureId == liveLectureId);
        }

        public void AddLiveLecture(LiveLecture lecture)
        {
            _dbContext.LiveLectures.Add(lecture);
            _dbContext.SaveChanges();
        }

        public void UpdateLiveLecture(LiveLecture lecture)
        {
            var existing = _dbContext.LiveLectures.FirstOrDefault(item =>
                item.LiveLectureId == lecture.LiveLectureId &&
                item.CourseId == lecture.CourseId &&
                item.InstructorId == lecture.InstructorId);
            if (existing is null)
            {
                return;
            }

            existing.Title = lecture.Title;
            existing.Description = lecture.Description;
            existing.StartTime = lecture.StartTime;
            existing.EndTime = lecture.EndTime;
            existing.MeetingUrl = lecture.MeetingUrl;
            existing.Status = lecture.Status;
            _dbContext.SaveChanges();
        }

        public void DeleteLiveLecture(int liveLectureId)
        {
            var lecture = _dbContext.LiveLectures.Find(liveLectureId);
            if (lecture is null)
            {
                return;
            }

            _dbContext.LiveLectures.Remove(lecture);
            _dbContext.SaveChanges();
        }

        public int GetTotalStudents(int instructorId)
        {
            return _dbContext.Enrollments
                .Where(enrollment =>
                    enrollment.Course.InstructorId == instructorId &&
                    enrollment.Status == "Active")
                .Select(enrollment => enrollment.StudentId)
                .Distinct()
                .Count();
        }
    }
}
