using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PlayBoardGame.Models.ViewModels
{
    public class UserProfileViewModel
    {
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        public AddressViewModels Address { get; set; }

        public string PhoneNumber { get; set; }
        
        public string TimeZone { get; set; }
        
        public SelectListItem[] TimeZoneList { get; set; }
        
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "Old password")]
        [UIHint("password")]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "New password")]
        [UIHint("password")]
        public string NewPassword { get; set; }
        
        [Required]
        [UIHint("password")]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
