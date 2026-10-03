using System.ComponentModel.DataAnnotations;

namespace StudySphere.Models
{
    public class ProfileViewModel
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        [StringLength(15)]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(100)]
        public string? ProfessionalTitle { get; set; }

        [StringLength(200)]
        public string? AreaOfExpertise { get; set; }

        [StringLength(200)]
        public string? Qualification { get; set; }

        public string Role { get; set; } = string.Empty;

        public string? ProfileImage { get; set; }

        public int EnrolledCourses { get; set; }

        public int CompletedCourses { get; set; }

        public int Certificates { get; set; }

        public IFormFile? ProfilePicture { get; set; }
    }
}