using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudySphere.Models
{
    public class Admin
    {
        [Key]
        [ForeignKey(nameof(User))]
        public int AdminId { get; set; }

        [StringLength(100)]
        public string? Designation { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}