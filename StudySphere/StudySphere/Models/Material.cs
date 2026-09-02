using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudySphere.Models
{
    public class Material
    {
        [Key]
        public int MaterialId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [StringLength(20)]
        public string MaterialType { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string FileUrl { get; set; } = string.Empty;

        // Used mainly for video
        public int? DurationInMinutes { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsPublished { get; set; } = false;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = null!;
    }
}