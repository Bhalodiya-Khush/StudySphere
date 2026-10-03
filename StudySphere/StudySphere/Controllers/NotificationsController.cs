using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySphere.Data;
using StudySphere.Models.ViewModels;

namespace StudySphere.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly StudySphereDbContext _dbContext;

        public NotificationsController(StudySphereDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email))
            {
                return Forbid();
            }

            var items = new List<NotificationItemViewModel>();
            if (User.IsInRole("Student"))
            {
                var studentId = await _dbContext.Students
                    .Where(student => student.User.Email.ToLower() == email.ToLower())
                    .Select(student => (int?)student.StudentId)
                    .SingleOrDefaultAsync();
                if (studentId is null)
                {
                    return Forbid();
                }

                var courseIds = await _dbContext.Enrollments
                    .Where(enrollment =>
                        enrollment.StudentId == studentId &&
                        enrollment.Status != "Withdrawn" &&
                        enrollment.Course.Status == "Approved")
                    .Select(enrollment => enrollment.CourseId)
                    .ToListAsync();
                var now = DateTime.UtcNow;

                var announcements = await _dbContext.Announcements
                    .AsNoTracking()
                    .Where(announcement =>
                        announcement.IsPublished &&
                        courseIds.Contains(announcement.CourseId))
                    .OrderByDescending(announcement => announcement.CreatedAt)
                    .Take(20)
                    .Select(announcement => new
                    {
                        announcement.Title,
                        announcement.Message,
                        announcement.CreatedAt,
                        announcement.CourseId,
                        CourseTitle = announcement.Course.Title
                    })
                    .ToListAsync();

                items.AddRange(announcements.Select(announcement =>
                    new NotificationItemViewModel
                    {
                        Title = announcement.Title,
                        Message = $"{announcement.CourseTitle}: {announcement.Message}",
                        Category = "Course announcement",
                        CreatedAt = announcement.CreatedAt,
                        Url = Url.Action(
                            "CourseDetails",
                            "StudentDashboard",
                            new { id = announcement.CourseId }) ?? string.Empty
                    }));

                var lectures = await _dbContext.LiveLectures
                    .AsNoTracking()
                    .Where(lecture =>
                        courseIds.Contains(lecture.CourseId) &&
                        lecture.Status == "Scheduled" &&
                        lecture.StartTime >= now)
                    .OrderBy(lecture => lecture.StartTime)
                    .Take(10)
                    .Select(lecture => new
                    {
                        lecture.Title,
                        lecture.StartTime,
                        lecture.CourseId,
                        CourseTitle = lecture.Course.Title
                    })
                    .ToListAsync();

                items.AddRange(lectures.Select(lecture =>
                    new NotificationItemViewModel
                    {
                        Title = $"Upcoming lecture: {lecture.Title}",
                        Message = $"{lecture.CourseTitle} · {lecture.StartTime.ToLocalTime():f}",
                        Category = "Live lecture",
                        CreatedAt = lecture.StartTime,
                        Url = Url.Action(
                            "CourseDetails",
                            "StudentDashboard",
                            new { id = lecture.CourseId }) ?? string.Empty
                    }));

                var assignments = await _dbContext.Assignments
                    .AsNoTracking()
                    .Where(assignment =>
                        assignment.IsPublished &&
                        assignment.DueAt >= now &&
                        courseIds.Contains(assignment.CourseId) &&
                        !_dbContext.AssignmentSubmissions.Any(submission =>
                            submission.AssignmentId == assignment.AssignmentId &&
                            submission.StudentId == studentId))
                    .OrderBy(assignment => assignment.DueAt)
                    .Take(10)
                    .Select(assignment => new
                    {
                        assignment.Title,
                        assignment.DueAt,
                        assignment.CourseId,
                        CourseTitle = assignment.Course.Title
                    })
                    .ToListAsync();

                items.AddRange(assignments.Select(assignment =>
                    new NotificationItemViewModel
                    {
                        Title = $"Assignment due: {assignment.Title}",
                        Message = $"{assignment.CourseTitle} · Due {assignment.DueAt.ToLocalTime():f}",
                        Category = "Assignment",
                        CreatedAt = assignment.DueAt,
                        Url = Url.Action(
                            "Assignments",
                            "StudentDashboard",
                            new { courseId = assignment.CourseId }) ?? string.Empty
                    }));
            }
            else if (User.IsInRole("Instructor"))
            {
                var instructorId = await _dbContext.Instructors
                    .Where(instructor => instructor.User.Email.ToLower() == email.ToLower())
                    .Select(instructor => (int?)instructor.InstructorId)
                    .SingleOrDefaultAsync();
                if (instructorId is null)
                {
                    return Forbid();
                }

                var courses = await _dbContext.Courses
                    .AsNoTracking()
                    .Where(course => course.InstructorId == instructorId)
                    .OrderByDescending(course => course.UpdatedAt ?? course.CreatedAt)
                    .Take(20)
                    .Select(course => new
                    {
                        course.Title,
                        course.Status,
                        course.CreatedAt,
                        course.UpdatedAt
                    })
                    .ToListAsync();

                items.AddRange(courses
                    .Where(course => course.Status is "Approved" or "Rejected" or "Pending")
                    .Select(course =>
                    {
                        var statusMessage = course.Status == "Pending"
                            ? "is waiting for administrator review"
                            : $"was {course.Status.ToLowerInvariant()}";
                        return new NotificationItemViewModel
                        {
                            Title = course.Status == "Pending"
                                ? "Course submitted for review"
                                : $"Course {course.Status.ToLowerInvariant()}",
                            Message = $"“{course.Title}” {statusMessage}.",
                            Category = "Course review",
                            CreatedAt = course.UpdatedAt ?? course.CreatedAt,
                            Url = Url.Action(
                                "Courses",
                                "InstructorDashboard") ?? string.Empty
                        };
                    }));

                var submissions = await _dbContext.AssignmentSubmissions
                    .AsNoTracking()
                    .Where(submission =>
                        submission.Assignment.Course.InstructorId == instructorId)
                    .OrderByDescending(submission => submission.SubmittedAt)
                    .Take(20)
                    .Select(submission => new
                    {
                        submission.SubmittedAt,
                        submission.AssignmentId,
                        AssignmentTitle = submission.Assignment.Title,
                        StudentName = submission.Student.User.FullName
                    })
                    .ToListAsync();

                items.AddRange(submissions.Select(submission =>
                    new NotificationItemViewModel
                    {
                        Title = "New assignment submission",
                        Message = $"{submission.StudentName} submitted “{submission.AssignmentTitle}”.",
                        Category = "Student activity",
                        CreatedAt = submission.SubmittedAt,
                        Url = Url.Action(
                            "AssignmentSubmissions",
                            "InstructorDashboard",
                            new { assignmentId = submission.AssignmentId }) ?? string.Empty
                    }));
            }
            else if (User.IsInRole("Admin"))
            {
                var pendingCourses = await _dbContext.Courses
                    .AsNoTracking()
                    .Where(course => course.Status == "Pending")
                    .OrderByDescending(course => course.CreatedAt)
                    .Take(20)
                    .Select(course => new
                    {
                        course.Title,
                        course.CreatedAt
                    })
                    .ToListAsync();

                items.AddRange(pendingCourses.Select(course =>
                    new NotificationItemViewModel
                    {
                        Title = "Course awaiting review",
                        Message = $"“{course.Title}” is waiting for approval.",
                        Category = "Course review",
                        CreatedAt = course.CreatedAt,
                        Url = Url.Action(
                            "CourseRequests",
                            "AdminDashboard") ?? string.Empty
                    }));

                var pendingInstructors = await _dbContext.Instructors
                    .AsNoTracking()
                    .Where(instructor => !instructor.User.IsActive)
                    .OrderByDescending(instructor => instructor.User.CreatedAt)
                    .Take(20)
                    .Select(instructor => new
                    {
                        instructor.User.FullName,
                        instructor.User.CreatedAt
                    })
                    .ToListAsync();

                items.AddRange(pendingInstructors.Select(instructor =>
                    new NotificationItemViewModel
                    {
                        Title = "Instructor awaiting activation",
                        Message = $"{instructor.FullName} is waiting for account approval.",
                        Category = "Account review",
                        CreatedAt = instructor.CreatedAt,
                        Url = Url.Action(
                            "Instructors",
                            "AdminDashboard") ?? string.Empty
                    }));
            }
            else
            {
                return Forbid();
            }

            return View(new NotificationViewModel
            {
                Items = items
                    .OrderByDescending(item => item.CreatedAt)
                    .Take(50)
                    .ToList()
            });
        }
    }
}
