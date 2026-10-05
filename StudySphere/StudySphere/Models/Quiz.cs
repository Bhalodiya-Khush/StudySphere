using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudySphere.Models
{
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(3000)]
        public string? Description { get; set; }

        [Required]
        public DateTime OpensAt { get; set; } = DateTime.UtcNow;

        public DateTime? DueAt { get; set; }

        [Range(1, 20)]
        public int MaxAttempts { get; set; } = 1;

        public bool IsPublished { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ValidateNever]
        public Course Course { get; set; } = null!;

        [ValidateNever]
        public ICollection<QuizQuestion> Questions { get; set; } =
            new List<QuizQuestion>();

        [ValidateNever]
        public ICollection<QuizAttempt> Attempts { get; set; } =
            new List<QuizAttempt>();
    }
}
