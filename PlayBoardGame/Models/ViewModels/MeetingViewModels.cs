using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PlayBoardGame.Models.ViewModels
{
    public class MeetingViewModels
    {
        public class CreateEditMeetingViewModel
        {
            public int MeetingId { get; set; }

            [Required] public string Title { get; set; }

            [Required]
            [Display(Name = "Start date and time")]
            public string StartDateTime { get; set; }

            [Required]
            [Display(Name = "End date and time")]
            public string EndDateTime { get; set; }

            public string Notes { get; set; }

            public bool IsEditable { get; set; }

            public string Organizer { get; set; }

            public AddressViewModels Address { get; set; }

            public List<int> SelectedGames { get; set; }

            public IEnumerable<Game> Games { get; set; }

            public string TimeZone { get; set; }
        }
    }
}