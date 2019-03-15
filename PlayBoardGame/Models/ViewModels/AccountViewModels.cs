using System.ComponentModel.DataAnnotations;

namespace PlayBoardGame.Models.ViewModels
{

    public class RegisterViewModel
    {
        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; }

        [Required, MinLength(6)]
        [UIHint("password")]
        public string Password { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [UIHint("password")]
        public string Password { get; set; }
    }

    public class SendResetPasswordLinkViewModel
    {
        [Required]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required, MinLength(6)]
        [Display(Name = "New password")]
        [UIHint("password")]
        public string NewPassword { get; set; }

        public string EmailToken { get; set; }
    }
}