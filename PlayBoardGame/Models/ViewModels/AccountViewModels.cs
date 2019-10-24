using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace PlayBoardGame.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; }

        [Required] [UIHint("password")] public string Password { get; set; }

        [Required]
        [UIHint("password")]
        [Display(Name = "Confirm password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Time zone")]
        public string TimeZone { get; set; }

        // bozy in ViewModels you have properties declared 3 ways:
        // Array, IEnumerable<> and List. Please decide for one. And IEnumerable is preferred one.
        public List<KeyValuePair<string, string>> TimeZoneList { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }

    public class LoginViewModel
    {
        [Required] public string Email { get; set; }

        [Required] [UIHint("password")] public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string TimeZone { get; set; }
    }

    public class SendResetPasswordLinkViewModel
    {
        [Required] public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required] public string Email { get; set; }

        [Required]
        [Display(Name = "New password")]
        [UIHint("password")]
        public string NewPassword { get; set; }

        [Required]
        [UIHint("password")]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }

        public string EmailToken { get; set; }
    }

    public enum AuthPageType
    {
        Login,
        Register
    }
}