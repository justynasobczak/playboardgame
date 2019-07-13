using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PlayBoardGame.Models.ViewModels
{

    public class RegisterViewModel
    {
        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; }

        [Required, MinLength(6)]
        [UIHint("password")]
        public string Password { get; set; }
        
        [Required]
        [Display(Name = "Time zone")]
        public string TimeZone { get; set; }
        
        // bozy in ViewModels you have properties declared 3 ways:
        // Array, IEnumerable<> and List. Please decide for one. And IEnumerable is preferred one.
        public SelectListItem[] TimeZoneList { get; set; }
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