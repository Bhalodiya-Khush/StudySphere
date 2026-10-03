using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudySphere.Models
{
    public class QuizAttempt
    {
        [Key]
        public int QuizAttemptId { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Range(1, 20)]
        public int AttemptNumber { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal Score { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal MaxScore { get; set; }

        [Required]
        [StringLength(4000)]
        public string AnswersJson { get; set; } = "{}";

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public Quiz Quiz { get; set; } = null!;

        public Student Student { get; set; } = null!;
    }
}
