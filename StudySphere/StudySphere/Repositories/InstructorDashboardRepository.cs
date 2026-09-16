using StudySphere.Models;
using StudySphere.Repositories.Interfaces;

namespace StudySphere.Repositories
{
    public class InstructorDashboardRepository
        : IInstructorDashboardRepository
    {
        // =========================================================
        // STATIC DATA
        // =========================================================

        private readonly Instructor _instructor;

        private readonly List<Course> _courses;

        private readonly List<Material> _materials;

        private readonly List<Enrollment> _enrollments;

        private readonly List<Announcement> _announcements;

        private readonly List<LiveLecture> _liveLectures;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public InstructorDashboardRepository()
        {
            // -----------------------------------------------------
            // Instructor
            // -----------------------------------------------------

            _instructor = new Instructor
            {
                InstructorId = 1,
                ProfessionalTitle = "Senior Software Developer",
                AreaOfExpertise = "Web Development",
                Qualification = "B.Tech Computer Engineering",
                Bio = "Experienced instructor in web and backend development."
            };


            // -----------------------------------------------------
            // Courses
            // -----------------------------------------------------

            _courses = new List<Course>
            {
                new Course
                {
                    CourseId = 1,
                    InstructorId = 1,
                    Title = "ASP.NET Core MVC",
                    Description = "Learn ASP.NET Core MVC from basic to advanced concepts.",
                    Category = "Web Development",
                    Level = "Intermediate",
                    ThumbnailUrl = "/images/courses/aspnet.jpg",
                    Price = 999,
                    Status = "Published",
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                },

                new Course
                {
                    CourseId = 2,
                    InstructorId = 1,
                    Title = "Java Spring Boot",
                    Description = "Learn backend development using Java and Spring Boot.",
                    Category = "Backend Development",
                    Level = "Intermediate",
                    ThumbnailUrl = "/images/courses/java.jpg",
                    Price = 1199,
                    Status = "Published",
                    CreatedAt = DateTime.UtcNow.AddDays(-25)
                },

                new Course
                {
                    CourseId = 3,
                    InstructorId = 1,
                    Title = "Database Management",
                    Description = "Learn SQL and database management concepts.",
                    Category = "Database",
                    Level = "Beginner",
                    ThumbnailUrl = "/images/courses/database.jpg",
                    Price = 799,
                    Status = "Published",
                    CreatedAt = DateTime.UtcNow.AddDays(-20)
                },

                new Course
                {
                    CourseId = 4,
                    InstructorId = 1,
                    Title = "Web Development",
                    Description = "Learn HTML, CSS and JavaScript for modern web development.",
                    Category = "Web Development",
                    Level = "Beginner",
                    ThumbnailUrl = "/images/courses/web.jpg",
                    Price = 699,
                    Status = "Draft",
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                }
            };


            // -----------------------------------------------------
            // Materials
            // -----------------------------------------------------

            _materials = new List<Material>
            {
                new Material
                {
                    MaterialId = 1,
                    CourseId = 1,
                    Title = "Introduction to ASP.NET Core MVC",
                    Description = "Introduction to MVC architecture.",
                    MaterialType = "Video",
                    FileUrl = "/materials/aspnet/introduction.mp4",
                    DurationInMinutes = 25,
                    DisplayOrder = 1,
                    IsPublished = true,
                    UploadedAt = DateTime.UtcNow.AddDays(-20)
                },

                new Material
                {
                    MaterialId = 2,
                    CourseId = 1,
                    Title = "MVC Architecture Notes",
                    Description = "PDF notes about MVC architecture.",
                    MaterialType = "PDF",
                    FileUrl = "/materials/aspnet/mvc-notes.pdf",
                    DurationInMinutes = null,
                    DisplayOrder = 2,
                    IsPublished = true,
                    UploadedAt = DateTime.UtcNow.AddDays(-18)
                },

                new Material
                {
                    MaterialId = 3,
                    CourseId = 1,
                    Title = "Controllers in ASP.NET Core",
                    Description = "Understanding controllers and actions.",
                    MaterialType = "Video",
                    FileUrl = "/materials/aspnet/controllers.mp4",
                    DurationInMinutes = 30,
                    DisplayOrder = 3,
                    IsPublished = true,
                    UploadedAt = DateTime.UtcNow.AddDays(-15)
                },

                new Material
                {
                    MaterialId = 4,
                    CourseId = 2,
                    Title = "Spring Boot Introduction",
                    Description = "Introduction to Spring Boot.",
                    MaterialType = "Video",
                    FileUrl = "/materials/java/spring-introduction.mp4",
                    DurationInMinutes = 35,
                    DisplayOrder = 1,
                    IsPublished = true,
                    UploadedAt = DateTime.UtcNow.AddDays(-12)
                },

                new Material
                {
                    MaterialId = 5,
                    CourseId = 2,
                    Title = "Spring Boot Notes",
                    Description = "Spring Boot study material.",
                    MaterialType = "PDF",
                    FileUrl = "/materials/java/spring-notes.pdf",
                    DurationInMinutes = null,
                    DisplayOrder = 2,
                    IsPublished = false,
                    UploadedAt = DateTime.UtcNow.AddDays(-10)
                }
            };


            // -----------------------------------------------------
            // Enrollments
            // -----------------------------------------------------

            _enrollments = new List<Enrollment>
            {
                new Enrollment
                {
                    EnrollmentId = 1,
                    StudentId = 1,
                    CourseId = 1,
                    EnrolledAt = DateTime.UtcNow.AddDays(-20),
                    Status = "Active",
                    Progress = 75
                },

                new Enrollment
                {
                    EnrollmentId = 2,
                    StudentId = 2,
                    CourseId = 1,
                    EnrolledAt = DateTime.UtcNow.AddDays(-15),
                    Status = "Active",
                    Progress = 50
                },

                new Enrollment
                {
                    EnrollmentId = 3,
                    StudentId = 3,
                    CourseId = 1,
                    EnrolledAt = DateTime.UtcNow.AddDays(-10),
                    Status = "Completed",
                    Progress = 100,
                    CompletedAt = DateTime.UtcNow.AddDays(-2)
                },

                new Enrollment
                {
                    EnrollmentId = 4,
                    StudentId = 4,
                    CourseId = 2,
                    EnrolledAt = DateTime.UtcNow.AddDays(-8),
                    Status = "Active",
                    Progress = 40
                }
            };


            // -----------------------------------------------------
            // Announcements
            // -----------------------------------------------------

            _announcements = new List<Announcement>
            {
                new Announcement
                {
                    AnnouncementId = 1,
                    CourseId = 1,
                    InstructorId = 1,
                    Title = "Welcome to ASP.NET Core MVC",
                    Message = "Welcome everyone. Please complete the first module this week.",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    IsPublished = true
                },

                new Announcement
                {
                    AnnouncementId = 2,
                    CourseId = 1,
                    InstructorId = 1,
                    Title = "Assignment 1",
                    Message = "Assignment 1 has been uploaded. Submit it before the deadline.",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    IsPublished = true
                },

                new Announcement
                {
                    AnnouncementId = 3,
                    CourseId = 2,
                    InstructorId = 1,
                    Title = "Spring Boot Practice",
                    Message = "Practice the REST API examples covered in the latest lecture.",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    IsPublished = true
                }
            };


            // -----------------------------------------------------
            // Live Lectures
            // -----------------------------------------------------

            _liveLectures = new List<LiveLecture>
            {
                new LiveLecture
                {
                    LiveLectureId = 1,
                    CourseId = 1,
                    InstructorId = 1,
                    Title = "ASP.NET Core MVC - Controllers",
                    Description = "Live session about controllers and routing.",
                    StartTime = DateTime.Now.AddDays(1).AddHours(2),
                    EndTime = DateTime.Now.AddDays(1).AddHours(3),
                    MeetingUrl = "https://meet.example.com/aspnet",
                    Status = "Scheduled",
                    CreatedAt = DateTime.UtcNow
                },

                new LiveLecture
                {
                    LiveLectureId = 2,
                    CourseId = 2,
                    InstructorId = 1,
                    Title = "Spring Boot REST API",
                    Description = "Live session about REST API development.",
                    StartTime = DateTime.Now.AddDays(2).AddHours(3),
                    EndTime = DateTime.Now.AddDays(2).AddHours(4),
                    MeetingUrl = "https://meet.example.com/spring",
                    Status = "Scheduled",
                    CreatedAt = DateTime.UtcNow
                }
            };
        }


        // =========================================================
        // INSTRUCTOR
        // =========================================================

        public Instructor GetInstructor(int instructorId)
        {
            return _instructor;
        }


        // =========================================================
        // COURSES
        // =========================================================

        public List<Course> GetCourses(int instructorId)
        {
            return _courses
                .Where(x => x.InstructorId == instructorId)
                .ToList();
        }


        public Course? GetCourseById(int courseId)
        {
            return _courses
                .FirstOrDefault(x => x.CourseId == courseId);
        }


        public void AddCourse(Course course)
        {
            course.CourseId = _courses.Count == 0
                ? 1
                : _courses.Max(x => x.CourseId) + 1;

            _courses.Add(course);
        }


        public void UpdateCourse(Course course)
        {
            var existingCourse = GetCourseById(course.CourseId);

            if (existingCourse == null)
                return;

            existingCourse.Title = course.Title;
            existingCourse.Description = course.Description;
            existingCourse.Category = course.Category;
            existingCourse.Level = course.Level;
            existingCourse.ThumbnailUrl = course.ThumbnailUrl;
            existingCourse.Price = course.Price;
            existingCourse.Status = course.Status;
            existingCourse.UpdatedAt = DateTime.UtcNow;
        }


        // =========================================================
        // MATERIALS
        // =========================================================

        public List<Material> GetMaterials(int courseId)
        {
            return _materials
                .Where(x => x.CourseId == courseId)
                .OrderBy(x => x.DisplayOrder)
                .ToList();
        }


        public Material? GetMaterialById(int materialId)
        {
            return _materials
                .FirstOrDefault(x => x.MaterialId == materialId);
        }


        public void AddMaterial(Material material)
        {
            material.MaterialId = _materials.Count == 0
                ? 1
                : _materials.Max(x => x.MaterialId) + 1;

            _materials.Add(material);
        }


        public void UpdateMaterial(Material material)
        {
            var existingMaterial = GetMaterialById(material.MaterialId);

            if (existingMaterial == null)
                return;

            existingMaterial.Title = material.Title;
            existingMaterial.Description = material.Description;
            existingMaterial.MaterialType = material.MaterialType;
            existingMaterial.FileUrl = material.FileUrl;
            existingMaterial.DurationInMinutes = material.DurationInMinutes;
            existingMaterial.DisplayOrder = material.DisplayOrder;
            existingMaterial.IsPublished = material.IsPublished;
        }


        public void DeleteMaterial(int materialId)
        {
            var material = GetMaterialById(materialId);

            if (material == null)
                return;

            _materials.Remove(material);
        }


        // =========================================================
        // ENROLLMENTS
        // =========================================================

        public List<Enrollment> GetEnrollments(int courseId)
        {
            return _enrollments
                .Where(x => x.CourseId == courseId)
                .ToList();
        }


        // =========================================================
        // ANNOUNCEMENTS
        // =========================================================

        public List<Announcement> GetAnnouncements(int courseId)
        {
            return _announcements
                .Where(x => x.CourseId == courseId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }


        public Announcement? GetAnnouncementById(int announcementId)
        {
            return _announcements
                .FirstOrDefault(x => x.AnnouncementId == announcementId);
        }


        public void AddAnnouncement(Announcement announcement)
        {
            announcement.AnnouncementId = _announcements.Count == 0
                ? 1
                : _announcements.Max(x => x.AnnouncementId) + 1;

            _announcements.Add(announcement);
        }


        public void UpdateAnnouncement(Announcement announcement)
        {
            var existingAnnouncement =
                GetAnnouncementById(announcement.AnnouncementId);

            if (existingAnnouncement == null)
                return;

            existingAnnouncement.Title = announcement.Title;
            existingAnnouncement.Message = announcement.Message;
            existingAnnouncement.IsPublished = announcement.IsPublished;
        }


        public void DeleteAnnouncement(int announcementId)
        {
            var announcement =
                GetAnnouncementById(announcementId);

            if (announcement == null)
                return;

            _announcements.Remove(announcement);
        }


        // =========================================================
        // LIVE LECTURES
        // =========================================================

        public List<LiveLecture> GetLiveLectures(int courseId)
        {
            return _liveLectures
                .Where(x => x.CourseId == courseId)
                .OrderBy(x => x.StartTime)
                .ToList();
        }


        public LiveLecture? GetLiveLectureById(int liveLectureId)
        {
            return _liveLectures
                .FirstOrDefault(x => x.LiveLectureId == liveLectureId);
        }


        public void AddLiveLecture(LiveLecture lecture)
        {
            lecture.LiveLectureId = _liveLectures.Count == 0
                ? 1
                : _liveLectures.Max(x => x.LiveLectureId) + 1;

            _liveLectures.Add(lecture);
        }


        public void UpdateLiveLecture(LiveLecture lecture)
        {
            var existingLecture =
                GetLiveLectureById(lecture.LiveLectureId);

            if (existingLecture == null)
                return;

            existingLecture.Title = lecture.Title;
            existingLecture.Description = lecture.Description;
            existingLecture.StartTime = lecture.StartTime;
            existingLecture.EndTime = lecture.EndTime;
            existingLecture.MeetingUrl = lecture.MeetingUrl;
            existingLecture.Status = lecture.Status;
        }


        public void DeleteLiveLecture(int liveLectureId)
        {
            var lecture =
                GetLiveLectureById(liveLectureId);

            if (lecture == null)
                return;

            _liveLectures.Remove(lecture);
        }


        // =========================================================
        // DASHBOARD
        // =========================================================

        public int GetTotalStudents(int instructorId)
        {
            var courseIds = _courses
                .Where(x => x.InstructorId == instructorId)
                .Select(x => x.CourseId)
                .ToList();

            return _enrollments
                .Where(x => courseIds.Contains(x.CourseId))
                .Select(x => x.StudentId)
                .Distinct()
                .Count();
        }
    }
}