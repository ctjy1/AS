using System.ComponentModel.DataAnnotations;

namespace Assignment2.ViewModels
{
    public class PasswordResetRequestViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }
}
