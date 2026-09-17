using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudySphere.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "int", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminId);
                    table.ForeignKey(
                        name: "FK_Admins_Users_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Instructors",
                columns: table => new
                {
                    InstructorId = table.Column<int>(type: "int", nullable: false),
                    ProfessionalTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AreaOfExpertise = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Qualification = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProfileImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructors", x => x.InstructorId);
                    table.ForeignKey(
                        name: "FK_Instructors_Users_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    EnrollmentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProfileImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentId);
                    table.ForeignKey(
                        name: "FK_Students_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InstructorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_Courses_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "InstructorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    AnnouncementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    InstructorId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.AnnouncementId);
                    table.ForeignKey(
                        name: "FK_Announcements_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Announcements_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "InstructorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    EnrollmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Progress = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.EnrollmentId);
                    table.ForeignKey(
                        name: "FK_Enrollments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LiveLectures",
                columns: table => new
                {
                    LiveLectureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    InstructorId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MeetingUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveLectures", x => x.LiveLectureId);
                    table.ForeignKey(
                        name: "FK_LiveLectures_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LiveLectures_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "InstructorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    MaterialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MaterialType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DurationInMinutes = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.MaterialId);
                    table.ForeignKey(
                        name: "FK_Materials_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "Phone", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "student1@studysphere.com", "Student One", true, "password", "9876543201", "Student" },
                    { 2, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "student2@studysphere.com", "Student Two", true, "password", "9876543202", "Student" },
                    { 3, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "student3@studysphere.com", "Student Three", true, "password", "9876543203", "Student" },
                    { 4, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "student4@studysphere.com", "Student Four", true, "password", "9876543204", "Student" },
                    { 5, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "student5@studysphere.com", "Student Five", true, "password", "9876543205", "Student" },
                    { 6, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "instructor1@studysphere.com", "Instructor One", true, "password", "9876543210", "Instructor" },
                    { 7, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "instructor2@studysphere.com", "Instructor Two", true, "password", "9876543211", "Instructor" },
                    { 8, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "instructor3@studysphere.com", "Instructor Three", true, "password", "9876543212", "Instructor" },
                    { 9, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@studysphere.com", "System Admin", true, "password", "9876543213", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "AdminId", "Designation" },
                values: new object[] { 9, "System Administrator" });

            migrationBuilder.InsertData(
                table: "Instructors",
                columns: new[] { "InstructorId", "AreaOfExpertise", "Bio", "ProfessionalTitle", "ProfileImage", "Qualification" },
                values: new object[,]
                {
                    { 6, "Web Development", "Experienced instructor in web and backend development.", "Senior Software Developer", null, "B.Tech Computer Engineering" },
                    { 7, "Java and Spring Boot", "Experienced Java and backend development instructor.", "Java Developer", null, "M.Tech Computer Science" },
                    { 8, "Database Management", "Experienced database management instructor.", "Database Specialist", null, "M.Tech Information Technology" }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "StudentId", "Bio", "EnrollmentNo", "ProfileImage" },
                values: new object[,]
                {
                    { 1, "Computer Engineering student.", "STU001", null },
                    { 2, "Computer Engineering student.", "STU002", null },
                    { 3, "Computer Engineering student.", "STU003", null },
                    { 4, "Computer Engineering student.", "STU004", null },
                    { 5, "Computer Engineering student.", "STU005", null }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "CourseId", "Category", "CreatedAt", "Description", "InstructorId", "Level", "Price", "Status", "ThumbnailUrl", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Web Development", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Learn ASP.NET Core MVC from basics to advanced concepts.", 6, "Intermediate", 1499m, "Approved", "/images/courses/aspnet.jpg", "ASP.NET Core MVC", null },
                    { 2, "Programming", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Learn Java programming and backend development fundamentals.", 7, "Beginner", 999m, "Approved", "/images/courses/java.jpg", "Java Programming", null },
                    { 3, "Programming", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Learn C# programming from fundamentals to advanced concepts.", 6, "Beginner", 799m, "Pending", "/images/courses/csharp.jpg", "Complete C# Programming", null },
                    { 4, "Database", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Learn database concepts, SQL and database design.", 8, "Intermediate", 1199m, "Pending", "/images/courses/database.jpg", "Database Management System", null },
                    { 5, "Web Development", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Learn HTML, CSS and basic web design concepts.", 7, "Beginner", 599m, "Rejected", "/images/courses/webdesign.jpg", "Basic Web Design", null }
                });

            migrationBuilder.InsertData(
                table: "Announcements",
                columns: new[] { "AnnouncementId", "CourseId", "CreatedAt", "InstructorId", "IsPublished", "Message", "Title" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, true, "Welcome to the ASP.NET Core MVC course.", "Welcome to ASP.NET Core MVC" },
                    { 2, 1, new DateTime(2026, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, true, "Assignment 1 has been published.", "Assignment 1" },
                    { 3, 2, new DateTime(2026, 9, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, true, "Practice the Spring Boot concepts covered in the lectures.", "Spring Boot Practice" }
                });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "EnrollmentId", "CompletedAt", "CourseId", "EnrolledAt", "Progress", "Status", "StudentId" },
                values: new object[,]
                {
                    { 1, null, 1, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 75m, "Active", 1 },
                    { 2, null, 1, new DateTime(2026, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 50m, "Active", 2 },
                    { 3, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2026, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 100m, "Completed", 3 },
                    { 4, null, 1, new DateTime(2026, 9, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 40m, "Active", 4 }
                });

            migrationBuilder.InsertData(
                table: "LiveLectures",
                columns: new[] { "LiveLectureId", "CourseId", "CreatedAt", "Description", "EndTime", "InstructorId", "MeetingUrl", "StartTime", "Status", "Title" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Live lecture about controllers in ASP.NET Core MVC.", new DateTime(2026, 9, 18, 17, 0, 0, 0, DateTimeKind.Unspecified), 6, "https://meet.example.com/aspnet", new DateTime(2026, 9, 18, 16, 0, 0, 0, DateTimeKind.Unspecified), "Scheduled", "ASP.NET Core MVC - Controllers" },
                    { 2, 2, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Live lecture about creating REST APIs using Spring Boot.", new DateTime(2026, 9, 19, 18, 0, 0, 0, DateTimeKind.Unspecified), 7, "https://meet.example.com/spring", new DateTime(2026, 9, 19, 17, 0, 0, 0, DateTimeKind.Unspecified), "Scheduled", "Spring Boot REST API" }
                });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "MaterialId", "CourseId", "Description", "DisplayOrder", "DurationInMinutes", "FileUrl", "IsPublished", "MaterialType", "Title", "UploadedAt" },
                values: new object[,]
                {
                    { 1, 1, "Introduction to ASP.NET Core MVC.", 1, 25, "/materials/aspnet/introduction.mp4", true, "Video", "Introduction to ASP.NET Core MVC", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 1, "PDF notes explaining MVC architecture.", 2, null, "/materials/aspnet/mvc-notes.pdf", true, "PDF", "MVC Architecture Notes", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 1, "Understanding controllers and action methods.", 3, 30, "/materials/aspnet/controllers.mp4", true, "Video", "Controllers in ASP.NET Core", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 2, "Introduction to Spring Boot.", 1, 35, "/materials/java/spring-introduction.mp4", true, "Video", "Spring Boot Introduction", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 2, "Spring Boot learning notes.", 2, null, "/materials/java/spring-notes.pdf", false, "PDF", "Spring Boot Notes", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_CourseId",
                table: "Announcements",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_InstructorId",
                table: "Announcements",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstructorId",
                table: "Courses",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId",
                table: "Enrollments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveLectures_CourseId",
                table: "LiveLectures",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveLectures_InstructorId",
                table: "LiveLectures",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_CourseId",
                table: "Materials",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "LiveLectures");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Instructors");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
