using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudySphere.Models
{
    public class StudentDashboardViewModels
    {
        [Key]
        [ForeignKey(nameof(User))]
        public int StudentId { get; set; }

        [StringLength(50)]
        public string? EnrollmentNo { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(500)]
        public string? ProfileImage { get; set; }

        // Navigation property
        public User User { get; set; } = null!;

        public ICollection<Enrollment> Enrollments { get; set; }
            = new List<Enrollment>();

        public ICollection<LiveLecture> LiveLectures { get; set; }
            = new List<LiveLecture>();
    }
}