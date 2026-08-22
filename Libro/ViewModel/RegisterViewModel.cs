using System.ComponentModel.DataAnnotations;

namespace Libro.ViewModel
{
    public class RegisterViewModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Fullname { get; set; }
        [Required]
        [AllowedValues("Librarian", "Member", ErrorMessage = "Role must be either 'Librarian' or 'Member'.")]
        public string Role { get; set; }
        public string? ImageUrl { get; set; }
    }
}
