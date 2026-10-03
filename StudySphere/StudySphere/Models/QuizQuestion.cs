using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudySphere.Models
{
    public class QuizQuestion
    {
        [Key]
        public int QuizQuestionId { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Prompt { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string OptionA { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string OptionB { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string OptionC { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string OptionD { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[A-D]$")]
        public string CorrectOption { get; set; } = "A";

        [Range(0.01, 10000)]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Marks { get; set; } = 1;

        public Quiz Quiz { get; set; } = null!;
    }
}
