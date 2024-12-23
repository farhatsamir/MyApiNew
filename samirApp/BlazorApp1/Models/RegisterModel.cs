using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models
{
    public class RegisterModel
    {
        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required, Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }

    }
}
