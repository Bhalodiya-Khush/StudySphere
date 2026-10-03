using System.ComponentModel.DataAnnotations;

namespace StudySphere.Models
{
    public class LessonProgress
    {
        [Key]
        public int LessonProgressId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int MaterialId { get; set; }

        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        public Student Student { get; set; } = null!;

        public Material Material { get; set; } = null!;
    }
}
