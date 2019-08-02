using System.Collections.Generic;

namespace PlayBoardGame.Models.ViewModels
{
    public class AddressViewModels
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public Dictionary<string, string> Countries { get; set; } = Constants.Countries;
    }
}