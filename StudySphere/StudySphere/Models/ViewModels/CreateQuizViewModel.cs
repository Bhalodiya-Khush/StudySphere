using System.ComponentModel.DataAnnotations;

namespace StudySphere.Models.ViewModels
{
    public class CreateQuizViewModel
    {
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
    }
}
