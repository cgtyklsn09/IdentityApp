using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels
{
    public class CreateViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        [Display(Name = "Password Again")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
