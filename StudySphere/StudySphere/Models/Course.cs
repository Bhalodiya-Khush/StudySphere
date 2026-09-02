using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public Instructor Instructor { get; set; } = null!;

        public ICollection<Enrollment> Enrollments { get; set; }
            = new List<Enrollment>();

        public ICollection<Material> Materials { get; set; }
            = new List<Material>();

        public ICollection<Announcement> Announcements { get; set; }
            = new List<Announcement>();

        public ICollection<LiveLecture> LiveLectures { get; set; }
            = new List<LiveLecture>();
    }
}