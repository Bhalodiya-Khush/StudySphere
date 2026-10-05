using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudySphere.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(50)]
        public string? Level { get; set; }

        [StringLength(500)]
        public string? ThumbnailUrl { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Draft";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign Key
        [Required]
        public int InstructorId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(InstructorId))]
        [ValidateNever]
        public Instructor Instructor { get; set; } = null!;

        [ValidateNever]
        public ICollection<Enrollment> Enrollments { get; set; }
            = new List<Enrollment>();

        [ValidateNever]
        public ICollection<Material> Materials { get; set; }
            = new List<Material>();

        [ValidateNever]
        public ICollection<Announcement> Announcements { get; set; }
            = new List<Announcement>();

        [ValidateNever]
        public ICollection<LiveLecture> LiveLectures { get; set; }
            = new List<LiveLecture>();

        [ValidateNever]
        public ICollection<Assignment> Assignments { get; set; }
            = new List<Assignment>();

        [ValidateNever]
        public ICollection<Quiz> Quizzes { get; set; }
            = new List<Quiz>();

        [ValidateNever]
        public ICollection<CourseCertificate> Certificates { get; set; }
            = new List<CourseCertificate>();
    }
}