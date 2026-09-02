using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudySphere.Models
{
    public class LiveLecture
    {
        [Key]
        public int LiveLectureId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int InstructorId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [StringLength(1000)]
        public string? MeetingUrl { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Scheduled";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = null!;

        [ForeignKey(nameof(InstructorId))]
        public Instructor Instructor { get; set; } = null!;
    }
}