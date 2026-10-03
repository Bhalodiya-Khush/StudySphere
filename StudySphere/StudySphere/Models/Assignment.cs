using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudySphere.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(5000)]
        public string Instructions { get; set; } = string.Empty;

        [Range(1, 10000)]
        [Column(TypeName = "decimal(8,2)")]
        public decimal MaxMarks { get; set; }

        [Required]
        public DateTime DueAt { get; set; }

        public bool IsPublished { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Course Course { get; set; } = null!;

        public ICollection<AssignmentSubmission> Submissions { get; set; } =
            new List<AssignmentSubmission>();
    }
}
