using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudySphere.Models;

namespace StudySphere.Data;

public static class DevelopmentDataSeeder
{
    public const string DemoPassword = "StudySphere@123";

    public static async Task SeedAsync(
        IServiceProvider services,
        string contentRootPath)
    {
        var dbContext = services.GetRequiredService<StudySphereDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var pendingMigrations = await dbContext.Database
            .GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            throw new InvalidOperationException(
                "The development database has pending EF Core migrations. Run 'dotnet ef database update' before starting StudySphere.");
        }

        var sampleProfiles = await dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.UserId)
            .ToListAsync();

        foreach (var profile in sampleProfiles)
        {
            var email = profile.Email.Trim().ToLowerInvariant();
            var identityUser = await userManager.FindByEmailAsync(email);
            if (identityUser is null)
            {
                identityUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = profile.FullName,
                    PhoneNumber = profile.Phone
                };

                var createResult = await userManager.CreateAsync(
                    identityUser,
                    DemoPassword);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        createResult.Errors.Select(error => error.Description));
                    throw new InvalidOperationException(
                        $"Could not create the development account '{email}': {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(identityUser, profile.Role))
            {
                var roleResult = await userManager.AddToRoleAsync(
                    identityUser,
                    profile.Role);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        roleResult.Errors.Select(error => error.Description));
                    throw new InvalidOperationException(
                        $"Could not assign the '{profile.Role}' role to '{email}': {errors}");
                }
            }
        }

        await SeedAssignmentsAsync(dbContext);
        await SeedQuizzesAsync(dbContext);
        await SeedAssignmentSubmissionAsync(dbContext, contentRootPath);
        await SeedQuizAttemptAsync(dbContext);
        await SeedLessonProgressAndCertificateAsync(dbContext);
        await SeedCourseMaterialsAndThumbnailsAsync(dbContext, contentRootPath);
    }

    private static async Task SeedAssignmentsAsync(
        StudySphereDbContext dbContext)
    {
        var now = DateTime.UtcNow;
        var assignments = new[]
        {
            new Assignment
            {
                CourseId = 1,
                Title = "MVC Fundamentals Assignment",
                Instructions = "Create a small ASP.NET Core MVC application with a controller, view, and model.",
                MaxMarks = 100,
                DueAt = now.AddDays(14),
                IsPublished = true
            },
            new Assignment
            {
                CourseId = 2,
                Title = "Java Basics Assignment",
                Instructions = "Write a Java program that demonstrates classes, methods, and collections.",
                MaxMarks = 50,
                DueAt = now.AddDays(21),
                IsPublished = true
            }
        };

        foreach (var assignment in assignments)
        {
            if (!await dbContext.Assignments.AnyAsync(existing =>
                    existing.CourseId == assignment.CourseId &&
                    existing.Title == assignment.Title))
            {
                dbContext.Assignments.Add(assignment);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedQuizzesAsync(
        StudySphereDbContext dbContext)
    {
        var now = DateTime.UtcNow;
        var quizDefinitions = new[]
        {
            new Quiz
            {
                CourseId = 1,
                Title = "MVC Fundamentals Quiz",
                Description = "A short quiz covering MVC concepts.",
                OpensAt = now.AddDays(-1),
                DueAt = now.AddDays(30),
                MaxAttempts = 3,
                IsPublished = true,
                Questions =
                {
                    CreateQuestion(
                        "What does MVC stand for?",
                        "Model-View-Controller",
                        "Multiple-View-Computer",
                        "Model-Variable-Class",
                        "Main-View-Component",
                        "A"),
                    CreateQuestion(
                        "Which component handles incoming requests?",
                        "View",
                        "Controller",
                        "CSS",
                        "Database",
                        "B")
                }
            },
            new Quiz
            {
                CourseId = 2,
                Title = "Java Basics Quiz",
                Description = "Review Java programming fundamentals.",
                OpensAt = now.AddDays(-1),
                DueAt = now.AddDays(30),
                MaxAttempts = 3,
                IsPublished = true,
                Questions =
                {
                    CreateQuestion(
                        "Which keyword declares a class in Java?",
                        "function",
                        "define",
                        "class",
                        "struct",
                        "C")
                }
            }
        };

        foreach (var quiz in quizDefinitions)
        {
            if (!await dbContext.Quizzes.AnyAsync(existing =>
                    existing.CourseId == quiz.CourseId &&
                    existing.Title == quiz.Title))
            {
                dbContext.Quizzes.Add(quiz);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static QuizQuestion CreateQuestion(
        string prompt,
        string optionA,
        string optionB,
        string optionC,
        string optionD,
        string correctOption)
    {
        return new QuizQuestion
        {
            Prompt = prompt,
            OptionA = optionA,
            OptionB = optionB,
            OptionC = optionC,
            OptionD = optionD,
            CorrectOption = correctOption,
            Marks = 5
        };
    }

    private static async Task SeedAssignmentSubmissionAsync(
        StudySphereDbContext dbContext,
        string contentRootPath)
    {
        var assignment = await dbContext.Assignments
            .FirstAsync(item => item.Title == "MVC Fundamentals Assignment");
        var student = await dbContext.Students
            .FirstAsync(item => item.User.Email == "student1@studysphere.com");

        if (await dbContext.AssignmentSubmissions.AnyAsync(submission =>
                submission.AssignmentId == assignment.AssignmentId &&
                submission.StudentId == student.StudentId))
        {
            return;
        }

        const string relativePath =
            "assignment-submissions/1/demo-submission.txt";
        var filePath = Path.Combine(
            contentRootPath,
            "App_Data",
            relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        await File.WriteAllTextAsync(
            filePath,
            "Development sample submission for the MVC Fundamentals Assignment.");

        dbContext.AssignmentSubmissions.Add(new AssignmentSubmission
        {
            AssignmentId = assignment.AssignmentId,
            StudentId = student.StudentId,
            FileUrl = relativePath,
            StudentComment = "Development sample submission.",
            SubmittedAt = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedQuizAttemptAsync(
        StudySphereDbContext dbContext)
    {
        var quiz = await dbContext.Quizzes
            .Include(item => item.Questions)
            .FirstAsync(item => item.Title == "MVC Fundamentals Quiz");
        var student = await dbContext.Students
            .FirstAsync(item => item.User.Email == "student1@studysphere.com");

        if (await dbContext.QuizAttempts.AnyAsync(attempt =>
                attempt.QuizId == quiz.QuizId &&
                attempt.StudentId == student.StudentId))
        {
            return;
        }

        var answers = quiz.Questions.ToDictionary(
            question => question.QuizQuestionId,
            question => question.CorrectOption);
        var submittedAt = DateTime.UtcNow.AddHours(-1);
        dbContext.QuizAttempts.Add(new QuizAttempt
        {
            QuizId = quiz.QuizId,
            StudentId = student.StudentId,
            AttemptNumber = 1,
            Score = quiz.Questions.Sum(question => question.Marks),
            MaxScore = quiz.Questions.Sum(question => question.Marks),
            AnswersJson = JsonSerializer.Serialize(answers),
            StartedAt = submittedAt.AddMinutes(-10),
            SubmittedAt = submittedAt
        });
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedLessonProgressAndCertificateAsync(
        StudySphereDbContext dbContext)
    {
        var student = await dbContext.Students
            .FirstAsync(item => item.User.Email == "student3@studysphere.com");
        var material = await dbContext.Materials
            .FirstAsync(item =>
                item.CourseId == 2 &&
                item.IsPublished);

        if (!await dbContext.LessonProgress.AnyAsync(progress =>
                progress.StudentId == student.StudentId &&
                progress.MaterialId == material.MaterialId))
        {
            dbContext.LessonProgress.Add(new LessonProgress
            {
                StudentId = student.StudentId,
                MaterialId = material.MaterialId,
                CompletedAt = DateTime.UtcNow.AddDays(-2)
            });
        }

        if (!await dbContext.CourseCertificates.AnyAsync(certificate =>
                certificate.StudentId == student.StudentId &&
                certificate.CourseId == 2))
        {
            dbContext.CourseCertificates.Add(new CourseCertificate
            {
                StudentId = student.StudentId,
                CourseId = 2,
                CertificateCode = "DEV-JAVA-CERT-0001",
                IssuedAt = DateTime.UtcNow.AddDays(-1)
            });
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedCourseMaterialsAndThumbnailsAsync(
        StudySphereDbContext dbContext,
        string contentRootPath)
    {
        var appData = Path.Combine(contentRootPath, "App_Data");
        var thumbnailsDir = Path.Combine(appData, "course-thumbnails");
        var materials1Dir = Path.Combine(appData, "course-materials", "1");
        var materials2Dir = Path.Combine(appData, "course-materials", "2");

        Directory.CreateDirectory(thumbnailsDir);
        Directory.CreateDirectory(materials1Dir);
        Directory.CreateDirectory(materials2Dir);

        // Normalize Courses thumbnails in DB
        var courses = await dbContext.Courses.ToListAsync();
        foreach (var course in courses)
        {
            if (string.IsNullOrWhiteSpace(course.ThumbnailUrl) ||
                course.ThumbnailUrl.StartsWith("/images/courses/", StringComparison.OrdinalIgnoreCase))
            {
                var fileName = course.CourseId switch
                {
                    1 => "aspnet.jpg",
                    2 => "java.jpg",
                    3 => "csharp.jpg",
                    4 => "database.jpg",
                    5 => "webdesign.jpg",
                    _ => "default-course.jpg"
                };
                course.ThumbnailUrl = $"/course-thumbnails/{fileName}";
            }
        }

        // Normalize Materials FileUrl in DB
        var materials = await dbContext.Materials.ToListAsync();
        foreach (var material in materials)
        {
            if (material.FileUrl.StartsWith("/materials/", StringComparison.OrdinalIgnoreCase) ||
                material.FileUrl.StartsWith("materials/", StringComparison.OrdinalIgnoreCase))
            {
                material.FileUrl = material.MaterialId switch
                {
                    1 => "course-materials/1/introduction.mp4",
                    2 => "course-materials/1/mvc-notes.pdf",
                    3 => "course-materials/1/controllers.mp4",
                    4 => "course-materials/2/spring-introduction.mp4",
                    5 => "course-materials/2/spring-notes.pdf",
                    _ => material.FileUrl.Replace("/materials/", "course-materials/").TrimStart('/')
                };
            }
        }

        await dbContext.SaveChangesAsync();
    }
}
