using System.ComponentModel.DataAnnotations;

namespace StudySphere.Models
{
    public class CourseCertificate
    {
        [Key]
        public int CourseCertificateId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(50)]
        public string CertificateCode { get; set; } = string.Empty;

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        public Student Student { get; set; } = null!;

        public Course Course { get; set; } = null!;
    }
}
