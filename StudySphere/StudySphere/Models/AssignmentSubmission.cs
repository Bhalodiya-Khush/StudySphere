using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudySphere.Models
{
    public class AssignmentSubmission
    {
        [Key]
        public int SubmissionId { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [StringLength(1000)]
        public string FileUrl { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? StudentComment { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(8,2)")]
        public decimal? MarksAwarded { get; set; }

        [StringLength(4000)]
        public string? Feedback { get; set; }

        public DateTime? EvaluatedAt { get; set; }

        public Assignment Assignment { get; set; } = null!;

        public Student Student { get; set; } = null!;
    }
}
