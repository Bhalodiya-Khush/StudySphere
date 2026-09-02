using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudySphere.Models
{
    public class Instructor
    {
        [Key]
        [ForeignKey(nameof(User))]
        public int InstructorId { get; set; }

        [StringLength(100)]
        public string? ProfessionalTitle { get; set; }

        [StringLength(200)]
        public string? AreaOfExpertise { get; set; }

        [StringLength(200)]
        public string? Qualification { get; set; }

        [StringLength(1000)]
        public string? Bio { get; set; }

        [StringLength(500)]
        public string? ProfileImage { get; set; }

        // Navigation property
        public User User { get; set; } = null!;

        public ICollection<Course> Courses { get; set; }
            = new List<Course>();

        public ICollection<Announcement> Announcements { get; set; }
            = new List<Announcement>();

        public ICollection<LiveLecture> LiveLectures { get; set; }
            = new List<LiveLecture>();
    }
}