using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace StudySphere.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
    }
}