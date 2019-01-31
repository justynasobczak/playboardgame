using System.ComponentModel.DataAnnotations;

namespace PlayBoardGame.Models.ViewModels
{
    public class UserProfileViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string PhoneNumber { get; set; }
    }
}
