using System.ComponentModel.DataAnnotations;

namespace StudySphere.ViewModels.Authentication
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        public string FullName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Enter a valid phone number.")]
        public string PhoneNumber { get; set; } = string.Empty;


        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;


        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;


        [Required(ErrorMessage = "Please select a role.")]
        public string Role { get; set; } = string.Empty;


        // Instructor-specific fields
        public string? ProfessionalTitle { get; set; }

        public string? AreaOfExpertise { get; set; }

        public string? Qualification { get; set; }
    }
}