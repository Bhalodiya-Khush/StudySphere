using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudySphere.Models
{
    public class Announcement
    {
        [Key]
        public int AnnouncementId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int InstructorId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(3000)]
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsPublished { get; set; } = true;

        // Navigation properties
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = null!;

        [ForeignKey(nameof(InstructorId))]
        public Instructor Instructor { get; set; } = null!;
    }
}